using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SwitchTigger : MonoBehaviour, ITriggerBase
{
	public AudioClip clip;
	public PlayerController player;
	public Vector3 gateDirection;
	public float speed;

	public Tilemap gateLayer;
	public Tilemap switchLayer;

	public Tile tileOpened;
	public Tile tileClosed;

	BitmapCollision bmpCol;

	Vector3 gateOrigPos;
	Vector3Int gateCell;
	Vector3Int switchCell;

	float position;
	public bool closed;

	GlobalAudioManager globalAudio;

	void Start()
	{
		position = 0.0f;
		globalAudio = GameObject.Find("GlobalAudio").GetComponent<GlobalAudioManager> ();
	}

	public void Init(BitmapCollision col)
	{
		bmpCol = col;
	}
	
	public void SetupBase(Vector3 wPos)
	{
		gateCell = gateLayer.WorldToCell (wPos);
		Matrix4x4 mat = gateLayer.GetTransformMatrix (gateCell);
		gateOrigPos = new Vector3 (mat.GetRow (0).w, mat.GetRow (1).w, mat.GetRow (2).w);
		bmpCol.AddBox (wPos, BitmapCollision.LayerMask.DynamicBlock);
	}

	public void SetupTrigger(Vector3 wPos)
	{
		switchCell = switchLayer.WorldToCell (wPos);
	}

	public void Trigger()
	{
		if ((closed && position==0.0f) || (!closed && position==1.0f))
		{
			globalAudio.PlayClip (clip);
			closed = !closed;
			switchLayer.SetTile (switchCell, closed?tileClosed:tileOpened);
		}
	}

	public void FixedUpdate()
	{
		bmpCol.DeleteBox (gateLayer.CellToWorld (gateCell), BitmapCollision.LayerMask.DynamicBlock, new Vector2Int (Mathf.RoundToInt (gateDirection.x * position * bmpCol.sizeX), Mathf.RoundToInt (gateDirection.y * position * bmpCol.sizeY)));
		if (closed)
		{
			position = Mathf.Clamp (position - Time.deltaTime * speed, 0.0f, 1.0f);
		} 
		else
		{
			position = Mathf.Clamp (position + Time.deltaTime * speed, 0.0f, 1.0f);
		}

		if (position != 0.0f && position != 1.0f)
		{
			UpdateGate ();
		}
		bmpCol.AddBox (gateLayer.CellToWorld (gateCell), BitmapCollision.LayerMask.DynamicBlock, new Vector2Int (Mathf.RoundToInt (gateDirection.x * position * bmpCol.sizeX), Mathf.RoundToInt (gateDirection.y * position * bmpCol.sizeY)));

	}

	public void UpdateGate()
	{
		Matrix4x4 mat = gateLayer.GetTransformMatrix(gateCell);
		Vector4 r = mat.GetRow (0);
		r.w = gateOrigPos.x + gateDirection.x * position;
		mat.SetRow (0,r);
		r = mat.GetRow (1);
		r.w = gateOrigPos.y + gateDirection.y * position;
		mat.SetRow (1,r);

		gateLayer.SetTransformMatrix(gateCell, mat);
	}

}
