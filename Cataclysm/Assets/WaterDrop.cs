using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDrop : MonoBehaviour {

	BoxCollider2D bc;
	int layerMaskForMovement;
	public BitmapCollision bmpCol;

	// Use this for initialization
	void Start () 
	{
		bc = GetComponent<BoxCollider2D> ();
		layerMaskForMovement = LayerMask.GetMask ("Background", "Interaction", "Water");
	}

	// Update is called once per frame
	void FixedUpdate ()
	{
		bool col = bmpCol.IsCollision (bc.bounds.center + Vector3.down * (1 / 12.0f));
//		var ray = Physics2D.BoxCast(bc.bounds.center + Vector3.down*(1/12.0f),bc.size,0.0f, Vector2.zero, layerMaskForMovement);
//		if (ray.collider == null)
		if (!col)
		{
			bmpCol.RemovePixel (bc.bounds.center);
			transform.position += Vector3.down * (1 / 12.0f);
			bmpCol.AddPixel (bc.bounds.center, Color.blue);
		} 
		else
		{
			var direction = Vector3.right;
			if (Random.Range (-10.0f, 10.0f) <= 0)
			{
				direction = Vector3.left;
			}
			bool ccol = bmpCol.IsCollision (bc.bounds.center + direction * (1 / 3.0f));
			//var lray = Physics2D.BoxCast(bc.bounds.center + direction*(1/3.0f), bc.size, 0.0f, Vector2.zero, layerMaskForMovement);
			//if (lray.collider == null)
			if (!ccol)
			{
				bmpCol.RemovePixel (bc.bounds.center);
				transform.position += direction * (1 / 3.0f);
				bmpCol.AddPixel (bc.bounds.center, Color.blue);
			}
		}

	}

}
