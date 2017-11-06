using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	ScoreBoard globalScores;

	void Start()
	{
		Time.timeScale = 1.0f;
		Cursor.visible = true;
		globalScores = GameObject.Find("GlobalScores").GetComponent<ScoreBoard> ();
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
