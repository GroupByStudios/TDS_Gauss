using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class VideoPlayer : MonoBehaviour {

	MovieTexture movie;
	AudioSource audioSource;

	// Use this for initialization
	void Start () {

		movie = GetComponent<RawImage>().mainTexture as MovieTexture;
		audioSource = GetComponent<AudioSource>();

		if (movie != null && audioSource != null)
		{
			audioSource.Stop();
			movie.Stop();

			audioSource.Play();
			movie.Play();
		}

	}

	void Update()
	{
		if (movie != null)
		{
			if(!movie.isPlaying || Input.anyKey)
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
