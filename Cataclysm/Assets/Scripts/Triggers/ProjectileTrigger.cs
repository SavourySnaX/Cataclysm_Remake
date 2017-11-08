﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ProjectileTrigger : MonoBehaviour, ITriggerBase
{
	public AudioClip clip;
	public PlayerController player;
	public Vector3 directionMin;
	public Vector3 directionMax;
	public AnimationCurve directionWeighting;
	public Vector3 spawnOffset;
	public float speed;
	public float speedAdjust=0f;
	public Vector3 drag;
	public GameObject bulletPrefab;
	public List<GameObject> bulletPrefabs;
	public float fireRateMin;
	public float fireRateMax;
	public int multiMin = 1;
	public int multiMax = 1;
	public AnimationCurve fireRateWeighting;
	public BitmapCollision.LayerMask colMask = BitmapCollision.LayerMask.All;
	public BitmapCollision.LayerMask colType = BitmapCollision.LayerMask.None;

	float fireTime;
	BitmapCollision bmpCol;

	GlobalAudioManager globalAudio;

	void Start()
	{
		fireTime = 0.0f;
		globalAudio = GameObject.Find("GlobalAudio").GetComponent<GlobalAudioManager> ();
	}

	public void Init(BitmapCollision col)
	{
		bmpCol = col;
	}

	public void SetupBase(Vector3 wpos)
	{
	}

	public void SetupTrigger(Vector3 wpos)
	{
	}

	public void Trigger()
	{
		if (fireTime == 0.0f)
		{
			int num = Random.Range (multiMin, multiMax);
			for (int a = 0; a < num; a++)
			{
				globalAudio.PlayClip (clip);
				fireTime = Mathf.Lerp (fireRateMin, fireRateMax, fireRateWeighting.Evaluate (Random.value));
				GameObject go;
				if (bulletPrefabs.Count > 0)
				{
					go = Instantiate (bulletPrefabs [a % bulletPrefabs.Count], transform.position + spawnOffset, transform.rotation);
				} 
				else
				{
					go = Instantiate (bulletPrefab, transform.position + spawnOffset, transform.rotation);
				}
				LinearProjectileMove moveScript = go.GetComponent<LinearProjectileMove> ();
				moveScript.bmpCol = bmpCol;
				moveScript.speed = speed + Random.Range (0f, speedAdjust);
				moveScript.direction = Vector3.Lerp (directionMin, directionMax, directionWeighting.Evaluate (Random.value));
				moveScript.player = player;
				moveScript.drag = drag;
				moveScript.colMask = colMask;
				moveScript.colType = colType;
			}
		}
	}

	public void FixedUpdate()
	{
		fireTime = Mathf.Max(fireTime - Time.deltaTime, 0.0f);
	}
}
