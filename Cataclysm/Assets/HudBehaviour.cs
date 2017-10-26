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
	public float timerStart=60.0f;
	public float waterAmount=1000.0f;
	public GameObject scoreObject;
	public GameObject blockObject;
	public GameObject scoreShadowObject;
	public GameObject blockShadowObject;
	public GameObject timerObject;
	public GameObject waterObject;

	// Use this for initialization
	void Start () 
	{
		score = 0.0f;
		timer = 0;
		blocksCount = 8;
		water = 0;
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
		Vector3 lScale = waterObject.GetComponent<RectTransform> ().localScale;
		lScale.y = water / waterAmount;
		waterObject.GetComponent<RectTransform> ().localScale = lScale;

	}

	void UpdateFailMeter()
	{
		if (timer >= timerStart)
		{
			// Game Over
			return;
		}
		Vector3 lScale = timerObject.GetComponent<RectTransform> ().localScale;
		lScale.y = timer / timerStart;
		timerObject.GetComponent<RectTransform> ().localScale = lScale;
	}

	void UpdateScore()
	{
		string t = string.Format ("Score : {0:0000}", Mathf.FloorToInt(score));
		scoreShadowObject.GetComponent<Text> ().text = t;
		scoreObject.GetComponent<Text> ().text = t;
	}

	void UpdateBlocks()
	{
		string t = string.Format ("* {0}", blocksCount);
		blockShadowObject.GetComponent<Text> ().text = t;
		blockObject.GetComponent<Text> ().text = t;
	}

	// Update is called once per frame
	void Update () 
	{
		timer += Time.deltaTime;
		UpdateWaterMeter ();
		UpdateFailMeter ();
		UpdateScore ();
		UpdateBlocks ();
	}
}
