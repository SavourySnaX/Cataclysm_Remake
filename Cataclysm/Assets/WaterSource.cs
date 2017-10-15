using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WaterSource : MonoBehaviour {

	public Vector3[] spawnLocations;
	public int numSpawnLocations;

	public int totalDrops=500;
	public float spawnTimer=0.25f;
	public Vector3 spawnLocation;
	float nextSpawn=0.0f;
	public GameObject prefab;
	public GameObject background;
	BoxCollider2D bc;
	int layerMaskForMovement;
	Tilemap tm;

	// Use this for initialization
	void Start () 
	{
		bc = prefab.GetComponent<BoxCollider2D> ();
		layerMaskForMovement = LayerMask.GetMask ("Background", "Interaction", "Water", "Player");

		spawnLocations = new Vector3[20];
		numSpawnLocations = 0;

		tm = GetComponent<Tilemap>();
		for (int x = 0; x < tm.cellBounds.size.x; x++) 
		{
			for (int y = 0; y < tm.cellBounds.size.y; y++) 
			{
				Sprite s = tm.GetSprite (new Vector3Int (x, y, 0));

				if (s == null)
					continue;

				Debug.Log (x+" "+y+" "+s.name);

				Vector3 spawnStart;
				switch (s.name) 
				{
				case "Water_L":
					spawnStart = new Vector3 (-1 / 3.0f, 0, 0);
					break;
				case "Water_R":
					spawnStart = new Vector3 (4 / 3.0f, 0, 0);
					break;
				case "Water_U":
					spawnStart = new Vector3 (0, 13 / 12.0f, 0);
					break;
				case "Water_D":
					spawnStart = new Vector3 (0, -1 / 12.0f, 0);
					break;
				default:
					continue;
				}

				var vec = tm.CellToWorld (new Vector3Int (x, y,0))+tm.origin;
				vec += spawnStart;

				spawnLocations [numSpawnLocations++] = vec;
			}
		}

	}
	
	// Update is called once per frame
	void Update () 
	{
		nextSpawn += Time.deltaTime;
		if (nextSpawn >= spawnTimer)
		{
			nextSpawn = 0.0f;
			var ray = Physics2D.BoxCast(spawnLocation,bc.size,0.0f, Vector2.zero, layerMaskForMovement);
			if (ray.collider == null)
			{
				if (totalDrops > 0)
				{
					totalDrops--;

					Instantiate (prefab, spawnLocation, Quaternion.identity).GetComponent<WaterDrop> ().bmpCol = background.GetComponent<BitmapCollision> ();
				}
			}
		}
	}
}
