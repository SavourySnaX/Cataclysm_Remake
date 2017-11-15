using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterGoblinController : MonoBehaviour, IEnemyBase
{
	enum Action
	{
		Left,
		Right,
		Up,
		Down,

		Min = Left,
		Max = Down
	}

	public float speed = 5.0f;
	public float chanceToRepeatAction = 0.10f;
	public float detectionRange=6f;
	public bool lethal = false;
	public bool diesToWater=false;
	public BitmapCollision bmpCol;
	public AnimationCurve AIWeighting;
	public PlayerController player;
	public GameObject deathParticlePrefab;
	public GameObject weaponPrefab;
	ProjectileTrigger weapon;
	public EnemySpawner spawner;
	Vector3 position;
	Action currentAction;
	bool dead;
	bool lookingLeft;
	BitmapCollision.LayerMask colType=BitmapCollision.LayerMask.Enemy;
	Animator anim;

	GlobalAudioManager globalAudio;

	float nextDelay;

	public void Init (BitmapCollision _bmpCol, PlayerController _player, EnemySpawner _spawn, BitmapCollision.LayerMask _colType)
	{
		bmpCol = _bmpCol;
		player = _player;
		spawner = _spawn;
		colType = _colType;
		weapon = Instantiate(weaponPrefab, transform).GetComponent<ProjectileTrigger>();
		weapon.Init(bmpCol);
		weapon.colMask = BitmapCollision.LayerMask.All & ~(BitmapCollision.LayerMask.Enemy|BitmapCollision.LayerMask.Water);
		weapon.colType = BitmapCollision.LayerMask.Bullet;
		weapon.player = player;
	}

	void Start()
	{
		lookingLeft = true;
		dead = false;
		position = transform.position;
		currentAction = Action.Left;
		nextDelay = 0.0f;
		globalAudio = GameObject.Find("GlobalAudio").GetComponent<GlobalAudioManager> ();
		anim = GetComponent<Animator>();
	}

	void IdleAnim()
	{
		anim.SetBool("left", false);
		anim.SetBool("right", false);
		anim.SetBool("up", false);
		anim.SetBool("down", false);
	}

	void LeftAnim()
	{
		anim.SetBool("left", true);
		anim.SetBool("right", false);
		anim.SetBool("up", false);
		anim.SetBool("down", false);
	}

	void RightAnim()
	{
		anim.SetBool("left", true);
		anim.SetBool("right", false);
		anim.SetBool("up", false);
		anim.SetBool("down", false);
	}

	void DownAnim()
	{
		anim.SetBool("left", false);
		anim.SetBool("right", false);
		anim.SetBool("up", false);
		anim.SetBool("down", true);
	}

	void UpAnim()
	{
		anim.SetBool("left", false);
		anim.SetBool("right", false);
		anim.SetBool("up", true);
		anim.SetBool("down", false);
		anim.SetBool("sleep", false);
	}

	Action GetAction()
	{
		// Check if  player is directly horizontal to creature, if it is and its facing the right way, we can fire

		float dirY = player.transform.position.y - transform.position.y;
		if (Mathf.Abs( dirY ) < 1.0f)
		{
			float dirX = transform.position.x - player.transform.position.x;

			if ((lookingLeft && dirX > 0f)||(!lookingLeft && dirX < 0f))
			{
				weapon.spawnOffset = transform.localRotation * -Vector3.right/2;
				weapon.directionMax = transform.localRotation * -Vector3.right;
				GetComponentInChildren<ProjectileTrigger>().Trigger();
			}
		}
		if (currentAction >= Action.Left)
		{
			if (Random.value >= chanceToRepeatAction)
				return currentAction;
		}
		// Prob should have weights here
		int numItems = (Action.Max + 1) - Action.Min;
		int selection = (int)Action.Min + (int)(numItems * AIWeighting.Evaluate(Random.value));
		return (Action)selection;
	}

	IEnumerator DeathSequence()
	{
		if (!dead)
		{
			dead = true;
			player.KilledEnemy ();
			globalAudio.PurpleDeath ();
			gameObject.GetComponent<Renderer> ().enabled = false;
			GameObject go = Instantiate (deathParticlePrefab, transform.position, Quaternion.identity);
			yield return new WaitForSecondsRealtime (1.5f);
			spawner.Died ();
			DestroyObject (go);
			DestroyObject (this.gameObject);
		}
	}

	public void Die()
	{
		StartCoroutine(DeathSequence());
	}

	void FixedUpdate()
	{
		BitmapCollision.LayerMask enemyMask = BitmapCollision.LayerMask.All & (~(BitmapCollision.LayerMask.Water | BitmapCollision.LayerMask.Enemy | BitmapCollision.LayerMask.EnemyIgnore | BitmapCollision.LayerMask.Player));
		bmpCol.DeleteBox(transform.position, colType);

		if (dead)
			return;
		
		nextDelay = Mathf.Clamp(nextDelay - Time.deltaTime, 0.0f, 100.0f);

		if (lethal)
		{
			if (bmpCol.IsBoxCollision(transform.position, BitmapCollision.LayerMask.Player))
			{
				player.KillPlayer();
			}
		}
		if (diesToWater)
		{
			if (bmpCol.IsBoxCollision (transform.position, BitmapCollision.LayerMask.Water))
			{
				Die ();
				return;
			}
		}

		if (position == transform.position && nextDelay == 0.0f)
		{
			currentAction = GetAction();

			switch (currentAction)
			{
				default:
				case Action.Left:
					LeftAnim();
					nextDelay = 0.0f;
					lookingLeft = true;
					transform.localRotation = Quaternion.AngleAxis(0, Vector3.up);
					if (!bmpCol.IsBoxCollision(transform.position + Vector3.left, enemyMask))
					{
						position += Vector3.left;
					}
					break;
				case Action.Right:
					RightAnim();
					nextDelay = 0.0f;
					lookingLeft = false;
					transform.localRotation = Quaternion.AngleAxis(180, Vector3.up);
					if (!bmpCol.IsBoxCollision(transform.position + Vector3.right, enemyMask))
					{
						position += Vector3.right;
					}
					break;
				case Action.Up:
					UpAnim();
					nextDelay = 0.0f;
					if (!bmpCol.IsBoxCollision(transform.position + Vector3.up, enemyMask))
					{
						position += Vector3.up;
					}
					break;
				case Action.Down:
					DownAnim();
					nextDelay = 0.0f;
					if (!bmpCol.IsBoxCollision(transform.position + Vector3.down, enemyMask))
					{
						position += Vector3.down;
					}
					break;
			}

		}

		transform.position = Vector3.MoveTowards(transform.position, position, Time.deltaTime * speed);

		bmpCol.AddBox(transform.position, colType);
	}
}
