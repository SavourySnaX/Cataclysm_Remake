﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
	public float speed = 1.0f;
	public int totalBlocks = 8;
	public BitmapCollision bmpCol;
	public HudBehaviour hud;
	public GameObject boxPrefab;
	public GameObject deathParticlePrefab;
	public GameObject weaponPrefab;

	Vector3 position;
	List<GameObject> blocksList;
	Animator anim;
	bool dead;
	bool hasWeapon;

	ProjectileTrigger weapon;
	float keyResponse = 0.0f;
	readonly bool invincible=false;

	GlobalAudioManager globalAudio;

	void Start()
	{
		anim = GetComponent<Animator>();
		blocksList = new List<GameObject>();
		position = transform.position;
		dead = false;
		hasWeapon = false;
		globalAudio = GameObject.Find("GlobalAudio").GetComponent<GlobalAudioManager> ();
		hud.SetBlocks (totalBlocks);
	}

	void AddBlock(Vector3 position)
	{
		if (totalBlocks != 0)
		{
			globalAudio.DropBlock ();
			bmpCol.AddBox(transform.position, BitmapCollision.LayerMask.Block);
			blocksList.Add(Instantiate(boxPrefab, transform.position, Quaternion.identity));
			totalBlocks--;
			hud.SetBlocks(totalBlocks);
		}
	}

	void RemoveBlock(Vector3 nPos)
	{
		GameObject toRemove = null;
		for (int a = 0; a < blocksList.Count; a++)
		{
			if (blocksList[a].transform.position == nPos)
			{
				toRemove = blocksList[a];
				break;
			}
		}
		if (toRemove != null)
		{
			globalAudio.CollectBlock ();
			bmpCol.DeleteBox (toRemove.transform.position, BitmapCollision.LayerMask.Block);
			blocksList.Remove (toRemove);
			DestroyObject (toRemove);
			totalBlocks++;
			hud.SetBlocks (totalBlocks);
		} 
		else
		{
			// Picking up a pick placed in level block
			switch (bmpCol.GetTileName(nPos))
			{
				case "Pickup_ExtraBlock":
					totalBlocks++;
					hud.SetBlocks (totalBlocks);
					break;
				case "Pickup_Scatter":
				case "Pickup_Scatter2":
				case "Pickup_Laser":
				case "Pickup_Burst":
					weapon = Instantiate(weaponPrefab, transform).GetComponent<ProjectileTrigger>();
					weapon.Init(bmpCol);
					weapon.colMask = BitmapCollision.LayerMask.All & ~(BitmapCollision.LayerMask.Player|BitmapCollision.LayerMask.Water);
					weapon.colType = BitmapCollision.LayerMask.Bullet;
					hasWeapon = true;
					break;
				default:
					break;
			}
			globalAudio.CollectBlock();
			bmpCol.DeleteTile(nPos, BitmapCollision.LayerMask.Block);
		}
	}

	bool HandleDynamics(BitmapCollision.LayerMask colMask, Vector3 newPos)
	{
		// Check triggers
		if ((colMask & BitmapCollision.LayerMask.Triggers) != BitmapCollision.LayerMask.None)
		{
			ProcessTriggers(newPos);
		}
		if ((colMask & BitmapCollision.LayerMask.Plug) == BitmapCollision.LayerMask.Plug)
		{
			hud.ScorePlug ();
			globalAudio.Plug ();
			bmpCol.DeleteTile(newPos, BitmapCollision.LayerMask.Plug);
			return true;
		}
		if ((colMask & BitmapCollision.LayerMask.Block) == BitmapCollision.LayerMask.Block)
		{
			RemoveBlock(newPos);
			return true;
		}
		return false;
	}

	IEnumerator DeathSequence(System.Action uiAction)
	{
		if (!dead)
		{
			dead = true;
			globalAudio.PlayerDeath ();
			gameObject.GetComponent<Renderer>().enabled = false;
			gameObject.GetComponent<ParticleSystem>().Stop();
			Instantiate(deathParticlePrefab, transform.position, Quaternion.identity);
			yield return new WaitForSecondsRealtime(1.5f);
			uiAction();
		}
	}

	public void KilledEnemy()
	{
		hud.ScoreEnemy ();
	}

	public void KillPlayer()
	{
		if (!invincible)
		{
			StartCoroutine (DeathSequence (hud.Killed));
		}
	}

	void ProcessTriggers(Vector3 pos)
	{
		bmpCol.TriggerAction(bmpCol.GetCollisionMask(pos));
	}

	void FixedUpdate()
	{
		if (dead)
		{
			return;
		}
		BitmapCollision.LayerMask playerMask = BitmapCollision.LayerMask.All & (~(BitmapCollision.LayerMask.Water | BitmapCollision.LayerMask.Player | BitmapCollision.LayerMask.PlayerIgnore | BitmapCollision.LayerMask.Enemy));
		bmpCol.DeleteBox(transform.position, BitmapCollision.LayerMask.Player);

		if (bmpCol.IsCollision(transform.position, BitmapCollision.LayerMask.Triggers))
		{
			ProcessTriggers(transform.position);
		}

		if (Input.GetButton("PlaceBlock") && position == transform.position)
		{
			if (!bmpCol.IsBoxCollision(transform.position, BitmapCollision.LayerMask.Block | BitmapCollision.LayerMask.Background))
			{
				AddBlock(transform.position);
			}
		}
		if (Input.GetButton("Shoot") && hasWeapon)
		{
			weapon.spawnOffset = transform.localRotation * Vector3.right/2;
			weapon.directionMin.x = (transform.localRotation * Vector3.right).x;
			weapon.directionMax.x = (transform.localRotation * Vector3.right).x;
			GetComponentInChildren<ProjectileTrigger>().Trigger();
		}
		if (Input.GetButton("QuitLevel"))
		{
			StartCoroutine(DeathSequence(hud.Quit));
			return;
		}
		if (Input.GetButton("Pause"))
		{
			hud.Pause();
			return;
		}
		float hzMove = Input.GetAxis("Horizontal");
		float vtMove = Input.GetAxis("Vertical");

		if (Input.GetKeyDown("1"))
		{
			hud.gameTimeScale = 1f;
		}
		if (Input.GetKeyDown("2"))
		{
			hud.gameTimeScale = 1.25f;
		}
		if (Input.GetKeyDown("3"))
		{
			hud.gameTimeScale = 1.5f;
		}
		if (Input.GetKeyDown("4"))
		{
			hud.gameTimeScale = 1.75f;
		}
		if (Input.GetKeyDown("5"))
		{
			hud.gameTimeScale = 2f;
		}
		if (hzMove < -keyResponse && position == transform.position)
		{
			transform.localRotation = Quaternion.AngleAxis(180, Vector3.up);
			BitmapCollision.LayerMask colMask = bmpCol.GetCollisionMask(transform.position + Vector3.left);
			if (!bmpCol.IsBoxCollision(transform.position + Vector3.left, playerMask) || HandleDynamics(colMask, transform.position + Vector3.left))
			{
				position += Vector3.left;
			}
		}
		if (hzMove > keyResponse && position == transform.position)
		{
			transform.localRotation = Quaternion.AngleAxis(0, Vector3.up);
			BitmapCollision.LayerMask colMask = bmpCol.GetCollisionMask(transform.position + Vector3.right);
			if (!bmpCol.IsBoxCollision(transform.position + Vector3.right, playerMask) || HandleDynamics(colMask, transform.position + Vector3.right))
			{
				position += Vector3.right;
			}
		}
		if (vtMove > keyResponse && position == transform.position)
		{
			BitmapCollision.LayerMask colMask = bmpCol.GetCollisionMask(transform.position + Vector3.up);
			if (!bmpCol.IsBoxCollision(transform.position + Vector3.up, playerMask) || HandleDynamics(colMask, transform.position + Vector3.up))
			{
				position += Vector3.up;
			}
		}
		if (vtMove < -keyResponse && position == transform.position)
		{
			BitmapCollision.LayerMask colMask = bmpCol.GetCollisionMask(transform.position + Vector3.down);
			if (!bmpCol.IsBoxCollision(transform.position + Vector3.down, playerMask) || HandleDynamics(colMask, transform.position + Vector3.down))
			{
				position += Vector3.down;
			}
		}

		anim.SetBool("moving", transform.position != position);

		transform.position = Vector3.MoveTowards(transform.position, position, Time.deltaTime * speed);

		bmpCol.AddBox(transform.position, BitmapCollision.LayerMask.Player);
	}
}
