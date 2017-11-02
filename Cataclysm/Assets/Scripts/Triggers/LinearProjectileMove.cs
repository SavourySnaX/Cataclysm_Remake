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

	void Start()
	{
	}

	void FixedUpdate()
	{
		BitmapCollision.LayerMask colMask = BitmapCollision.LayerMask.All;

		if (bmpCol.IsCollision(transform.position, colMask))
		{
			BitmapCollision.LayerMask mask = bmpCol.GetCollisionMask (transform.position);
			if ((mask & BitmapCollision.LayerMask.Player) == BitmapCollision.LayerMask.Player)
			{
				player.KillPlayer ();
			}
			DestroyObject(this.gameObject);
			return;
		}

		direction += drag * Time.deltaTime;
		transform.position += direction * speed * Time.deltaTime;
	}
}
