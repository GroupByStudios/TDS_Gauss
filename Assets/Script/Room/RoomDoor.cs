using UnityEngine;
using System.Collections;

using System;

public class RoomDoor : MonoBehaviour {

	public bool IsOpen;
	public DoorCloseType CloseType;
	public float CloseVelocity;
	public bool CompoundDoor;

	public bool Automatic = false;
	public Vector2 AutomaticTriggerVolume;

	public bool CompensateSize = false;

	Vector3 _closedPosition;
	Vector3 _openPosition;
	private bool ChangeState = false;
	private Bounds _bounds;

	void Awake()
	{
		_closedPosition = transform.position;

		_bounds = GetComponent<MeshRenderer>().bounds;
	}

	// Update is called once per frame
	void Update () {	

		if (Automatic)
		{
			CheckPlayerProximity();
		}

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
				_limitPosition = this.transform.lossyScale.x * this._bounds.extents.x;
				break;
			case DoorCloseType.LEFT:
				_worldDirection = transform.right * -1;
				_localDirection = transform.InverseTransformVector(transform.right * -1);
				_limitPosition = this.transform.lossyScale.x * this._bounds.extents.x;
				break;
			case DoorCloseType.UP:
				_worldDirection = transform.up;				
				_localDirection = transform.InverseTransformVector(transform.up);
				_limitPosition = this.transform.lossyScale.y * this._bounds.extents.y;
				break;
			case DoorCloseType.DOWN:
				_worldDirection = transform.up * -1;				
				_localDirection = transform.InverseTransformVector(transform.up * -1);
				_limitPosition = this.transform.lossyScale.y * this._bounds.extents.y;
				break;
			}

			_limitPosition *= 2;

			if (CompensateSize)
				_limitPosition += 0.85f;
			else
				_limitPosition -= 0.1f;

			if (CompoundDoor)
				_limitPosition /= 2;

			_localDirection.Normalize();
			_openPosition = _closedPosition + _worldDirection * _limitPosition;

			if (IsOpen)
			{
				transform.position = Vector3.Lerp(transform.position, _closedPosition, CloseVelocity);
				if (transform.position.V3Equal(_closedPosition))
				{
					transform.position = _closedPosition;
					ChangeState = false;
					IsOpen = false;
				}
			}
			else
			{
				transform.position = Vector3.Lerp(transform.position, _openPosition, CloseVelocity);
				if (transform.position.V3Equal(_openPosition))
				{
					transform.position = _openPosition;
					ChangeState = false;
					IsOpen = true;
				}
			}
		}
	}

	public void CheckPlayerProximity()
	{
		if (PlayerManager.Instance.IsPlayerInside(new Vector2(_closedPosition.x, _closedPosition.z), AutomaticTriggerVolume, false))
		{
			ChangeDoorState(true);
		}
		else
		{
			ChangeDoorState(false);
		}
	}

	public void ChangeDoorState(bool OpenDoor)
	{
		this.ChangeState =  !IsOpen && OpenDoor || IsOpen && !OpenDoor;
	}

	void OnDrawGizmos()
	{
		
		// Desenha o volume do ativador
		if (Automatic){

            if (!Application.isPlaying)
            {
                    _closedPosition = transform.position;
            }
			
			Gizmos.color = Color.red;
			Gizmos.DrawWireCube(_closedPosition, new Vector3(AutomaticTriggerVolume.x, 0, AutomaticTriggerVolume.y));
		}

	}
}

public enum DoorCloseType
{
	UP 		= 0,
	LEFT 	= 1,
	RIGHT	= 2,
	DOWN	= 3,
}


