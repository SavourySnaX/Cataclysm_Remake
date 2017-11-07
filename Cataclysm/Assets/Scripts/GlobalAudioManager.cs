﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalAudioManager : MonoBehaviour 
{
	public AudioClip collapse;
	public AudioClip collectBlock;
	public AudioClip dropBlock;
	public AudioClip laser1;
	public AudioClip laser2;
	public AudioClip mortar;
	public AudioClip playerDeath;
	public AudioClip plug;
	public AudioClip pressure;
	public AudioClip purpleDeath;

	AudioSource audioSrc;

	float collapseDelay;
	float pressureDelay;

	private static GlobalAudioManager instance = null;

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
		audioSrc = GetComponent<AudioSource> ();
		collapseDelay = 0.0f;
		pressureDelay = 0.0f;
	}

	public void CollectBlock()
	{
		if (!Application.isEditor)
		{
			audioSrc.PlayOneShot (collectBlock);
		}
	}

	public void DropBlock()
	{
		if (!Application.isEditor)
		{
			audioSrc.PlayOneShot (dropBlock);
		}
	}

	public void Collapse()
	{
		if (!Application.isEditor)
		{
			if (collapseDelay == 0.0f)
			{
				audioSrc.PlayOneShot (collapse);
				collapseDelay = collapse.length;
			}
		}
	}

	public void Pressure()
	{
		if (!Application.isEditor)
		{
			if (pressureDelay == 0.0f)
			{
				audioSrc.PlayOneShot (pressure);
				pressureDelay = pressure.length;
			}
		}
	}

	public void PlayerDeath()
	{
		if (!Application.isEditor)
		{
			audioSrc.PlayOneShot (playerDeath);
		}
	}

	public void Plug()
	{
		if (!Application.isEditor)
		{
			audioSrc.PlayOneShot (plug);
		}
	}

	public void PlayClip(AudioClip clip)
	{
		if (!Application.isEditor)
		{
			audioSrc.PlayOneShot (clip);
		}
	}

	public void PurpleDeath()
	{
		if (!Application.isEditor)
		{
			audioSrc.PlayOneShot (purpleDeath);
		}
	}

	void FixedUpdate () 
	{
		collapseDelay = Mathf.Max (0.0f, collapseDelay - Time.deltaTime);
		pressureDelay = Mathf.Max (0.0f, pressureDelay - Time.deltaTime);
	}
}
