using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float speed=1.0f;
	public BitmapCollision bmpCol;
	public GameObject boxPrefab;
    Vector3 position;
    BoxCollider2D bc;
	int layerMaskForMovement;

	// Use this for initialization
	void Start ()
    {
        position = transform.position;
        bc = GetComponent<BoxCollider2D>();
		layerMaskForMovement = LayerMask.GetMask ("Background");
	}

	bool HandleDynamics(BitmapCollision.LayerMask colMask,Vector3 newPos)
	{
		if ((colMask & BitmapCollision.LayerMask.Plug)==BitmapCollision.LayerMask.Plug)
		{
			bmpCol.DeleteTile (bmpCol.mainTilemap,newPos,BitmapCollision.LayerMask.Plug);
			return true;
		}
		return false;
	}

	// Update is called once per frame
	void FixedUpdate ()
    {
		BitmapCollision.LayerMask playerMask = BitmapCollision.LayerMask.All & (~(BitmapCollision.LayerMask.Water | BitmapCollision.LayerMask.Player | BitmapCollision.LayerMask.Block | BitmapCollision.LayerMask.PlayerIgnore));
		bmpCol.DeleteBox(transform.position,BitmapCollision.LayerMask.Player);
		if (Input.GetKeyDown (KeyCode.Space) && position==transform.position)
		{
			if (!bmpCol.IsBoxCollision (transform.position,BitmapCollision.LayerMask.Block|BitmapCollision.LayerMask.Background))
			{
				bmpCol.AddBox (transform.position, BitmapCollision.LayerMask.Block);
				Instantiate (boxPrefab, transform.position, Quaternion.identity);
			}
		}	
		if (Input.GetKeyDown (KeyCode.Escape))
		{
			SceneManager.LoadScene ("mainmenu");
			return;
		}
        if (Input.GetKey(KeyCode.Z) && position == transform.position)
        {
			transform.localRotation = Quaternion.AngleAxis (180, Vector3.up);
			BitmapCollision.LayerMask colMask = bmpCol.GetCollisionMask (transform.position + Vector3.left);
			if (!bmpCol.IsBoxCollision (transform.position + Vector3.left, playerMask) || HandleDynamics (colMask, transform.position + Vector3.left))
			{
				position += Vector3.left;
			} 
        }
        if (Input.GetKey(KeyCode.X) && position == transform.position)
        {
			transform.localRotation = Quaternion.AngleAxis (0, Vector3.up);
			BitmapCollision.LayerMask colMask = bmpCol.GetCollisionMask (transform.position + Vector3.right);
			if (!bmpCol.IsBoxCollision (transform.position + Vector3.right, playerMask) || HandleDynamics (colMask, transform.position + Vector3.right))
			{
				position += Vector3.right;
			}
        }
        if (Input.GetKey(KeyCode.P) && position == transform.position)
        {
			BitmapCollision.LayerMask colMask = bmpCol.GetCollisionMask (transform.position + Vector3.up);
			if (!bmpCol.IsBoxCollision (transform.position + Vector3.up, playerMask) || HandleDynamics (colMask, transform.position + Vector3.up))
			{
				position += Vector3.up;
			}
        }
        if (Input.GetKey(KeyCode.L) && position == transform.position)
        {
			BitmapCollision.LayerMask colMask = bmpCol.GetCollisionMask (transform.position + Vector3.down);
			if (!bmpCol.IsBoxCollision (transform.position + Vector3.down, playerMask) || HandleDynamics (colMask, transform.position + Vector3.down))
			{
				position += Vector3.down;
			}
        }

        transform.position = Vector3.MoveTowards(transform.position, position, Time.deltaTime * speed);

		bmpCol.AddBox (transform.position, BitmapCollision.LayerMask.Player);
	}
}
