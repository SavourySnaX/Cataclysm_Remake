using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Interactions : MonoBehaviour 
{
	public struct PressureData
	{
		public Vector3	position;
		public int		totalSize;
		public int		deltaCollapse;
	};

	public List<Vector3> collapseLocations;
	public List<PressureData> pressureLocations;

	public GameObject background;
	public HudBehaviour hud;

	Tilemap tm;
	BitmapCollision bmpCol;

	// Use this for initialization
	void Start () 
	{
		collapseLocations = new List<Vector3>();
		pressureLocations = new List<PressureData> ();

		tm = GetComponent<Tilemap>();
		bmpCol = background.GetComponent<BitmapCollision> ();
		for (int x = tm.cellBounds.min.x; x <= tm.cellBounds.max.x; x++) 
		{
			for (int y = tm.cellBounds.min.y; y <= tm.cellBounds.max.y; y++) 
			{
				Sprite s = tm.GetSprite (new Vector3Int (x, y, 0));

				if (s == null)
					continue;

				switch (s.name) 
				{
				case "Collapse":
					collapseLocations.Add (tm.CellToWorld (new Vector3Int (x, y, 0)));
					continue;
				case "Pressure_T":
					PressureData data;
					data.position = tm.CellToWorld (new Vector3Int (x, y, 0));
					data.deltaCollapse = 12;
					// Compute size of pressure tile
					data.totalSize = 1;
					var nextTile = new Vector3Int (x, y - 1, 0);
					while (true)
					{
						Sprite ns = tm.GetSprite (nextTile);
						if (ns == null)
							break;
						if (ns.name == "Pressure_B")
						{
							data.totalSize++;
							nextTile += new Vector3Int (0, -1, 0);
						} 
						else
						{
							break;
						}
					}
					pressureLocations.Add (data);
					continue;
				default:
					continue;
				}
			}
		}
	}

	void CheckCollapse()
	{
		List<Vector3> toRemove = new List<Vector3> ();
		foreach (var v in collapseLocations)
		{
			if (bmpCol.HandleCollapse (tm, v))
			{
				hud.ScoreDestroyBlock ();
				toRemove.Add (v);
			}
		}
		foreach (var v in toRemove)
		{
			collapseLocations.Remove (v);
		}
	}

	void CheckPressure()
	{
		for (int a=0;a<pressureLocations.Count;a++)
		{
			var t = pressureLocations [a];
			bmpCol.HandlePressure (tm,ref t);
			pressureLocations [a] = t;
		}
	}

	// Update is called once per frame
	void FixedUpdate () 
	{
		CheckCollapse ();
		CheckPressure ();
	}
}
