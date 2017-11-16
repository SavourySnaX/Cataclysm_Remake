using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearProjectileMove : MonoBehaviour
{
	public BitmapCollision bmpCol;
	public PlayerController player;
	public Vector3 direction;
	public Vector3 drag;
	public float speed;
	public BitmapCollision.LayerMask colMask = BitmapCollision.LayerMask.All;
	public BitmapCollision.LayerMask colType = BitmapCollision.LayerMask.None;

	public bool playerSeeking = false;
	public float playerDistance = 0f;
	public float followScale = 0f;

	Vector3 lastPosition;
	EnemySpawner spawner;
	void Start()
	{
		lastPosition = transform.position;
		spawner = bmpCol.GetComponentInChildren<EnemySpawner> ();
	}

	void FixedUpdate()
	{
		BitmapCollision.LayerMask mask = BitmapCollision.LayerMask.None;

		lastPosition = transform.position;

		if (playerSeeking)
		{
			float distance = Vector3.Distance(player.transform.position, transform.position);
			if (distance<playerDistance)
			{
				drag = Vector3.Normalize(player.transform.position - transform.position) * followScale * (playerDistance/distance);
			}
		}

		direction += drag * Time.deltaTime;
		transform.position += direction * speed * Time.deltaTime;

		mask = bmpCol.SweepCollisionMask (lastPosition, transform.position, colMask);
		if (mask!=BitmapCollision.LayerMask.None)
		{
			if ((mask & BitmapCollision.LayerMask.Player) == BitmapCollision.LayerMask.Player)
			{
				player.KillPlayer ();
			}
			if (((colType & BitmapCollision.LayerMask.Bullet) != BitmapCollision.LayerMask.None) && ((mask & BitmapCollision.LayerMask.Enemy) != BitmapCollision.LayerMask.None))
			{
				spawner.KillMob (mask & BitmapCollision.LayerMask.Enemy);
			}
			DestroyObject(this.gameObject);
			return;
		}
	}
}
