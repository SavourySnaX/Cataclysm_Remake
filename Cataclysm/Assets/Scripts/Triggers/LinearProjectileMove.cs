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

		//bmpCol.RemoveSweep(lastPosition, transform.position, colType);
		//bmpCol.RemovePixel(lastPosition, colType);

		lastPosition = transform.position;
		direction += drag * Time.deltaTime;
		transform.position += direction * speed * Time.deltaTime;

		//mask = bmpCol.GetCollisionMask (transform.position, colMask);
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

		//bmpCol.AddSweep(lastPosition, transform.position, colType);
		//bmpCol.AddPixel(transform.position,colType);
	}
}
