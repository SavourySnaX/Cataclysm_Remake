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

	// Use this for initialization
	void Start () 
	{
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

		musSlider.value = globalMusic.volume;
		fxSlider.value = globalAudio.volume;
	}

	public void OnMusicChanged()
	{
		globalMusic.volume = musSlider.value;
	}

	public void OnFxChanged()
	{
		globalAudio.volume = fxSlider.value;
		globalAudio.GetComponent<GlobalAudioManager> ().Pressure ();
	}
}
