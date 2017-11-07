using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemySpawner : MonoBehaviour, ITriggerBase
{
	readonly BitmapCollision.LayerMask[] layerNum = new BitmapCollision.LayerMask[4] {
		BitmapCollision.LayerMask.Enemy1,
		BitmapCollision.LayerMask.Enemy2,
		BitmapCollision.LayerMask.Enemy3,
		BitmapCollision.LayerMask.Enemy4
	};
	public GameObject enemyPrefab;

	public int maxEnemies=3;
	public float spawnDelay=2.0f;

	public PlayerController player;

	BitmapCollision bmpCol;
	float spawnTime;
	int numEnemies;

	GameObject [] enemy = new GameObject[4];

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

	public void KillMob(BitmapCollision.LayerMask l)
	{
		if ((l & BitmapCollision.LayerMask.Enemy1) != BitmapCollision.LayerMask.None)
		{
			enemy [0].GetComponent<IEnemyBase> ().Die ();
			return;
		}
		if ((l & BitmapCollision.LayerMask.Enemy2) != BitmapCollision.LayerMask.None)
		{
			enemy [1].GetComponent<IEnemyBase> ().Die ();
			return;
		}
		if ((l & BitmapCollision.LayerMask.Enemy3) != BitmapCollision.LayerMask.None)
		{
			enemy [2].GetComponent<IEnemyBase> ().Die ();
			return;
		}
		if ((l & BitmapCollision.LayerMask.Enemy4) != BitmapCollision.LayerMask.None)
		{
			enemy [3].GetComponent<IEnemyBase> ().Die ();
			return;
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
			go.GetComponent<IEnemyBase> ().Init (bmpCol, player, this,layerNum[numEnemies-1]);
			enemy [numEnemies - 1] = go;
		}
	}
}
