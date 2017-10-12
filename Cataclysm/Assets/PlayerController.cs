using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed=1.0f;
    Vector3 position;
    BoxCollider2D bc;
	SpriteRenderer sr;
	int layerMaskForMovement;

	// Use this for initialization
	void Start ()
    {
        position = transform.position;
        bc = GetComponent<BoxCollider2D>();
		sr = GetComponent<SpriteRenderer> ();
		layerMaskForMovement = LayerMask.GetMask ("Background");
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {

        if (Input.GetKey(KeyCode.Z) && position == transform.position)
        {
			sr.flipX = true;
			var ray = Physics2D.BoxCast(bc.bounds.center, bc.size, 0.0f, Vector2.left, 1.0f, layerMaskForMovement);
            if (ray.collider==null)
            {
                position += Vector3.left;
            }
        }
        if (Input.GetKey(KeyCode.X) && position == transform.position)
        {
			sr.flipX = false;
			var ray = Physics2D.BoxCast(bc.bounds.center, bc.size, 0.0f, Vector2.right, 1.0f, layerMaskForMovement);
			if (ray.collider == null) 
			{
				position += Vector3.right;
			}
        }
        if (Input.GetKey(KeyCode.P) && position == transform.position)
        {
			var ray = Physics2D.BoxCast(bc.bounds.center, bc.size, 0.0f, Vector2.up, 1.0f, layerMaskForMovement);
			if (ray.collider == null) 
			{
				position += Vector3.up;
			}
        }
        if (Input.GetKey(KeyCode.L) && position == transform.position)
        {
			var ray = Physics2D.BoxCast(bc.bounds.center, bc.size, 0.0f, Vector2.down, 1.0f, layerMaskForMovement);
			if (ray.collider == null)
			{
				position += Vector3.down;
			}
        }

        transform.position = Vector3.MoveTowards(transform.position, position, Time.deltaTime * speed);
	}
}
