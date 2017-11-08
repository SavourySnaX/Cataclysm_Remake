﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WaterSource : MonoBehaviour
{

	public List<Vector3> spawnLocations;

	public int totalDrops = 500;
	public float spawnTimer = 0.25f;
	public int curSpawnLoc = 0;
	float nextSpawn = 0.0f;
	public GameObject prefab;
	public GameObject background;
	public Color normal = new Color(51 / 255.0f, 119 / 255.0f, 119 / 255.0f, 0.75f);
	public Color medium = new Color(81 / 255.0f, 190 / 255.0f, 190 / 255.0f, 0.825f);
	public Color fast = new Color(109 / 255.0f, 1.0f, 1.0f, 1.0f);
	public bool waterIsLethal=false;
	public PlayerController player;
	Tilemap tm;
	BitmapCollision bmpCol;

	void Start()
	{
		curSpawnLoc = 0;

		spawnLocations = new List<Vector3>();

		tm = GetComponent<Tilemap>();
		bmpCol = background.GetComponent<BitmapCollision>();
		for (int x = tm.cellBounds.min.x; x <= tm.cellBounds.max.x; x++)
		{
			for (int y = tm.cellBounds.min.y; y <= tm.cellBounds.max.y; y++)
			{
				Sprite s = tm.GetSprite(new Vector3Int(x, y, 0));

				if (s == null)
					continue;

				Vector3 spawnStart;
				switch (s.name)
				{
					case "Water_L":
						spawnStart = new Vector3(-1 + 2 / 3.0f + (1 / 3.0f) / 2.0f, 11 / 12.0f + (1 / 24.0f), 0);
						break;
					case "Water_R":
						spawnStart = new Vector3(1 + (1 / 3.0f) / 2.0f, (11 / 12.0f) + (1 / 24.0f), 0);
						break;
					case "Water_U":
						spawnStart = new Vector3 (1 / 3.0f + (1 / 3.0f) / 2.0f, 13 / 12.0f - (1 / 24.0f), 0);
						break;
					case "Water_D":
						spawnStart = new Vector3(1 / 3.0f + (1 / 3.0f) / 2.0f, -1 / 12.0f - (1 / 24.0f), 0);
						break;
					default:
						continue;
				}

				var vec = tm.CellToWorld(new Vector3Int(x, y, 0));
				vec += spawnStart;

				spawnLocations.Add(vec);
			}
		}
		GameObject pObj = GameObject.Find("Player");
		if (pObj != null)
		{
			player = pObj.GetComponent<PlayerController>();
		}
		else
		{
			player = null;
		}
	}

	public void AddWater()
	{
		totalDrops++;
	}

	void SpoutWater()
	{
		nextSpawn += Time.deltaTime;
		if (nextSpawn >= spawnTimer)
		{
			bool col = bmpCol.IsCollision(spawnLocations[curSpawnLoc], BitmapCollision.LayerMask.All);
			if (!col)
			{
				nextSpawn = 0.0f;
				if (totalDrops > 0)
				{
					totalDrops--;

					GameObject t = Instantiate(prefab, spawnLocations[curSpawnLoc], Quaternion.identity);
					t.GetComponent<WaterDrop>().bmpCol = background.GetComponent<BitmapCollision>();
					t.GetComponent<WaterDrop>().hud = background.GetComponent<HudBehaviour>();
					t.GetComponent<WaterDrop>().src = this.GetComponent<WaterSource>();
					t.GetComponent<WaterDrop>().normal = normal;
					t.GetComponent<WaterDrop>().medium = medium;
					t.GetComponent<WaterDrop>().fast = fast;
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

	void FixedUpdate()
	{
		SpoutWater();
	}
}
