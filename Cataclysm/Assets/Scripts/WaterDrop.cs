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

	Color fast_src;
	Color medium_src;
	Color normal_src;

	public Color fast_dest;
	public Color medium_dest;
	public Color normal_dest;

	float changeRate;
	float destTime;
	bool colourMix;

	public BitmapCollision.LayerMask waterLayer=BitmapCollision.LayerMask.Water1;
	public BitmapCollision.LayerMask ignore=BitmapCollision.LayerMask.None;

	public BitmapCollision.LayerMask checkCollision;

	void Start()
	{
		sr = GetComponent<SpriteRenderer>();
		destTime = 0f;
		colourMix = false;
		normal_src = normal;
		medium_src = medium;
		fast_src = fast;
		changeRate = Random.Range(0.3f,0.7f);
		checkCollision = BitmapCollision.LayerMask.All & (~ignore);
		if (src.waterIsLethal)
		{
			checkCollision &= ~BitmapCollision.LayerMask.Player;
		}
	}

	void TransformColours()
	{
		if (colourMix && destTime != 1f)
		{
			fast = Color.Lerp (fast_src, fast_dest, destTime);
			medium = Color.Lerp (medium_src, medium_dest, destTime);
			normal = Color.Lerp (normal_src, normal_dest, destTime);
			destTime = Mathf.Clamp(destTime + Time.deltaTime * changeRate, 0f, 1f);
		}
	}

	void FixedUpdate()
	{
		TransformColours ();
		//if (waterLayer!=BitmapCollision.LayerMask.Water)
		{
			BitmapCollision.LayerMask waterComMask = bmpCol.GetCrossCollisionMask (transform.position, BitmapCollision.LayerMask.Water);
			if ((waterComMask & (~waterLayer)) != BitmapCollision.LayerMask.None)
			{
				colourMix = true;
				waterLayer = BitmapCollision.LayerMask.Water;
				bmpCol.AddPixel (transform.position, waterLayer);
			}
		}
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
			bmpCol.RemovePixel(transform.position, waterLayer);
			transform.position += Vector3.down * (1f / bmpCol.sizeY);
			bmpCol.AddPixel(transform.position, waterLayer);
			sr.color = Color.Lerp(medium,fast,Random.value);
		}
		else
		{
			BitmapCollision.LayerMask lm = bmpCol.GetCollisionMask(transform.position + Vector3.down * (1f / bmpCol.sizeY));
			if ((BitmapCollision.LayerMask.Drain & lm) == BitmapCollision.LayerMask.Drain)
			{
				bmpCol.RemovePixel(transform.position, waterLayer);
				DestroyObject(gameObject);
				if (!hud.AddWater (waterLayer))
				{
					src.AddWater ();
				}
				return;
			}
			if ((BitmapCollision.LayerMask.FailDrain & lm) == BitmapCollision.LayerMask.FailDrain)
			{
				bmpCol.RemovePixel(transform.position, waterLayer);
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
				bmpCol.RemovePixel(transform.position, waterLayer);
				transform.position += direction * (1f / bmpCol.sizeX);
				bmpCol.AddPixel(transform.position, waterLayer);
				sr.color = Color.Lerp(normal,medium,Random.value);
			}
			else
			{
				sr.color = normal;
			}
		}

	}

}
