using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTrigger : MonoBehaviour, ITriggerBase
{
	public PlayerController player;
	public Vector3 directionMin;
	public Vector3 directionMax;
	public Vector3 spawnOffset;
	public float speed;
	public Vector3 drag;
	public GameObject bulletPrefab;
	public float fireRateMin;
	public float fireRateMax;

	float fireTime;
	BitmapCollision bmpCol;

	void Start()
	{
		fireTime = 0.0f;
	}

	public void Init(BitmapCollision col)
	{
		bmpCol = col;
	}

	public void Trigger()
	{
		if (fireTime == 0.0f)
		{
			fireTime = Mathf.Lerp (fireRateMin, fireRateMax, Random.value);
			GameObject go = Instantiate(bulletPrefab, transform.position + spawnOffset, transform.rotation);
			LinearProjectileMove moveScript = go.GetComponent<LinearProjectileMove>();
			moveScript.bmpCol = bmpCol;
			moveScript.speed = speed;
			moveScript.direction = Vector3.Lerp(directionMin,directionMax,Random.value);
			moveScript.player = player;
			moveScript.drag = drag;
		}
	}

	public void FixedUpdate()
	{
		fireTime = Mathf.Max(fireTime - Time.deltaTime, 0.0f);
	}
}
