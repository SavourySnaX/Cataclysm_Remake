using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDrop : MonoBehaviour {

	public BitmapCollision bmpCol;
	SpriteRenderer sr;

	Color fast;
	Color medium;
	Color normal;

	// Use this for initialization
	void Start () 
	{
		sr = GetComponent<SpriteRenderer> ();

		normal = new Color (1.0f, 1.0f, 1.0f, 0.75f);
		medium = new Color (1.0f, 1.0f, 1.0f, 0.825f);
		fast = new Color (1.0f, 1.0f, 1.0f, 1.0f);
	}

	// Update is called once per frame
	void FixedUpdate ()
	{
		bool col = bmpCol.IsCollision (transform.position + Vector3.down * (1 / 12.0f));
		if (!col)
		{
			bmpCol.RemovePixel (transform.position);
			transform.position += Vector3.down * (1 / 12.0f);
			bmpCol.AddPixel (transform.position, Color.blue);
			sr.color = fast;
		} 
		else
		{
			if (Color.magenta == bmpCol.GetCollisionColor (transform.position + Vector3.down * (1 / 12.0f)))
			{
				bmpCol.RemovePixel (transform.position);
				DestroyObject (gameObject);
				return;
			}
			var direction = Vector3.right;
			if (Random.Range (-10.0f, 10.0f) <= 0)
			{
				direction = Vector3.left;
			}
			bool ccol = bmpCol.IsCollision (transform.position + direction * (1 / 3.0f));
			if (!ccol)
			{
				bmpCol.RemovePixel (transform.position);
				transform.position += direction * (1 / 3.0f);
				bmpCol.AddPixel (transform.position, Color.blue);
				sr.color = medium;
			} 
			else
			{
				sr.color = normal;
			}
		}

	}

}
