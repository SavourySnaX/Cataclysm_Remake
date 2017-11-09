using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour 
{
	public Slider musSlider;
	public Slider fxSlider;

	AudioSource globalAudio;
	AudioSource globalMusic;

	bool firstFrame=true;

	// Use this for initialization
	void Start () 
	{
		firstFrame = true;
		globalAudio = GameObject.Find("GlobalAudio").GetComponent<AudioSource> ();
		globalMusic = GameObject.Find("Music").GetComponent<AudioSource> ();

		musSlider.onValueChanged.AddListener (delegate
		{
			OnMusicChanged();
		});
		fxSlider.onValueChanged.AddListener (delegate
		{
			OnFxChanged ();
		});

		musSlider.value = PlayerPrefs.GetFloat("MusicVolume",globalMusic.volume);
		fxSlider.value = PlayerPrefs.GetFloat("FXVolume",globalAudio.volume);
	}

	public void OnMusicChanged()
	{
		globalMusic.volume = musSlider.value;
		PlayerPrefs.SetFloat ("MusicVolume", musSlider.value);
	}

	public void OnFxChanged()
	{
		globalAudio.volume = fxSlider.value;
		PlayerPrefs.SetFloat ("FXVolume", fxSlider.value);
		if (!firstFrame)
		{
			globalAudio.GetComponent<GlobalAudioManager> ().Pressure ();
		}
	}

	public void Update()
	{
		firstFrame = false;
	}
}
