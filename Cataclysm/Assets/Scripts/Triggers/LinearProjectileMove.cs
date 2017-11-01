using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearProjectileMove : MonoBehaviour
{
	public BitmapCollision bmpCol;
	public Vector3 direction;
	public float speed;

	void Start()
	{

	}

	void FixedUpdate()
	{
		BitmapCollision.LayerMask colMask = BitmapCollision.LayerMask.All;

		if (bmpCol.IsCollision(transform.position, colMask))
		{
			// Just delete for now
			DestroyObject(this.gameObject);
			return;
		}

		transform.position += direction * speed * Time.deltaTime;
	}
}
