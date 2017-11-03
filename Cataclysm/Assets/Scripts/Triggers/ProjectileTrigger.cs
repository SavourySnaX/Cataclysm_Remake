using System.Collections;
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
	public Vector3 drag;
	public GameObject bulletPrefab;
	public float fireRateMin;
	public float fireRateMax;
	public AnimationCurve fireRateWeighting;

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
			globalAudio.PlayClip (clip);
			fireTime = Mathf.Lerp (fireRateMin, fireRateMax, fireRateWeighting.Evaluate(Random.value));
			GameObject go = Instantiate(bulletPrefab, transform.position + spawnOffset, transform.rotation);
			LinearProjectileMove moveScript = go.GetComponent<LinearProjectileMove>();
			moveScript.bmpCol = bmpCol;
			moveScript.speed = speed;
			moveScript.direction = Vector3.Lerp(directionMin,directionMax,directionWeighting.Evaluate(Random.value));
			moveScript.player = player;
			moveScript.drag = drag;
		}
	}

	public void FixedUpdate()
	{
		fireTime = Mathf.Max(fireTime - Time.deltaTime, 0.0f);
	}
}
