using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeliController : MonoBehaviour, IEnemyBase
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
	public bool lethal = false;
	public bool diesToWater=false;
	public BitmapCollision bmpCol;
	public AnimationCurve AIWeighting;
	public PlayerController player;
	public GameObject deathParticlePrefab;
	public EnemySpawner spawner;
	Vector3 position;
	Action currentAction;
	bool dead;

	GlobalAudioManager globalAudio;

	float nextDelay;

	public void Init (BitmapCollision _bmpCol, PlayerController _player, EnemySpawner _spawn)
	{
		bmpCol = _bmpCol;
		player = _player;
		spawner = _spawn;
	}

	void Start()
	{
		dead = false;
		position = transform.position;
		currentAction = Action.Left;
		nextDelay = 0.0f;
		globalAudio = GameObject.Find("GlobalAudio").GetComponent<GlobalAudioManager> ();
	}

	Action GetAction()
	{
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
			player.KilledEnemy ();
			globalAudio.PurpleDeath ();
			dead = true;
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
		if (dead)
			return;
		BitmapCollision.LayerMask enemyMask = BitmapCollision.LayerMask.All & (~(BitmapCollision.LayerMask.Water | BitmapCollision.LayerMask.Enemy | BitmapCollision.LayerMask.EnemyIgnore | BitmapCollision.LayerMask.Player));
		bmpCol.DeleteBox(transform.position, BitmapCollision.LayerMask.Enemy);

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
					nextDelay = 0.0f;
					transform.localRotation = Quaternion.AngleAxis(0, Vector3.up);
					if (!bmpCol.IsBoxCollision(transform.position + Vector3.left, enemyMask))
					{
						position += Vector3.left;
					}
					break;
				case Action.Right:
					nextDelay = 0.0f;
					transform.localRotation = Quaternion.AngleAxis(180, Vector3.up);
					if (!bmpCol.IsBoxCollision(transform.position + Vector3.right, enemyMask))
					{
						position += Vector3.right;
					}
					break;
				case Action.Up:
					nextDelay = 0.0f;
					if (!bmpCol.IsBoxCollision(transform.position + Vector3.up, enemyMask))
					{
						position += Vector3.up;
					}
					break;
				case Action.Down:
					nextDelay = 0.0f;
					if (!bmpCol.IsBoxCollision(transform.position + Vector3.down, enemyMask))
					{
						position += Vector3.down;
					}
					break;
			}

		}

		transform.position = Vector3.MoveTowards(transform.position, position, Time.deltaTime * speed);

		bmpCol.AddBox(transform.position, BitmapCollision.LayerMask.Enemy);
	}
}
