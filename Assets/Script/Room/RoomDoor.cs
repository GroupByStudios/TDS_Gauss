using UnityEngine;
using System.Collections;

using System;

public class RoomDoor : MonoBehaviour {

	public bool IsOpen;
	public DoorCloseType CloseType;
	public float CloseVelocity;
	public bool CompoundDoor;

	Vector3 _closedPosition;
	Vector3 _openPosition;
	public bool ChangeState = false; // Temp

	void Awake()
	{
		_closedPosition = transform.position;
	}

	// Update is called once per frame
	void Update () {	

		if (ChangeState)
		{
			Vector3 _localDirection = Vector3.zero;
			Vector3 _worldDirection = Vector3.zero;
			float _limitPosition = 0f;

			switch(CloseType)
			{
			case DoorCloseType.RIGHT:
				_worldDirection = transform.right;
				_localDirection = transform.InverseTransformVector(transform.right);
				_limitPosition = this.transform.lossyScale.x;
				break;
			case DoorCloseType.LEFT:
				_worldDirection = transform.right * -1;
				_localDirection = transform.InverseTransformVector(transform.right * -1);
				_limitPosition = this.transform.lossyScale.x;
				break;
			case DoorCloseType.UP:
				_worldDirection = transform.up;				
				_localDirection = transform.InverseTransformVector(transform.up);
				_limitPosition = this.transform.lossyScale.y;
				break;
			case DoorCloseType.DOWN:
				_worldDirection = transform.up * -1;				
				_localDirection = transform.InverseTransformVector(transform.up * -1);
				_limitPosition = this.transform.lossyScale.y;
				break;
			}

			if (CompoundDoor)
				_limitPosition /= 2;

			_localDirection.Normalize();
			_openPosition = _closedPosition + _worldDirection * _limitPosition;

			if (IsOpen)
			{
				transform.position = Vector3.Lerp(transform.position, _closedPosition, CloseVelocity);
				if (transform.position.V3Equal(_closedPosition))
				{
					ChangeState = false;
					IsOpen = false;
				}
			}
			else
			{
				transform.position = Vector3.Lerp(transform.position, _openPosition, CloseVelocity);
				if (transform.position.V3Equal(_openPosition))
				{
					ChangeState = false;
					IsOpen = true;
				}
			}
		}
	}

	public void ChangeDoorState(bool OpenDoor)
	{
		this.ChangeState =  !IsOpen && OpenDoor || IsOpen && !OpenDoor;
	}
}

public enum DoorCloseType
{
	UP 		= 0,
	LEFT 	= 1,
	RIGHT	= 2,
	DOWN	= 3,
}


