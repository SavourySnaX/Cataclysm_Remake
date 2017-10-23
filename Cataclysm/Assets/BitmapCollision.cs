using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BitmapCollision : MonoBehaviour 
{
	readonly int sizeX = 3;
	readonly int sizeY = 12;

	public Texture2D collision;
	public Texture2D texture;
	public Bounds mainTilemapBounds;
	Tilemap mainTilemap;

	// We currently assume mainTilemap is larger or equal to the others.. todo fix (easy to test)
	void ComputeCollisionBitmap(Tilemap tilemap)
	{
		// Take care of maps that are a different offset than the mainTilemap
		int ofx=tilemap.origin.x-mainTilemap.origin.x;
		int ofy=tilemap.origin.y-mainTilemap.origin.y;

		for (int x = tilemap.cellBounds.min.x; x < tilemap.cellBounds.max.x; x++) 
		{
			for (int y = tilemap.cellBounds.min.y; y < tilemap.cellBounds.max.y; y++) 
			{
				Sprite s = tilemap.GetSprite (new Vector3Int (x, y, 0));
				for (int xx = 0; xx < sizeX; xx++)
				{
					for (int yy = 0; yy < sizeY; yy++)
					{
						int fx = (ofx + (x - tilemap.origin.x)) * sizeX;
						int fy = (ofy + (y - tilemap.origin.y)) * sizeY;

						if (s == null)
						{
							if (tilemap == mainTilemap)
							{
								texture.SetPixel (fx + xx, fy + yy, Color.black);
							}
						}
						else
						{
							float sx = s.textureRect.min.x + xx * (((float)s.textureRect.width) / ((float)sizeX)) + 0.5f*(((float)s.textureRect.width) / ((float)sizeX));
							float sy = s.textureRect.min.y + yy * (((float)s.textureRect.height) / ((float)sizeY)) + 0.5f*(((float)s.textureRect.height) / ((float)sizeY));

							Color col = collision.GetPixel ((int)sx, (int)sy);

							if (col == Color.black)
							{
								texture.SetPixel (fx + xx, fy + yy, Color.green);
							} 
							else if (tilemap==mainTilemap)
							{
								texture.SetPixel (fx + xx, fy + yy, Color.black);
							}
						}
					}
				}
			}
		}
	}

	void ComputeCollisionBitmaps()
	{
		var components = transform.parent.gameObject.GetComponentsInChildren<Tilemap> ();

		foreach (var tilemap in components)
		{
			ComputeCollisionBitmap (tilemap);
		}
	}

	// Use this for initialization
	void Start () 
	{
		mainTilemap = GetComponent<Tilemap>();
		mainTilemapBounds = mainTilemap.localBounds;
		texture = new Texture2D (mainTilemap.cellBounds.size.x * sizeX, mainTilemap.cellBounds.size.y * sizeY);

		ComputeCollisionBitmaps ();
	}

	Vector3Int GetPixelCoord(Vector3 worldPos)
	{
		Vector3 localPos = mainTilemap.WorldToLocal (worldPos) - mainTilemap.origin;
		Vector3Int cellPos = mainTilemap.LocalToCell (localPos);
		Vector3 cellDiff = localPos - cellPos;
		cellPos *= new Vector3Int(sizeX, sizeY, 1);
		cellPos += new Vector3Int ((int)(cellDiff.x * sizeX), (int)(cellDiff.y * sizeY), 0);
		return cellPos;
	}

	public bool IsCollision(Vector3 worldPos)
	{
		Vector3Int cellPos = GetPixelCoord (worldPos);
		return texture.GetPixel (cellPos.x, cellPos.y) != Color.black;
	}

	public void DeleteTile(Tilemap tm2, Vector3 wPos)
	{
		Vector3 lPos = mainTilemap.WorldToLocal (wPos);
		Vector3Int cellPos = mainTilemap.LocalToCell (lPos);

		tm2.SetTile (tm2.WorldToCell(wPos), null);
		cellPos -= mainTilemap.origin;
		cellPos *= new Vector3Int (sizeX, sizeY, 1);
		for (int x = cellPos.x; x < cellPos.x + sizeX; x++)
		{
			for (int y = cellPos.y; y < cellPos.y + sizeY; y++)
			{
				texture.SetPixel (x, y, Color.black);
			}
		}
	}

	public void MoveTile1PixelDown(Tilemap tm2, Vector3 wPos)
	{
		Tile t = tm2.GetTile<Tile> (tm2.WorldToCell (wPos));
		Matrix4x4 mat = tm2.GetTransformMatrix(tm2.WorldToCell(wPos));
		Vector4 v1 = mat.GetRow (1);
		v1.w = v1.w - 1 / 12.0f;
		mat.SetRow (1, v1);
		tm2.SetTransformMatrix (tm2.WorldToCell (wPos), mat);
	}

	public void MoveTile12PixelsUp(Tilemap tm2, Vector3 wPos)
	{
		Tile t = tm2.GetTile<Tile> (tm2.WorldToCell (wPos));
		Matrix4x4 mat = tm2.GetTransformMatrix(tm2.WorldToCell(wPos));
		Vector4 v1 = mat.GetRow (1);
		v1.w = v1.w + 12 / 12.0f;
		mat.SetRow (1, v1);
		tm2.SetTransformMatrix (tm2.WorldToCell (wPos), mat);
	}

	public bool HandleCollapse(Tilemap tm2, Vector3 wPos)
	{
		Vector3 lPos = mainTilemap.WorldToLocal (wPos);
		Vector3Int cellPos = mainTilemap.LocalToCell (lPos);
		Vector3Int origPos = tm2.WorldToCell(wPos);

		// Get Cell above this one
		cellPos += new Vector3Int(0,1,0);
		cellPos -= mainTilemap.origin;
		cellPos *= new Vector3Int (sizeX, sizeY, 1);
		int cnt = 0;
		for (int x = cellPos.x; x < cellPos.x + sizeX; x++)
		{
			if (texture.GetPixel (x, cellPos.y) == Color.blue)
			{
				cnt++;
			}
		}

		if (cnt > 2)
		{
			DeleteTile (tm2, wPos);
			return true;
		}
		return false;
	}

	public bool HandlePressure(Tilemap tm2, ref Interactions.PressureData pd)
	{
		if (pd.totalSize > 0)
		{
			Vector3 wPos = pd.position;
			Vector3 lPos = mainTilemap.WorldToLocal (wPos);
			Vector3Int cellPos = mainTilemap.LocalToCell (lPos);
			Vector3Int origPos = tm2.WorldToCell (wPos);

			// Get Cell above this one
			cellPos -= mainTilemap.origin;
			cellPos *= new Vector3Int (sizeX, sizeY, 1);
			cellPos += new Vector3Int (0, pd.deltaCollapse, 0);
			int cnt = 0;
			for (int x = cellPos.x; x < cellPos.x + sizeX; x++)
			{
				for (int y = cellPos.y; y < cellPos.y + sizeY; y++)
				{
					if (texture.GetPixel (x, y) == Color.blue)
					{
						cnt++;
					}
				}
			}
			if (cnt == sizeX * sizeY)
			{
				Vector3 nPos = wPos;
				for (int a = 0; a < pd.totalSize; a++)
				{
					MoveTile1PixelDown (tm2, nPos);
					nPos += new Vector3 (0, -1, 0);
				}
				cellPos -= new Vector3Int (0, sizeY, 0);
				for (int x = cellPos.x; x < cellPos.x + sizeX; x++)
				{
					texture.SetPixel (x, cellPos.y + 11, Color.black);
				}
				pd.deltaCollapse--;
				if (pd.deltaCollapse == 0)
				{
					var origTile = tm2.GetTile<Tile> (tm2.WorldToCell(pd.position));
					tm2.SetTile (tm2.WorldToCell(pd.position), null);
					pd.deltaCollapse = 12;
					pd.totalSize--;
					pd.position += new Vector3 (0, -1, 0);
					tm2.SetTile(tm2.WorldToCell(pd.position), origTile);
					nPos = pd.position;
					for (int a = 0; a < pd.totalSize; a++)
					{
						MoveTile12PixelsUp (tm2, nPos);
						nPos += new Vector3 (0, -1, 0);
					}
				}
			}
		}
		return false;
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
