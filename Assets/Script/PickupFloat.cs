using UnityEngine;
using System.Collections;

public class PickupFloat : MonoBehaviour {

	public float RotationSpeed = 15f;

	// Update is called once per frame
	void Update () {


		transform.Rotate(Vector3.up * RotationSpeed * Time.deltaTime);
	
	}
}
