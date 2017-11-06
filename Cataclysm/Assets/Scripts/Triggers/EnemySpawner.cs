using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemySpawner : MonoBehaviour, ITriggerBase
{
	public GameObject enemyPrefab;

	public int maxEnemies=3;
	public float spawnDelay=2.0f;

	public PlayerController player;

	BitmapCollision bmpCol;
	float spawnTime;
	int numEnemies;

	void Start()
	{
		spawnTime = spawnDelay;
		numEnemies = 0;
	}

	public void Init(BitmapCollision col)
	{
		bmpCol = col;
	}
	
	public void SetupBase(Vector3 wPos)
	{
	}

	public void SetupTrigger(Vector3 wPos)
	{
	}

	public void Trigger()
	{
	}

	public void Died()
	{
		if (numEnemies>0)
		{
			spawnTime+=spawnDelay;
			numEnemies--;
		}
	}

	public void FixedUpdate()
	{
		spawnTime = Mathf.Max (0.0f, spawnTime - Time.deltaTime);
		if (spawnTime == 0.0f && numEnemies < maxEnemies)
		{
			spawnTime = spawnDelay;
			numEnemies++;

			GameObject go = Instantiate (enemyPrefab, transform.position, Quaternion.identity);
			go.GetComponent<IEnemyBase> ().Init (bmpCol, player, this);
		}
	}
}
