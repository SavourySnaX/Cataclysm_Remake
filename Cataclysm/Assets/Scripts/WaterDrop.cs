using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDrop : MonoBehaviour
{

	public BitmapCollision bmpCol;
	public HudBehaviour hud;
	public WaterSource src;
	SpriteRenderer sr;

	public Color fast;
	public Color medium;
	public Color normal;

	void Start()
	{
		sr = GetComponent<SpriteRenderer>();
	}

	void FixedUpdate()
	{
		BitmapCollision.LayerMask checkCollision = BitmapCollision.LayerMask.All;
		BitmapCollision.LayerMask curLayer = bmpCol.GetCollisionMask(transform.position);
		if ((curLayer & (BitmapCollision.LayerMask.Player | BitmapCollision.LayerMask.Block | BitmapCollision.LayerMask.DynamicBlock)) != BitmapCollision.LayerMask.None)
		{
			checkCollision &= ~(curLayer & (BitmapCollision.LayerMask.Player | BitmapCollision.LayerMask.Block | BitmapCollision.LayerMask.DynamicBlock));
			if (src.waterIsLethal && ((curLayer & BitmapCollision.LayerMask.Player)==BitmapCollision.LayerMask.Player))
			{
				src.player.KillPlayer();
			}
		}
		bool col = bmpCol.IsCollision(transform.position + Vector3.down * (1f / bmpCol.sizeY), checkCollision);
		if (!col)
		{
			bmpCol.RemovePixel(transform.position, BitmapCollision.LayerMask.Water);
			transform.position += Vector3.down * (1f / bmpCol.sizeY);
			bmpCol.AddPixel(transform.position, BitmapCollision.LayerMask.Water);
			sr.color = Color.Lerp(medium,fast,Random.value);
		}
		else
		{
			BitmapCollision.LayerMask lm = bmpCol.GetCollisionMask(transform.position + Vector3.down * (1f / bmpCol.sizeY));
			if ((BitmapCollision.LayerMask.Drain & lm) == BitmapCollision.LayerMask.Drain)
			{
				bmpCol.RemovePixel(transform.position, BitmapCollision.LayerMask.Water);
				DestroyObject(gameObject);
				hud.AddWater();
				return;
			}
			if ((BitmapCollision.LayerMask.FailDrain & lm) == BitmapCollision.LayerMask.FailDrain)
			{
				bmpCol.RemovePixel(transform.position, BitmapCollision.LayerMask.Water);
				DestroyObject(gameObject);
				hud.AddFail();
				src.AddWater();
				return;
			}
			var direction = Vector3.right;
			if (Random.Range(-10.0f, 10.0f) <= 0)
			{
				direction = Vector3.left;
			}
			bool ccol = bmpCol.IsCollision(transform.position + direction * (1f / bmpCol.sizeX), checkCollision);
			if (!ccol)
			{
				bmpCol.RemovePixel(transform.position, BitmapCollision.LayerMask.Water);
				transform.position += direction * (1f / bmpCol.sizeX);
				bmpCol.AddPixel(transform.position, BitmapCollision.LayerMask.Water);
				sr.color = Color.Lerp(normal,medium,Random.value);
			}
			else
			{
				sr.color = normal;
			}
		}

	}

}
