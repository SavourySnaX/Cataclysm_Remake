using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Interactions : MonoBehaviour 
{
	public List<Vector3> collapseLocations;
	public List<Vector3> pressureLocations;

	public GameObject background;

	Tilemap tm;
	BitmapCollision bmpCol;

	// Use this for initialization
	void Start () 
	{
		collapseLocations = new List<Vector3>();
		pressureLocations = new List<Vector3> ();

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
					pressureLocations.Add (tm.CellToWorld (new Vector3Int (x, y, 0)));
					continue;
				default:
					continue;
				}
			}
		}
	}

	void CheckCollapse()
	{
		foreach (var v in collapseLocations)
		{
			bmpCol.HandleCollapse (tm,v);
		}
	}

	void CheckPressure()
	{
	}

	// Update is called once per frame
	void Update () 
	{
		CheckCollapse ();
		CheckPressure ();
	}
}
