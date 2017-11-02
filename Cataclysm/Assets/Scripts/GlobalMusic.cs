using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalMusic : MonoBehaviour 
{
	private static GlobalMusic instance = null;

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
