using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalState : MonoBehaviour 
{
	private static GlobalState instance = null;

	void OnApplicationQuit()
	{
		PlayerPrefs.Save ();
	}

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

}
