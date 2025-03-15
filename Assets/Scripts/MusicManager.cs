using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

	public static MusicManager Instance;

	public AudioSource mainMusic;

	static bool isPlaying = false;

	float lastVolume;

	void Awake()
	{
		if (Instance != null)
		{
			Destroy(this);
			return;
		}

		Instance = this;
		DontDestroyOnLoad (this.gameObject);
	}

	// Use this for initialization
	void Start () 
	{
		if (!isPlaying) {
			mainMusic.Play ();
			isPlaying = true;
			DontDestroyOnLoad (mainMusic);
		}
		lastVolume = AudioListener.volume;
	}

	public bool Mute(){
		if (AudioListener.volume > 0) {
			lastVolume = AudioListener.volume;
			AudioListener.volume = 0;
			return true;
		} else {
			AudioListener.volume = lastVolume;
			return false;
		}
	}
}
