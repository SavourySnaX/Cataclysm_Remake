using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBoard : MonoBehaviour 
{
	const int numLevels = 44;
	const int lvlAdjust = 3;

	int currentLevel;
	public bool newHighScore;
	public bool newFastTime;

	float[] fastestTimes=new float[numLevels]{210f,190f,70f,999f,80f,80f,110f,170f,130f,120f,110f,210f,100f,120f,210f,210f,140f,140f,200f,80f,240f,200f,140f,100f,
		150f,130f,120f,200f,210f,110f,160f,130f,160f,145f,150f,110f,75f,110f,285f,150f,160f,205f,0,0};
	int[] highestScores=new int[numLevels]{130,130,150,0,130,150,160,150,140,140,130,150,140,140,130,160,160,170,145,135,145,130,130,140,
		140,130,145,130,130,130,140,140,140,145,140,130,150,130,150,155,130,130,0,0};
	private static ScoreBoard instance = null;

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad (this.gameObject);
		} 
		else
		{
			DestroyObject (this.gameObject);
		}
	}

	// Use this for initialization
	void Start () 
	{
		newHighScore = false;
		newFastTime = false;
		currentLevel = 99;
	}

	public void SetCurrentLevel(int lvl)
	{
		currentLevel = lvl;
		newHighScore = false;
		newFastTime = false;
	}

	public int GetRecordScore(int lvl)
	{
		return highestScores [lvl+lvlAdjust];
	}

	public float GetRecordTime(int lvl)
	{
		return fastestTimes [lvl+lvlAdjust];
	}

	public int GetCurrentLevel()
	{
		return currentLevel;
	}

	public bool IsUnlocked(int lvl)
	{
		return PlayerPrefs.GetInt("unlock"+(lvl + lvlAdjust),lvl>=-1 && lvl<=1?1:0)==1?true:false;
	}

	public float GetTime(int lvl)
	{
		return PlayerPrefs.GetFloat ("time" + (lvl + lvlAdjust), 10000f);
	}

	public float GetScore(int lvl)
	{
		return PlayerPrefs.GetInt ("score" + (lvl + lvlAdjust), 0);
	}

	public float GetTime()
	{
		return PlayerPrefs.GetFloat ("time" + (currentLevel + lvlAdjust), 10000f);
	}

	public float GetScore()
	{
		return PlayerPrefs.GetInt ("score" + (currentLevel + lvlAdjust), 0);
	}

	public bool IsHighScore()
	{
		return newHighScore;
	}

	public bool IsFastest()
	{
		return newFastTime;
	}

	public void LevelComplete(float time, int score)
	{
		if (PlayerPrefs.GetInt ("score" + (currentLevel + lvlAdjust), 0) < score)
		{
			PlayerPrefs.SetInt ("score" + (currentLevel + lvlAdjust), score);
			newHighScore = true;
		}
		if (PlayerPrefs.GetFloat ("time" + (currentLevel + lvlAdjust), 10000f) > time)
		{
			PlayerPrefs.SetFloat ("time" + (currentLevel + lvlAdjust), time);
			newFastTime = true;
		}
		if (NextLevel()+lvlAdjust < numLevels)
		{
			PlayerPrefs.SetInt ("unlock" + (NextLevel() + lvlAdjust), 1);
		}
		PlayerPrefs.Save ();
	}

	public bool LevelAvailable(int lvl)
	{
		if (lvl >= -3 && lvl <= 38)
			return true;
		return false;
	}

	public int NextLevel()
	{
		if (currentLevel >= 0)
		{
			return currentLevel + 1;
		}

		return currentLevel - 1;
	}
}
