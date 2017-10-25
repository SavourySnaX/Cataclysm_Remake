using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		
	}

	public void LoadLevel(int level)
	{
		SceneManager.LoadScene ("level" + level);
	}

	public void Quit()
	{
		Application.Quit ();
	}

	// Update is called once per frame
	void Update () 
	{
		
	}
}
