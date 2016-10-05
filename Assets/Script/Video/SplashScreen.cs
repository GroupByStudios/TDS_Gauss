using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SplashScreen : MonoBehaviour {

	public VideoPlayer player;
	public float SplashScreenSecond = 2f;

	float currentSplashScreenSecond = 0f;

	// Use this for initialization
	void Start () {
	

	}
	
	// Update is called once per frame
	void Update () {
	
		currentSplashScreenSecond += Time.deltaTime;

		if (currentSplashScreenSecond > SplashScreenSecond)
		{
			if (player != null)
			{
				player.gameObject.SetActive(true);
				gameObject.SetActive(false);
			}
			else
			{
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
			}
		}
	}
}
