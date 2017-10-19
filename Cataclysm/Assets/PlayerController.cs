using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public float speed=1.0f;
	public BitmapCollision bmpCol;
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

	void HandleDynamics(Collider2D collider,Vector3 newPos)
	{
		var go = collider.gameObject;
		var tm = go.GetComponent<Tilemap> ();
		Vector3 localPos = tm.WorldToLocal (newPos);
		Vector3Int cellPos = tm.LocalToCell (localPos);
		Sprite s = tm.GetSprite (cellPos);
		if (s != null)
		{
			if (s.name == "Plug_A" || s.name == "Plug_B")
			{
				bmpCol.DeleteTile (tm,newPos);
			}
		}
	}

	// Update is called once per frame
	void FixedUpdate ()
    {

        if (Input.GetKey(KeyCode.Z) && position == transform.position)
        {
			transform.localRotation = Quaternion.AngleAxis (180, Vector3.up);
//			transform.localScale=new Vector3 (-transform.localScale.x, transform.localScale.y);
			var ray = Physics2D.BoxCast(bc.bounds.center, bc.size, 0.0f, Vector2.left, 1.0f, layerMaskForMovement);
			if (ray.collider == null)
			{
				position += Vector3.left;
			} 
			else
				HandleDynamics (ray.collider,bc.bounds.center+Vector3.left);
        }
        if (Input.GetKey(KeyCode.X) && position == transform.position)
        {
			transform.localRotation = Quaternion.AngleAxis (0, Vector3.up);
//			transform.localScale = new Vector2 (transform.localScale.x, transform.localScale.y);
			var ray = Physics2D.BoxCast(bc.bounds.center, bc.size, 0.0f, Vector2.right, 1.0f, layerMaskForMovement);
			if (ray.collider == null) 
			{
				position += Vector3.right;
			}
			else
				HandleDynamics (ray.collider,bc.bounds.center+Vector3.right);
        }
        if (Input.GetKey(KeyCode.P) && position == transform.position)
        {
			var ray = Physics2D.BoxCast(bc.bounds.center, bc.size, 0.0f, Vector2.up, 1.0f, layerMaskForMovement);
			if (ray.collider == null) 
			{
				position += Vector3.up;
			}
			else
				HandleDynamics (ray.collider,bc.bounds.center+Vector3.up);
        }
        if (Input.GetKey(KeyCode.L) && position == transform.position)
        {
			var ray = Physics2D.BoxCast(bc.bounds.center, bc.size, 0.0f, Vector2.down, 1.0f, layerMaskForMovement);
			if (ray.collider == null)
			{
				position += Vector3.down;
			}
			else
				HandleDynamics (ray.collider,bc.bounds.center+Vector3.down);
        }

        transform.position = Vector3.MoveTowards(transform.position, position, Time.deltaTime * speed);
	}
}
