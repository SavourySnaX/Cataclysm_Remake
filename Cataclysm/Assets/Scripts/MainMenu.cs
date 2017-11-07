using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
	ScoreBoard globalScores;

	void Start()
	{
		Time.timeScale = 1.0f;
		Cursor.visible = true;
		globalScores = GameObject.Find("GlobalScores").GetComponent<ScoreBoard> ();
		if (globalScores.GetCurrentLevel () != 99)
		{
			// Set default button
			GameObject go = GameObject.Find("Level"+globalScores.GetCurrentLevel());
			if (go != null)
			{
				GameObject.Find ("EventSystem").GetComponent<EventSystem> ().firstSelectedGameObject = go;
			}
		}
	}

	public void LoadLevel(int level)
	{
		globalScores.SetCurrentLevel (level);
		SceneManager.LoadScene("level" + level);
	}

	public void Quit()
	{
		Application.Quit();
	}

	void Update()
	{

	}
}
