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
		if (lvl >= -3 && lvl <= 10)
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
