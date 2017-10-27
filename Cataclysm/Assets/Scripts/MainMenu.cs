using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	void Start () 
	{
		Time.timeScale = 1.0f;
	}

	public void LoadLevel(int level)
	{
		SceneManager.LoadScene ("level" + level);
	}

	public void Quit()
	{
		Application.Quit ();
	}

	void Update () 
	{
		
	}
}
