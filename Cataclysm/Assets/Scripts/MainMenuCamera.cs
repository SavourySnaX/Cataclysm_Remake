using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
	public float speed;

	public float leftExtent;
	public float rightExtent;
	public float topExtent;
	public float bottomExtent;
	public float finalScale;

	float length;
	float dir;

	void Start()
	{
		length = 0.0f;
		dir = 1;

		Camera.main.orthographicSize = Mathf.Lerp(6, finalScale, length);
		transform.position = new Vector3(Mathf.Lerp(leftExtent, rightExtent, length), Mathf.Lerp(bottomExtent, topExtent, length), transform.position.z);
	}

	void FixedUpdate()
	{
		length += dir * speed * Time.deltaTime;
		if (dir > 0.0f)
		{
			if (length >= 1.0f)
			{
				dir = 0.0f;
				length = 1.0f;
			}
		}
		if (dir < 0.0f)
		{
			if (length <= 0.0f)
			{
				dir = 1.0f;
				length = 0.0f;
			}
		}

		Camera.main.orthographicSize = Mathf.Lerp(6, finalScale, length);
		transform.position = new Vector3(Mathf.Lerp(leftExtent, rightExtent, length), Mathf.Lerp(bottomExtent, topExtent, length), transform.position.z);
	}
}
