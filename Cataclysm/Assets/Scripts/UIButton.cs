using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIButton : MonoBehaviour , ISelectHandler, IPointerEnterHandler
{
	public GameObject trophyPrefab;
	public GameObject timePrefab;
	public int lvl;
	ScoreBoard globalScores;

	Text stats;
	// Use this for initialization
	void Start ()
	{
		globalScores = GameObject.Find ("GlobalScores").GetComponent<ScoreBoard> ();

		GetComponent<Button> ().interactable = (Application.isEditor || globalScores.IsUnlocked (lvl));
		if (Application.isEditor || globalScores.GetTime (lvl) < globalScores.GetRecordTime(lvl))
		{
			Instantiate (timePrefab, transform);
		}
		if (Application.isEditor || globalScores.GetScore (lvl) > globalScores.GetRecordScore(lvl))
		{
			Instantiate (trophyPrefab, transform);
		}
		stats = GameObject.Find ("Stats").GetComponentInChildren<Text> ();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (Cursor.visible)
		{
			EventSystem.current.SetSelectedGameObject (gameObject, eventData);
		}
	}

	public void OnSelect(BaseEventData eventData)
	{
		string score;
		string time;
		if (globalScores.GetScore (lvl) == 0)
		{
			score = "Score ----\n";
		} 
		else
		{
			score = string.Format ("Score {0:0000}\n", globalScores.GetScore (lvl));
		}
		if (globalScores.GetTime (lvl) == 10000f)
		{
			time = "Time --:--";
		} 
		else
		{
			int fminutes = Mathf.FloorToInt (globalScores.GetTime (lvl) / 60);
			int fseconds = Mathf.FloorToInt (globalScores.GetTime (lvl) % 60);
			time = string.Format ("Time {0:00}:{1:00}", fminutes, fseconds);
		}
		stats.text = score + time;
	}

	// Update is called once per frame
	void Update () 
	{
	}
}
