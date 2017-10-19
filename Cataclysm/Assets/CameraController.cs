using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour 
{
	public BitmapCollision bc;
	public Transform target;

	void FollowTargetInBounds()
	{
		float vertExtent = Camera.main.orthographicSize;
		float horzExtent = (vertExtent * Screen.width) / Screen.height;
		float leftBound = bc.mainTilemapBounds.min.x + horzExtent;
		float rightBound = bc.mainTilemapBounds.max.x-1.0f - horzExtent;
		float bottomBound = bc.mainTilemapBounds.min.y + vertExtent;
		float topBound = bc.mainTilemapBounds.max.y - vertExtent;

		var pos = new Vector3 (target.position.x, target.position.y, transform.position.z);
		pos.x = Mathf.Clamp (pos.x, leftBound, rightBound);
		pos.y = Mathf.Clamp (pos.y, bottomBound, topBound);

		transform.position = Vector3.MoveTowards (transform.position, pos, 25.0f*Time.deltaTime);
	}

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		FollowTargetInBounds ();

	}
}
