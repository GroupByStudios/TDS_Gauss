using UnityEngine;
using System.Collections;

public class CreditsScript : MonoBehaviour {

	public float Speed = 10f;
	public float YLimite = 1949;

	// Update is called once per frame
	void Update () {

		if (transform.position.y < YLimite)
		{
			transform.position = transform.position + Vector3.up * Speed * Time.deltaTime;
		}

	}
}
