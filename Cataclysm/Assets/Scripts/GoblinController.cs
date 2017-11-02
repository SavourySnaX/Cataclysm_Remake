using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinController : MonoBehaviour
{
	enum Action
	{
		Idle = 0,
		Sleep,
		Left,
		Right,
		Up,
		Down,

		Min = Idle,
		Max = Down
	}

	public float speed = 5.0f;
	public float sleepTimeMin = 3.0f;
	public float sleepTimeMax = 5.0f;
	public float idleTimeMin = 0.5f;
	public float idleIimeMax = 0.5f;
	public float chanceToRepeatAction = 0.10f;
	public bool lethal = false;
	public bool diesToWater=false;
	public BitmapCollision bmpCol;
	public AnimationCurve AIWeighting;
	public PlayerController player;
	public GameObject deathParticlePrefab;
	Vector3 position;
	Animator anim;
	Action currentAction;
	bool dead;

	float nextDelay;

	void Start()
	{
		dead = false;
		anim = GetComponent<Animator>();
		position = transform.position;
		currentAction = Action.Idle;
		nextDelay = 0.0f;
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

	void IdleAnim()
	{
		anim.SetBool("left", false);
		anim.SetBool("right", false);
		anim.SetBool("up", false);
		anim.SetBool("down", false);
		anim.SetBool("sleep", false);
	}

	void SleepAnim()
	{
		anim.SetBool("left", false);
		anim.SetBool("right", false);
		anim.SetBool("up", false);
		anim.SetBool("down", false);
		anim.SetBool("sleep", true);
	}

	void LeftAnim()
	{
		anim.SetBool("left", true);
		anim.SetBool("right", false);
		anim.SetBool("up", false);
		anim.SetBool("down", false);
		anim.SetBool("sleep", false);
	}

	void RightAnim()
	{
		anim.SetBool("left", false);
		anim.SetBool("right", true);
		anim.SetBool("up", false);
		anim.SetBool("down", false);
		anim.SetBool("sleep", false);
	}

	void DownAnim()
	{
		anim.SetBool("left", false);
		anim.SetBool("right", false);
		anim.SetBool("up", false);
		anim.SetBool("down", true);
		anim.SetBool("sleep", false);
	}

	void UpAnim()
	{
		anim.SetBool("left", false);
		anim.SetBool("right", false);
		anim.SetBool("up", true);
		anim.SetBool("down", false);
		anim.SetBool("sleep", false);
	}

	IEnumerator DeathSequence()
	{
		if (!dead)
		{
			dead = true;
			DestroyObject (this.gameObject);
			gameObject.GetComponent<Renderer>().enabled = false;
			Instantiate(deathParticlePrefab, transform.position, Quaternion.identity);
			yield return new WaitForSecondsRealtime(1.5f);
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
				case Action.Idle:
					nextDelay = Random.Range(idleTimeMin, idleIimeMax);
					IdleAnim();
					break;
				case Action.Sleep:
					nextDelay = Random.Range(sleepTimeMin, sleepTimeMax);
					SleepAnim();
					break;
				case Action.Left:
					nextDelay = 0.0f;
					LeftAnim();
					transform.localRotation = Quaternion.AngleAxis(0, Vector3.up);
					if (!bmpCol.IsBoxCollision(transform.position + Vector3.left, enemyMask))
					{
						position += Vector3.left;
					}
					break;
				case Action.Right:
					nextDelay = 0.0f;
					RightAnim();
					transform.localRotation = Quaternion.AngleAxis(180, Vector3.up);
					if (!bmpCol.IsBoxCollision(transform.position + Vector3.right, enemyMask))
					{
						position += Vector3.right;
					}
					break;
				case Action.Up:
					nextDelay = 0.0f;
					UpAnim();
					if (!bmpCol.IsBoxCollision(transform.position + Vector3.up, enemyMask))
					{
						position += Vector3.up;
					}
					break;
				case Action.Down:
					nextDelay = 0.0f;
					DownAnim();
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
