using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BitmapCollision : MonoBehaviour
{
	[System.Flags]
	public enum LayerMask
	{
		None = 0,
		Background = 1<<0,
		Water = 1<<1,
		Drain = 1<<2,
		Player = 1<<3,
		Block = 1<<4,
		FailDrain = 1<<5,
		Plug = 1<<6,
		EnemyIgnore = 1<<7,
		PlayerIgnore = 1<<8,
		Bullet = 1<<9,
		DynamicBlock = 1<<10,
		Trigger1 = 1<<11,
		Trigger2 = 1<<12,
		Trigger3 = 1<<13,
		Trigger4 = 1<<14,
		Trigger5 = 1<<15,
		Trigger6 = 1<<16,
		Trigger7 = 1<<17,
		Trigger8 = 1<<18,
		Trigger9 = 1<<19,
		TriggerA = 1<<20,
		TriggerB = 1<<21,
		TriggerC = 1<<22,
		TriggerD = 1<<23,
		TriggerE = 1<<24,
		TriggerF = 1<<25,
		TriggerG = 1<<26,
		Enemy1 = 1<<27,
		Enemy2 = 1<<28,
		Enemy3 = 1<<29,
		Enemy4 = 1<<30,

		Enemy = LayerMask.Enemy1 | LayerMask.Enemy2 | LayerMask.Enemy3 | LayerMask.Enemy4,
		All = LayerMask.Background | LayerMask.Water | LayerMask.Drain | LayerMask.Player | LayerMask.Block | LayerMask.FailDrain | LayerMask.Plug | LayerMask.PlayerIgnore | LayerMask.EnemyIgnore | LayerMask.Enemy | LayerMask.DynamicBlock,
		Triggers = LayerMask.Trigger1 | LayerMask.Trigger2 | LayerMask.Trigger3 | LayerMask.Trigger4 | LayerMask.Trigger5 | LayerMask.Trigger6 | LayerMask.Trigger7 | LayerMask.Trigger8 | LayerMask.Trigger9 | LayerMask.TriggerA | LayerMask.TriggerB | LayerMask.TriggerC | LayerMask.TriggerD | LayerMask.TriggerE | LayerMask.TriggerF | LayerMask.TriggerG
	}

	public readonly int sizeX = 3;
	public readonly int sizeY = 12;

	public Texture2D collision;
	LayerMask[,] collisionMap;
	public Bounds mainTilemapBounds;
	public Tilemap mainTilemap;

	public GameObject[] triggerObjects;

	// TODO - move these and the material setters to a different script
	public Color backgroundMinColour;
	public Color backgroundMaxColour;
	public Color goalMinColour = new Color(43 / 255.0f, 237 / 255.0f, 246 / 255.0f);
	public Color goalMaxColour = new Color(57 / 255.0f, 64 / 255.0f, 203 / 255.0f);

	float pressureAccumulationFactor = 0.8f;

	// We currently assume mainTilemap is larger or equal to the others.. todo fix (easy to test)
	void ComputeCollisionBitmap(Tilemap tilemap)
	{
		// Take care of maps that are a different offset than the mainTilemap
		int ofx = tilemap.origin.x - mainTilemap.origin.x;
		int ofy = tilemap.origin.y - mainTilemap.origin.y;

		for (int x = tilemap.cellBounds.min.x; x < tilemap.cellBounds.max.x; x++)
		{
			for (int y = tilemap.cellBounds.min.y; y < tilemap.cellBounds.max.y; y++)
			{
				Sprite s = tilemap.GetSprite(new Vector3Int(x, y, 0));
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
								collisionMap[fx + xx, fy + yy] = LayerMask.None;
							}
						}
						else
						{
							float sx = s.textureRect.min.x + xx * (((float)s.textureRect.width) / ((float)sizeX)) + 0.5f * (((float)s.textureRect.width) / ((float)sizeX));
							float sy = s.textureRect.min.y + yy * (((float)s.textureRect.height) / ((float)sizeY)) + 0.5f * (((float)s.textureRect.height) / ((float)sizeY));

							Color col = collision.GetPixel((int)sx, (int)sy);

							if (col == Color.black)
							{
								collisionMap[fx + xx, fy + yy] = LayerMask.Background;
							}
							else if (col == Color.magenta)
							{
								collisionMap[fx + xx, fy + yy] |= LayerMask.Drain;
							}
							else if (col == Color.cyan)
							{
								collisionMap[fx + xx, fy + yy] |= LayerMask.FailDrain;
							}
							else if (col == Color.green)
							{
								collisionMap[fx + xx, fy + yy] |= LayerMask.Plug;
							}
							else if (col == Color.red)
							{
								collisionMap[fx + xx, fy + yy] |= LayerMask.EnemyIgnore;
							}
							else if (col == Color.blue)
							{
								collisionMap[fx + xx, fy + yy] |= LayerMask.PlayerIgnore;
							}
							else if (col == new Color(1,1,0))
							{
								collisionMap[fx + xx, fy + yy] |= LayerMask.Block;
							}
							else if (tilemap == mainTilemap)
							{
								collisionMap[fx + xx, fy + yy] = LayerMask.None;
							}
						}
					}
				}
			}
		}
	}

	public void TriggerAction(LayerMask triggers)
	{
		int idx = 0;
		LayerMask check = LayerMask.Trigger1;
		while ((check & LayerMask.Triggers) != LayerMask.None)
		{
			if ((triggers & check) == check)
			{
				triggerObjects[idx].GetComponent<ITriggerBase>().Trigger();
			}
			idx++;
			check = (LayerMask)((int)check << 1);
		}
	}

	int GetTriggerIdx(char l)
	{
		string idxS = "123456789ABCDEFG";
		for (int a=0;a<idxS.Length;a++)
		{
			if (l==idxS[a])
			{
				return a;
			}
		}
		return 0;
	}

	void ProcessBaseTriggers(Tilemap tilemap)
	{
		// Currently assumes there are 16 triggers
		triggerObjects = new GameObject[16];

		// Base triggers just define the tile co-ord and type of controlling object
		for (int x = tilemap.cellBounds.min.x; x < tilemap.cellBounds.max.x; x++)
		{
			for (int y = tilemap.cellBounds.min.y; y < tilemap.cellBounds.max.y; y++)
			{
				Vector3Int cellPos = new Vector3Int(x, y, 0);
				Sprite s = tilemap.GetSprite(cellPos);
				if (s != null)
				{
					string triggerName = s.name;
					int triggerIdx = GetTriggerIdx(s.name[s.name.Length - 1]);

					//Find gameobject for this trigger
					foreach (Transform child in transform)
					{
						if (child.name == triggerName)
						{
							triggerObjects[triggerIdx] = child.gameObject;
						}
					}

					if (triggerObjects[triggerIdx] != null)
					{
						triggerObjects[triggerIdx].transform.position = tilemap.GetCellCenterWorld(cellPos);
						triggerObjects[triggerIdx].GetComponent<ITriggerBase>().Init(this);
						triggerObjects [triggerIdx].GetComponent<ITriggerBase> ().SetupBase (tilemap.GetCellCenterWorld(cellPos));
					}
				}
			}
		}

	}

	void ProcessTriggerLayer(Tilemap tilemap)
	{
		int ofx = tilemap.origin.x - mainTilemap.origin.x;
		int ofy = tilemap.origin.y - mainTilemap.origin.y;

		for (int x = tilemap.cellBounds.min.x; x < tilemap.cellBounds.max.x; x++)
		{
			for (int y = tilemap.cellBounds.min.y; y < tilemap.cellBounds.max.y; y++)
			{
				Vector3Int cellPos = new Vector3Int (x, y, 0);
				Sprite s = tilemap.GetSprite(cellPos);
				if (s != null)
				{
					// Fill the entire X/Y tile space with collision layer
					LayerMask orIn = LayerMask.None;
					switch (s.name)
					{
					case "Trigger_1":
						orIn = LayerMask.Trigger1;
						break;
					case "Trigger_2":
						orIn = LayerMask.Trigger2;
						break;
					case "Trigger_3":
						orIn = LayerMask.Trigger3;
						break;
					case "Trigger_4":
						orIn = LayerMask.Trigger4;
						break;
					case "Trigger_5":
						orIn = LayerMask.Trigger5;
						break;
					case "Trigger_6":
						orIn = LayerMask.Trigger6;
						break;
					case "Trigger_7":
						orIn = LayerMask.Trigger7;
						break;
					case "Trigger_8":
						orIn = LayerMask.Trigger8;
						break;
					case "Trigger_9":
						orIn = LayerMask.Trigger9;
						break;
					case "Trigger_A":
						orIn = LayerMask.TriggerA;
						break;
					case "Trigger_B":
						orIn = LayerMask.TriggerB;
						break;
					case "Trigger_C":
						orIn = LayerMask.TriggerC;
						break;
					case "Trigger_D":
						orIn = LayerMask.TriggerD;
						break;
					case "Trigger_E":
						orIn = LayerMask.TriggerE;
						break;
					case "Trigger_F":
						orIn = LayerMask.TriggerF;
						break;
					case "Trigger_G":
						orIn = LayerMask.TriggerG;
						break;
					}
					string triggerName = s.name;
					int triggerIdx = GetTriggerIdx(s.name[s.name.Length - 1]);

					if (triggerObjects[triggerIdx] != null)
					{
						triggerObjects [triggerIdx].GetComponent<ITriggerBase> ().SetupTrigger (tilemap.GetCellCenterWorld(cellPos));
						for (int xx = 0; xx < sizeX; xx++)
						{
							for (int yy = 0; yy < sizeY; yy++)
							{
								int fx = (ofx + (x - tilemap.origin.x)) * sizeX;
								int fy = (ofy + (y - tilemap.origin.y)) * sizeY;

								collisionMap[fx + xx, fy + yy] |= orIn;
							}
						}
					}
				}
			}
		}
	}

	void ProcessTrigger(Tilemap tilemap)
	{
		// Needs to be first in list, since it defines the trigger objects
		if (tilemap.name == "Trigger_Base")
		{
			ProcessBaseTriggers(tilemap);
		}
		else
		{
			ProcessTriggerLayer(tilemap);
		}

		tilemap.gameObject.SetActive(false);    // Switch off the layer
	}

	void ComputeCollisionBitmaps()
	{
		var components = transform.parent.gameObject.GetComponentsInChildren<Tilemap>();

		foreach (var tilemap in components)
		{
			if (tilemap.name.StartsWith("Trigger_"))
			{
				ProcessTrigger(tilemap);
			}
			else
				ComputeCollisionBitmap(tilemap);
		}
	}

	// Use this for initialization
	void Start()
	{
		mainTilemap = GetComponent<Tilemap>();
		mainTilemapBounds = mainTilemap.localBounds;

		// todo move these to a seperate script
		GetComponent<TilemapRenderer>().material.SetVector("_WorldBounds", new Vector4( -3.0f, mainTilemapBounds.min.y, 3.0f, mainTilemapBounds.max.y) );
		GetComponent<TilemapRenderer>().material.SetColor("_RepBGMin",backgroundMinColour);
		GetComponent<TilemapRenderer>().material.SetColor("_RepBGMax",backgroundMaxColour);
		GetComponent<TilemapRenderer>().material.SetColor("_RepGoalMin",goalMinColour);
		GetComponent<TilemapRenderer>().material.SetColor("_RepGoalMax", goalMaxColour);

		collisionMap = new LayerMask[mainTilemap.cellBounds.size.x * sizeX, mainTilemap.cellBounds.size.y * sizeY];

		ComputeCollisionBitmaps();
	}

	Vector3Int GetPixelCoord(Vector3 worldPos)
	{
		Vector3 localPos = mainTilemap.WorldToLocal(worldPos) - mainTilemap.origin;
		Vector3Int cellPos = mainTilemap.LocalToCell(localPos);
		Vector3 cellDiff = localPos - cellPos;
		cellPos *= new Vector3Int(sizeX, sizeY, 1);
		cellPos += new Vector3Int((int)(cellDiff.x * sizeX), (int)(cellDiff.y * sizeY), 0);
		return cellPos;
	}

	Vector3Int GetCellCoord(Vector3 worldPos)
	{
		Vector3 localPos = mainTilemap.WorldToLocal(worldPos) - mainTilemap.origin;
		Vector3Int cellPos = mainTilemap.LocalToCell(localPos);
		cellPos *= new Vector3Int(sizeX, sizeY, 1);
		return cellPos;
	}

	public bool IsCollision(Vector3 worldPos, LayerMask compareMask)
	{
		Vector3Int cellPos = GetPixelCoord(worldPos);
		return (collisionMap[cellPos.x, cellPos.y] & compareMask) != LayerMask.None;
	}

	public bool IsBoxCollision(Vector3 worldPos, LayerMask compareMask)
	{
		Vector3Int cellPos = GetCellCoord(worldPos);
		bool collision = false;
		for (int x = 0; x < sizeX; x++)
		{
			for (int y = 0; y < sizeY; y++)
			{
				collision |= ((collisionMap[cellPos.x + x, cellPos.y + y] & compareMask) != LayerMask.None);
			}
		}
		return collision;
	}

	public LayerMask GetCollisionMask(Vector3 worldPos)
	{
		Vector3Int cellPos = GetPixelCoord(worldPos);
		return collisionMap[cellPos.x, cellPos.y];
	}

	public LayerMask GetCollisionMask(Vector3 worldPos, LayerMask compareMask)
	{
		Vector3Int cellPos = GetPixelCoord(worldPos);
		return collisionMap[cellPos.x, cellPos.y] & compareMask;
	}

	public void DeleteTile(Tilemap tm2, Vector3 wPos, LayerMask extra = LayerMask.None)
	{
		Vector3 lPos = mainTilemap.WorldToLocal(wPos);
		Vector3Int cellPos = mainTilemap.LocalToCell(lPos);

		tm2.SetTile(tm2.WorldToCell(wPos), null);
		cellPos -= mainTilemap.origin;
		cellPos *= new Vector3Int(sizeX, sizeY, 1);
		for (int x = cellPos.x; x < cellPos.x + sizeX; x++)
		{
			for (int y = cellPos.y; y < cellPos.y + sizeY; y++)
			{
				collisionMap[x, y] &= ~(LayerMask.Background | LayerMask.Player | extra);
			}
		}
	}

	public void DeleteTile(Vector3 wPos, LayerMask extra = LayerMask.None)
	{
		DeleteTile(mainTilemap, wPos, extra);
	}

	public void MoveTile1PixelDown(Tilemap tm2, Vector3 wPos)
	{
		Matrix4x4 mat = tm2.GetTransformMatrix(tm2.WorldToCell(wPos));
		Vector4 v1 = mat.GetRow(1);
		v1.w = v1.w - 1 / 12.0f;
		mat.SetRow(1, v1);
		tm2.SetTransformMatrix(tm2.WorldToCell(wPos), mat);
	}

	public void MoveTile12PixelsUp(Tilemap tm2, Vector3 wPos)
	{
		Matrix4x4 mat = tm2.GetTransformMatrix(tm2.WorldToCell(wPos));
		Vector4 v1 = mat.GetRow(1);
		v1.w = v1.w + 12 / 12.0f;
		mat.SetRow(1, v1);
		tm2.SetTransformMatrix(tm2.WorldToCell(wPos), mat);
	}

	public bool HandleCollapse(Tilemap tm2, Vector3 wPos)
	{
		Vector3 lPos = mainTilemap.WorldToLocal(wPos);
		Vector3Int cellPos = mainTilemap.LocalToCell(lPos);

		// Get Cell above this one
		cellPos += new Vector3Int(0, 1, 0);
		cellPos -= mainTilemap.origin;
		cellPos *= new Vector3Int(sizeX, sizeY, 1);
		int cnt = 0;
		for (int x = cellPos.x; x < cellPos.x + sizeX; x++)
		{
			if ((collisionMap[x, cellPos.y] & LayerMask.Water) == LayerMask.Water)
			{
				cnt++;
			}
		}

		if (cnt > 2)
		{
			DeleteTile(tm2, wPos);
			return true;
		}
		return false;
	}

	public bool HandlePressure(Tilemap tm2, ref Interactions.PressureData pd, Interactions interacts)
	{
		if (pd.totalSize > 0)
		{
			Vector3 wPos = pd.position;
			Vector3 lPos = mainTilemap.WorldToLocal(wPos);
			Vector3Int cellPos = mainTilemap.LocalToCell(lPos);

			// Get Cell above this one
			cellPos -= mainTilemap.origin;
			cellPos *= new Vector3Int(sizeX, sizeY, 1);
			cellPos += new Vector3Int(0, pd.deltaCollapse, 0);
			int cnt = 0;
			for (int x = cellPos.x; x < cellPos.x + sizeX; x++)
			{
				for (int y = cellPos.y; y < cellPos.y + sizeY*pd.numBlocksCheck; y++)
				{
					if ((collisionMap[x, y] & LayerMask.Water) == LayerMask.Water)
					{
						cnt++;
					}
				}
			}
			pd.failsafeCollapse -= Time.deltaTime;
			if ((cnt >= sizeX * Mathf.FloorToInt(sizeY*pd.numBlocksCheck*pressureAccumulationFactor)) || (pd.deltaCollapse != 12 && pd.failsafeCollapse <= 0.0f))
			{
				pd.failsafeCollapse = interacts.failsafeDelay;
				Vector3 nPos = wPos;
				for (int a = 0; a < pd.totalSize; a++)
				{
					MoveTile1PixelDown(tm2, nPos);
					nPos += new Vector3(0, -1, 0);
				}
				cellPos -= new Vector3Int(0, sizeY, 0);
				for (int x = cellPos.x; x < cellPos.x + sizeX; x++)
				{
					collisionMap[x, cellPos.y + 11] &= ~LayerMask.Background;
				}
				pd.deltaCollapse--;
				if (pd.deltaCollapse == 0)
				{
					var origTile = tm2.GetTile<Tile>(tm2.WorldToCell(pd.position));
					tm2.SetTile(tm2.WorldToCell(pd.position), null);
					pd.deltaCollapse = 12;
					pd.totalSize--;
					pd.numBlocksCheck++;
					pd.position += new Vector3(0, -1, 0);
					tm2.SetTile(tm2.WorldToCell(pd.position), origTile);
					nPos = pd.position;
					for (int a = 0; a < pd.totalSize; a++)
					{
						MoveTile12PixelsUp(tm2, nPos);
						nPos += new Vector3(0, -1, 0);
					}
				}
				return true;
			}
		}
		return false;
	}


	public void RemovePixel(Vector3 worldPos, LayerMask removeMask)
	{
		Vector3Int cellPos = GetPixelCoord(worldPos);
		collisionMap[cellPos.x, cellPos.y] &= ~removeMask;
	}

	public void AddPixel(Vector3 worldPos, LayerMask setMask)
	{
		Vector3Int cellPos = GetPixelCoord(worldPos);
		collisionMap[cellPos.x, cellPos.y] |= setMask;
	}

	public void RemoveSweep(Vector3 worldPos, Vector3 nPos, LayerMask removeMask)
	{
		Vector3Int ocellPos = GetPixelCoord(worldPos);
		Vector3Int ncellPos = GetPixelCoord (nPos);
		Vector3 fcell = ncellPos - ocellPos;
		Vector3 ocell = ocellPos;
		fcell.Normalize ();

		do
		{
			ocell+=fcell;
			ocellPos=Vector3Int.RoundToInt(ocell);
			collisionMap[ocellPos.x,ocellPos.y] &= ~removeMask;
		} while (ocellPos != ncellPos);
	}

	public void AddSweep(Vector3 worldPos, Vector3 nPos, LayerMask setMask)
	{
		Vector3Int ocellPos = GetPixelCoord(worldPos);
		Vector3Int ncellPos = GetPixelCoord (nPos);
		Vector3 fcell = ncellPos - ocellPos;
		Vector3 ocell = ocellPos;
		fcell.Normalize ();

		do
		{
			ocell+=fcell;
			ocellPos=Vector3Int.RoundToInt(ocell);
			collisionMap[ocellPos.x,ocellPos.y] |= setMask;
		} while (ocellPos != ncellPos);
	}

	public LayerMask SweepCollisionMask(Vector3 worldPos,Vector3 nPos, LayerMask compareMask)
	{
		Vector3Int ocellPos = GetPixelCoord(worldPos);
		Vector3Int ncellPos = GetPixelCoord (nPos);
		LayerMask col = LayerMask.None;
		Vector3 fcell = ncellPos - ocellPos;
		Vector3 ocell = ocellPos;
		fcell.Normalize ();

		do
		{
			ocell+=fcell;
			ocellPos=Vector3Int.RoundToInt(ocell);
			col|=(collisionMap[ocellPos.x,ocellPos.y] & compareMask);
		} while (ocellPos != ncellPos);

		return col;
	}

	public void AddBox(Vector3 worldPos, LayerMask setMask,Vector2Int offs=default(Vector2Int))
	{
		Vector3Int cellPos = GetCellCoord(worldPos);
		for (int x = 0; x < sizeX; x++)
		{
			for (int y = 0; y < sizeY; y++)
			{
				collisionMap [cellPos.x + x + offs.x, cellPos.y + y + offs.y] |= setMask;
			}
		}
	}

	public string GetTileName(Vector3 worldPos)
	{
		return mainTilemap.GetSprite(mainTilemap.WorldToCell(worldPos)).name;
	}

	public void DeleteBox(Vector3 worldPos, LayerMask removeMask,Vector2Int offs=default(Vector2Int))
	{
		Vector3Int cellPos = GetCellCoord(worldPos);
		for (int x = 0; x < sizeX; x++)
		{
			for (int y = 0; y < sizeY; y++)
			{
				collisionMap [cellPos.x + x + offs.x, cellPos.y + y + offs.y] &= ~removeMask;
			}
		}
	}

	// Update is called once per frame
	void Update()
	{

	}
}
