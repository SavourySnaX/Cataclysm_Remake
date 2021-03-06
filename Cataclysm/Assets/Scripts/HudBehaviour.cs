﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HudBehaviour : MonoBehaviour
{
	float timer;
	float water;
	int blocksCount;
	float score;
	bool pause = false;
	public float gameTimeScale = 1f;
	public float enemyScore = 2.0f;
	public float plugScore = 2.0f;
	public float scoreDestroyBlock = 3.0f;
	public float scoreWater = 0.10f;
	public float waterIncrement = 1.0f;
	public float failIncrement = 0.2f;
	public float timerStart = 240.0f;
	public float waterAmount = 800.0f;

	public GameObject prefabUI;
	public GameObject prefabPopup;
	public BitmapCollision.LayerMask expectedMask = BitmapCollision.LayerMask.Water1;

	GameObject currentPopup = null;
	ScoreBoard globalScores;

	Text scoreObject;
	Text blockObject;
	Text scoreShadowObject;
	Text blockShadowObject;
	RectTransform timerObject;
	RectTransform waterObject;

	void Start()
	{
		gameTimeScale = 1f;
		score = 0.0f;
		timer = 0;
		blocksCount = 8;
		water = 0;
		pause = false;
		Cursor.visible = false;

		GameObject t = Instantiate(prefabUI);
		t.GetComponent<Canvas>().worldCamera = Camera.main;

		var list = t.GetComponentsInChildren<RectTransform>();
		foreach (var l in list)
		{
			if (l.name == "WaterMeterHeight")
			{
				waterObject = l;
			}
			if (l.name == "FailMeterHeight")
			{
				timerObject = l;
			}
		}
		var tlist = t.GetComponentsInChildren<Text>();
		foreach (var l in tlist)
		{
			if (l.name == "Score")
			{
				scoreObject = l;
			}
			if (l.name == "ScoreDropShadow")
			{
				scoreShadowObject = l;
			}
			if (l.name == "BlockCount")
			{
				blockObject = l;
			}
			if (l.name == "BlockCountDropShadow")
			{
				blockShadowObject = l;
			}
		}
		GameObject go = GameObject.Find ("GlobalScores");
		if (go!=null)
		{
			globalScores = go.GetComponent<ScoreBoard> ();
		}
		else
		{
			globalScores=null;
		}
	}

	public bool AddWater(BitmapCollision.LayerMask expected)
	{
		if (expected != expectedMask)
		{
			AddFail ();
			return false;
		}
		water += waterIncrement;
		score += scoreWater;
		return true;
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

	public void ScorePlug()
	{
		score += plugScore;
	}

	public void ScoreEnemy()
	{
		score += enemyScore;
	}

	void UpdateWaterMeter()
	{
		if (water >= waterAmount)
		{
			Winner();
			return;
		}
		Vector3 lScale = waterObject.localScale;
		lScale.y = water / waterAmount;
		waterObject.localScale = lScale;
	}

	public void ShowPopup(string title, string message, string buttonText, System.Action method, string otherButtonText, System.Action otherMethod,bool defaultButtonMain)
	{
		pause = true;
		Cursor.visible = true;
		if (currentPopup == null)
		{
			currentPopup = Instantiate(prefabPopup);
			currentPopup.GetComponent<Canvas>().worldCamera = Camera.main;

			Button firstSelected = null;
			var list = currentPopup.GetComponentsInChildren<Button>();
			foreach (var l in list)
			{
				if (l.name == "MainMenuButton")
				{
					l.onClick.AddListener(new UnityAction(method));
					if (defaultButtonMain)
					{
						firstSelected = l;
					}
				}
				if (l.name == "RetryButton")
				{
					if (otherMethod==null)
					{
						l.transform.gameObject.SetActive(false);
						continue;
					}
					l.onClick.AddListener(new UnityAction(otherMethod));
					if (!defaultButtonMain)
					{
						firstSelected = l;
					}
				}
			}

			var slist = currentPopup.GetComponentsInChildren<EventSystem>();
			foreach (var l in slist)
			{
				l.SetSelectedGameObject(firstSelected.gameObject);
			}

			var tlist = currentPopup.GetComponentsInChildren<Text>();
			foreach (var l in tlist)
			{
				if (l.name == "Title")
				{
					l.text = title;
				}
				if (l.name == "Message")
				{
					l.text = message;
				}
				if (l.name == "Button")
				{
					l.text = buttonText;
				}
				if (l.name == "OtherButton")
				{
					l.text = otherButtonText;
				}
			}
		}
	}

	public void ClosePopup()
	{
		if (currentPopup != null)
		{
			DestroyObject(currentPopup);
			currentPopup = null;
		}
		pause = false;
		Time.timeScale = 1f;
		Cursor.visible = false;
	}

	public void GameOver()
	{
		ShowPopup("GAME OVER", "You ran out of time!!", "Main Menu", MainMenu,"Retry",CurrentLevel,false);
	}

	public void Quit()
	{
		ShowPopup("GAME OVER", "You quit!!", "Main Menu", MainMenu,"Retry",CurrentLevel,false);
	}

	public void Killed()
	{
		ShowPopup("GAME OVER", "You were killed!!", "Main Menu", MainMenu,"Retry",CurrentLevel,false);
	}

	public void Winner()
	{
		if (currentPopup == null)
		{
			string globalInfo = "";
			string minutes = Mathf.Floor (timer / 60).ToString ("0");
			string seconds = Mathf.Floor (timer % 60).ToString ("0");
			string minS = Mathf.Floor (timer / 60) != 1 ? "s" : "";
			string secS = Mathf.Floor (timer % 60) != 1 ? "s" : "";
			if (globalScores)
			{
				globalScores.LevelComplete (timer, Mathf.FloorToInt (score));
				globalInfo += "\n\n";
				if (globalScores.IsHighScore ())
				{
					globalInfo += "!!New Highest Score!!\n";
				} else
				{
					globalInfo += string.Format ("Highest Score : {0:0000}\n", globalScores.GetScore ());
				}
				if (globalScores.IsFastest ())
				{
					globalInfo += "!!New Fastest Time!!\n";
				} else
				{
					string fminutes = Mathf.Floor (globalScores.GetTime () / 60).ToString ("0");
					string fseconds = Mathf.Floor (globalScores.GetTime () % 60).ToString ("0");
					string fminS = Mathf.Floor (globalScores.GetTime() / 60) != 1 ? "s" : "";
					string fsecS = Mathf.Floor (globalScores.GetTime() % 60) != 1 ? "s" : "";
					globalInfo += string.Format ("Fastest : {0} minute" + fminS + " and {1} second" + fsecS, fminutes, fseconds);
				}
			}

			if (globalScores!=null && globalScores.LevelAvailable (globalScores.NextLevel ()))
			{
				ShowPopup ("!CONGRATULATIONS!", string.Format ("You scored : {0:0000}\nYou took : {1} minute" + minS + " and {2} second" + secS + globalInfo, Mathf.FloorToInt (score), minutes, seconds), "Next Level", NextLevel,"Main Menu",MainMenu,true);
			} else
			{
				ShowPopup ("!CONGRATULATIONS!", string.Format ("You scored : {0:0000}\nYou took : {1} minute" + minS + " and {2} second" + secS + globalInfo, Mathf.FloorToInt (score), minutes, seconds), "Main Menu", MainMenu,"",null,true);
			}
		}
	}

	public void Pause()
	{
		ShowPopup("PAUSED", "", "Resume", Resume, "Main Menu", MainMenu, true);
	}

	public void Resume()
	{
		ClosePopup();
	}

	public void MainMenu()
	{
		ClosePopup();
		SceneManager.LoadScene("mainmenu");
	}

	public void CurrentLevel()
	{
		ClosePopup();
		SceneManager.LoadScene("level" + globalScores.GetCurrentLevel());
	}

	public void NextLevel()
	{
		int nxt = globalScores.NextLevel ();
		globalScores.SetCurrentLevel(nxt);
		SceneManager.LoadScene("level"+nxt);
	}

	void UpdateFailMeter()
	{
		if (timer >= timerStart)
		{
			GameOver();
			return;
		}
		Vector3 lScale = timerObject.localScale;
		lScale.y = timer / timerStart;
		timerObject.localScale = lScale;
	}

	void UpdateScore()
	{
		string t = string.Format("Score : {0:0000}", Mathf.FloorToInt(score));
		scoreShadowObject.text = t;
		scoreObject.text = t;
	}

	void UpdateBlocks()
	{
		string t = string.Format("* {0}", blocksCount);
		blockShadowObject.text = t;
		blockObject.text = t;
	}

	void FixedUpdate()
	{
		timer += Time.deltaTime*gameTimeScale;
		UpdateWaterMeter();
		UpdateFailMeter();
		UpdateScore();
		UpdateBlocks();
		if (pause)
		{
			Time.timeScale = 0.0f;
		}
		else
		{
			Time.timeScale = gameTimeScale;
		}
	}
}
