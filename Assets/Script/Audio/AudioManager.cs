using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

	AudioSource[] myAudioSources;

	void Awake()
	{
		myAudioSources = GetComponents<AudioSource>();
	}

	int GetFreeAudioSourceIndex()
	{
		for(int i = 0; i < myAudioSources.Length; i++)
		{
			if (!myAudioSources[i].isPlaying) 
				return i;
		}

		return -1;
	}

	public void StopAll()
	{
		for(int i = 0; i < myAudioSources.Length; i++)
		{
			myAudioSources[i].Stop();
		}		
	}

	public void PlayClip(AudioClip clip_)
	{
		int _index = GetFreeAudioSourceIndex();

		if (_index > -1){
			myAudioSources[_index].clip = clip_;
			myAudioSources[_index].Play();
		}

		Debug.Log("AudioManager - Index: " + _index);
	}
}
