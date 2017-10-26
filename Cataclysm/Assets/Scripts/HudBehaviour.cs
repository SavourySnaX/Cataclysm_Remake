using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudBehaviour : MonoBehaviour 
{
	float timer;
	float water;
	int blocksCount;
	float score;
	public float scoreDestroyBlock=3.0f;
	public float scoreWater=0.10f;
	public float waterIncrement=1.0f;
	public float failIncrement=0.2f;
	public float timerStart=240.0f;
	public float waterAmount=800.0f;

	public GameObject prefabUI;

	Text scoreObject;
	Text blockObject;
	Text scoreShadowObject;
	Text blockShadowObject;
	RectTransform timerObject;
	RectTransform waterObject;

	void Start () 
	{
		score = 0.0f;
		timer = 0;
		blocksCount = 8;
		water = 0;

		GameObject t=Instantiate (prefabUI);
		t.GetComponent<Canvas> ().worldCamera = Camera.main;

		var list = t.GetComponentsInChildren<RectTransform> ();
		foreach (var l in list)
		{
			if (l.name == "WaterMeterHeight")
			{
				waterObject = l;
			}
			if (l.name =="FailMeterHeight")
			{
				timerObject = l;
			}
		}
		var tlist = t.GetComponentsInChildren<Text> ();
		foreach (var l in tlist)
		{
			if (l.name == "Score")
			{
				scoreObject = l;
			}
			if (l.name =="ScoreDropShadow")
			{
				scoreShadowObject = l;
			}
			if (l.name == "BlockCount")
			{
				blockObject = l;
			}
			if (l.name =="BlockCountDropShadow")
			{
				blockShadowObject = l;
			}
		}

	}

	public void AddWater()
	{
		water += waterIncrement;
		score += scoreWater;
	}

	public void AddFail()
	{
		timer += failIncrement;
	}

	public void SetBlocks(int bc)
	{
		blocksCount = bc;
	}

	public void ScoreDestroyBlock()
	{
		score += scoreDestroyBlock;
	}

	void UpdateWaterMeter()
	{
		if (water >= waterAmount)
		{
			// Win
			return;
		}
		Vector3 lScale = waterObject.localScale;
		lScale.y = water / waterAmount;
		waterObject.localScale = lScale;

	}

	void UpdateFailMeter()
	{
		if (timer >= timerStart)
		{
			// Game Over
			return;
		}
		Vector3 lScale = timerObject.localScale;
		lScale.y = timer / timerStart;
		timerObject.localScale = lScale;
	}

	void UpdateScore()
	{
		string t = string.Format ("Score : {0:0000}", Mathf.FloorToInt(score));
		scoreShadowObject.text = t;
		scoreObject.text = t;
	}

	void UpdateBlocks()
	{
		string t = string.Format ("* {0}", blocksCount);
		blockShadowObject.text = t;
		blockObject.text = t;
	}

	void Update () 
	{
		timer += Time.deltaTime;
		UpdateWaterMeter ();
		UpdateFailMeter ();
		UpdateScore ();
		UpdateBlocks ();
	}
}
