using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BitmapCollision : MonoBehaviour 
{
	public Texture2D texture;
	Tilemap tm;

	// Use this for initialization
	void Start () 
	{
		tm = GetComponent<Tilemap>();
		texture = new Texture2D (tm.cellBounds.size.x * 12, tm.cellBounds.size.y * 12);
		for (int x = 0; x < tm.cellBounds.size.x * 12; x++) 
		{
			for (int y = 0; y < tm.cellBounds.size.y * 12; y++) 
			{
				var vec = tm.CellToWorld (new Vector3Int (x/12, y/12,0))+tm.origin;
				vec += new Vector3 ((x % 12)/12.0f + (0.5f/12.0f), (y % 12)/12.0f + (0.5f/12.0f), 0);
				var res = Physics2D.Raycast (vec, Vector2.zero);
				if (res.collider == null) {
					texture.SetPixel (x, y, Color.black);
				} else
					texture.SetPixel (x, y, Color.green);
			}
		}
	}

	Vector3Int GetPixelCoord(Vector3 worldPos)
	{
		Vector3 localPos = tm.WorldToLocal (worldPos) - tm.origin;
		Vector3Int cellPos = tm.LocalToCell (localPos);
		Vector3 cellDiff = localPos - cellPos;
		cellPos *= 12;
		cellPos += new Vector3Int((int)(cellDiff.x * 12),(int)(cellDiff.y*12),0);
		return cellPos;
	}

	public bool IsCollision(Vector3 worldPos)
	{
		Vector3Int cellPos = GetPixelCoord (worldPos);
		return texture.GetPixel (cellPos.x, cellPos.y) != Color.black;
	}

	public void RemovePixel(Vector3 worldPos)
	{
		Vector3Int cellPos = GetPixelCoord (worldPos);
		texture.SetPixel (cellPos.x, cellPos.y, Color.black);
	}

	public void AddPixel(Vector3 worldPos, Color col)
	{
		Vector3Int cellPos = GetPixelCoord (worldPos);
		texture.SetPixel (cellPos.x, cellPos.y, col);
	}

	// Update is called once per frame
	void Update () 
	{
		
	}
}
