using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSource : MonoBehaviour {

	public int totalDrops=500;
	public float spawnTimer=0.25f;
	public Vector3 spawnLocation;
	float nextSpawn=0.0f;
	public GameObject prefab;
	public GameObject background;
	BoxCollider2D bc;
	int layerMaskForMovement;

	// Use this for initialization
	void Start () 
	{
		bc = prefab.GetComponent<BoxCollider2D> ();
		layerMaskForMovement = LayerMask.GetMask ("Background", "Interaction", "Water", "Player");
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
