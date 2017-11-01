using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	readonly float pixelWidth = 48;
	public BitmapCollision bc;
	public Transform target;

	void FollowTargetInBounds(bool snap = false)
	{
		Camera.main.orthographicSize = Screen.height / (2 * pixelWidth);
		float vertExtent = Camera.main.orthographicSize;
		float horzExtent = (vertExtent * Screen.width) / Screen.height;
		float leftBound = bc.mainTilemapBounds.min.x + horzExtent;
		float rightBound = bc.mainTilemapBounds.max.x - horzExtent;
		var pos = new Vector3(target.position.x, target.position.y, transform.position.z);

		if (leftBound > rightBound)
		{
			pos.x = rightBound + (Screen.width - pixelWidth * bc.mainTilemapBounds.size.x) / pixelWidth / 2.0f;
		}
		else
		{
			pos.x = Mathf.Clamp(pos.x, leftBound, rightBound);
		}

		float bottomBound = bc.mainTilemapBounds.min.y + vertExtent;
		float topBound = bc.mainTilemapBounds.max.y - vertExtent;

		if (bottomBound > topBound)
		{
			pos.y = topBound + (Screen.height - pixelWidth * bc.mainTilemapBounds.size.y) / pixelWidth / 2.0f;
		}
		else
		{
			pos.y = Mathf.Clamp(pos.y, bottomBound, topBound);
		}

		if (snap)
		{
			transform.position = pos;
		}
		else
		{
			transform.position = Vector3.MoveTowards(transform.position, pos, 25.0f * Time.deltaTime);
		}
	}

	void Start()
	{
		FollowTargetInBounds(true);
	}

	void FixedUpdate()
	{
		FollowTargetInBounds();

	}
}
