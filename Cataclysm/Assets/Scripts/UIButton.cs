using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButton : MonoBehaviour 
{
	public GameObject trophyPrefab;
	public GameObject timePrefab;
	public int lvl;
	ScoreBoard globalScores;

	// Use this for initialization
	void Start () 
	{
		globalScores = GameObject.Find ("GlobalScores").GetComponent<ScoreBoard>();

		GetComponent<Button> ().interactable = (Application.isEditor || globalScores.IsUnlocked (lvl));
		if (globalScores.GetTime (lvl) < 90f)
		{
			Instantiate (timePrefab, transform);
		}
		if (globalScores.GetScore (lvl) > 100)
		{
			Instantiate (trophyPrefab, transform);
		}
	}

	// Update is called once per frame
	void Update () 
	{
		
	}
}
