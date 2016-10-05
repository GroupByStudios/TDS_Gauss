using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class VideoPlayer : MonoBehaviour {

	MovieTexture movie;
	AudioSource audio;

	// Use this for initialization
	void Start () {

		movie = GetComponent<RawImage>().mainTexture as MovieTexture;
		audio = GetComponent<AudioSource>();

		if (movie != null && audio != null)
		{
			audio.Stop();
			movie.Stop();

			audio.Play();
			movie.Play();
		}

	}

	void Update()
	{
		if (movie != null)
		{
			if(!movie.isPlaying)
			{
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
			}
		}
		else
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
		}
	}
}
