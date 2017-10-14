using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BitmapCollision : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
	{
		TileMap tm = transform.parent.GetComponent<TileMap>;
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
}
