using UnityEngine;
using System.Collections;

public class RoomSpawner : MonoBehaviour {

	public float ShakeAmount = 1.0f;
	public RoomSpawnerState State;
	public float RiseSpeed = 0.5f;

	private Vector3 _originalPosition;
	private Vector3 _endPosition;



	// Use this for initialization
	void Start () {
		State = RoomSpawnerState.NotActivate;
		_originalPosition = this.transform.position;
		_endPosition = new Vector3(_originalPosition.x, 1, _originalPosition.z);
	}
	
	// Update is called once per frame
	void Update () {
		
		switch(State)
		{
		case RoomSpawnerState.NotActivate:
			break;
		case RoomSpawnerState.Activating:
			Rise();
			Shake();

			if (transform.position.y >= _endPosition.y){
				transform.position = _endPosition;
				State = RoomSpawnerState.Activated;
			}
			
			break;
		case RoomSpawnerState.Activated:
			break;
		}

	}

	private void Shake()
	{
		transform.position = _originalPosition + Random.insideUnitSphere * ShakeAmount;
	}

	private void Rise()
	{
		_originalPosition += Vector3.up * RiseSpeed * Time.deltaTime;
	}
}

public enum RoomSpawnerState
{
	NotActivate,
	Activating,
	Activated,
	Destroyed
}
