﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WaterSource : MonoBehaviour {

	public List<Vector3> spawnLocations;

	public int totalDrops=500;
	public float spawnTimer=0.25f;
	public int curSpawnLoc=0;
	float nextSpawn=0.0f;
	public GameObject prefab;
	public GameObject background;

	Tilemap tm;
	BitmapCollision bmpCol;

	// Use this for initialization
	void Start () 
	{
		curSpawnLoc = 0;

		spawnLocations = new List<Vector3>();

		tm = GetComponent<Tilemap>();
		bmpCol = background.GetComponent<BitmapCollision> ();
		for (int x = tm.cellBounds.min.x; x <= tm.cellBounds.max.x; x++) 
		{
			for (int y = tm.cellBounds.min.y; y <= tm.cellBounds.max.y; y++) 
			{
				Sprite s = tm.GetSprite (new Vector3Int (x, y, 0));

				if (s == null)
					continue;

				Vector3 spawnStart;
				switch (s.name) 
				{
				case "Water_L":
					spawnStart = new Vector3 (-1 + 2/3.0f + (1/3.0f)/2.0f, 11/12.0f + (1/24.0f), 0);
					break;
				case "Water_R":
					spawnStart = new Vector3 (1 + (1/3.0f)/2.0f, (11/12.0f) + (1/24.0f), 0);
					break;
/*				case "Water_U":
					spawnStart = new Vector3 (0, 13 / 12.0f, 0);
					break;*/
				case "Water_D":
					spawnStart = new Vector3 (1/3.0f + (1/3.0f)/2.0f, -1 / 12.0f - (1/24.0f), 0);
					break;
				default:
					continue;
				}

				var vec = tm.CellToWorld (new Vector3Int (x, y, 0));//+tm.origin;
				vec += spawnStart;

				spawnLocations.Add(vec);
			}
		}

	}

	void SpoutWater()
	{
		nextSpawn += Time.deltaTime;
		if (nextSpawn >= spawnTimer)
		{
			bool col = bmpCol.IsCollision (spawnLocations[curSpawnLoc]);
			//var ray = Physics2D.BoxCast(spawnLocations[curSpawnLoc],bc.size,0.0f, Vector2.zero, layerMaskForMovement);
			//if (ray.collider == null)
			if (!col)
			{
				nextSpawn = 0.0f;
				if (totalDrops > 0)
				{
					totalDrops--;

					Instantiate (prefab, spawnLocations[curSpawnLoc], Quaternion.identity).GetComponent<WaterDrop> ().bmpCol = background.GetComponent<BitmapCollision> ();
				}
			}

			curSpawnLoc++;
			if (curSpawnLoc >= spawnLocations.Count)
			{
				nextSpawn = 0.0f;
				curSpawnLoc = 0;
			}
		}
	}

	// Update is called once per frame
	void FixedUpdate () 
	{
		SpoutWater ();
	}
}
