using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float speed=1.0f;
	public int totalBlocks=8;
	public BitmapCollision bmpCol;
	public HudBehaviour hud;
	public GameObject boxPrefab;
    Vector3 position;
	List<GameObject> blocksList;

	void Start ()
    {
		blocksList = new List<GameObject> ();
        position = transform.position;
	}

	void AddBlock(Vector3 position)
	{
		if (totalBlocks != 0)
		{
			bmpCol.AddBox (transform.position, BitmapCollision.LayerMask.Block);
			blocksList.Add (Instantiate (boxPrefab, transform.position, Quaternion.identity));
			totalBlocks--;
			hud.SetBlocks (totalBlocks);
		}
	}

	void RemoveBlock(Vector3 position)
	{
		GameObject toRemove=null;
		for (int a = 0; a < blocksList.Count; a++)
		{
			if (blocksList [a].transform.position == position)
			{
				toRemove = blocksList[a];
				break;
			}
		}
		if (toRemove != null)
		{
			bmpCol.DeleteBox (toRemove.transform.position, BitmapCollision.LayerMask.Block);
			blocksList.Remove (toRemove);
			DestroyObject (toRemove);
			totalBlocks++;
			hud.SetBlocks (totalBlocks);
		}
	}

	bool HandleDynamics(BitmapCollision.LayerMask colMask,Vector3 newPos)
	{
		if ((colMask & BitmapCollision.LayerMask.Plug)==BitmapCollision.LayerMask.Plug)
		{
			bmpCol.DeleteTile (bmpCol.mainTilemap,newPos,BitmapCollision.LayerMask.Plug);
			return true;
		}
		if ((colMask & BitmapCollision.LayerMask.Block) == BitmapCollision.LayerMask.Block)
		{
			RemoveBlock (newPos);
			return true;
		}
		return false;
	}

	void FixedUpdate ()
    {
		BitmapCollision.LayerMask playerMask = BitmapCollision.LayerMask.All & (~(BitmapCollision.LayerMask.Water | BitmapCollision.LayerMask.Player | BitmapCollision.LayerMask.PlayerIgnore));
		bmpCol.DeleteBox(transform.position,BitmapCollision.LayerMask.Player);
		if (Input.GetKey(KeyCode.Space) && position==transform.position)
		{
			if (!bmpCol.IsBoxCollision (transform.position,BitmapCollision.LayerMask.Block|BitmapCollision.LayerMask.Background))
			{
				AddBlock (transform.position);
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
