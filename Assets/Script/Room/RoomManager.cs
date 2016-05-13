using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour {

	public RoomManagerState State;
	public RoomDoor[] EnterDoors;
	public RoomDoor[] ExitDoors;
	public RoomSpawner[] Spawners;
	public Vector3 RoomSize;
	public Vector3 DoorActivationBox;
	public Vector3 RoomActivationBox;

	float sizeX;
	float sizeZ;
	[HideInInspector] public float MinX;
	[HideInInspector] public float MaxX;
	[HideInInspector] public float MinZ;
	[HideInInspector] public float MaxZ;

	void Awake() {

	}

	// Use this for initialization
	void Start () {

		State = RoomManagerState.NotActivated;

		sizeX = this.DoorActivationBox.x / 2;
		sizeZ = this.DoorActivationBox.z / 2;
		MinX = this.transform.position.x - sizeX;
		MaxX = this.transform.position.x + sizeX;
		MinZ = this.transform.position.z - sizeZ;
		MaxZ = this.transform.position.z + sizeZ;
	}
	
	// Update is called once per frame
	void Update () {

		switch(State)
		{
		case RoomManagerState.NotActivated:

			// Trocar para Eventos e Maquina de Estado
			if (PlayerManager.Instance.IsPlayerInside(new Vector2(transform.position.x, transform.position.z), new Vector2(DoorActivationBox.x, DoorActivationBox.z), false))
			{
				OpenDoors(EnterDoors);

				if (PlayerManager.Instance.IsPlayerInside(new Vector2(transform.position.x, transform.position.z), new Vector2(RoomActivationBox.x, RoomActivationBox.z), true))
				{				
					CloseDoors(EnterDoors);

					// ATIVA A SALA
					State = RoomManagerState.Activated;

				}
			}
			else
			{
				CloseDoors(EnterDoors);
			}

			break;
		case RoomManagerState.Activated:

			// Inicializa os spawners
			for(int i = 0; i < Spawners.Length; i++)
			{
				if (Spawners[i] != null)
				{
					switch(Spawners[i].State)
					{
					case RoomSpawnerState.NotActivate:
						Spawners[i].State = RoomSpawnerState.Activating;
						break;
					default :break;
					}
				}
				
			}

			break;
		case RoomManagerState.Finished:
			break;
		}
	}

	void LinkSpwaners()
	{
		for (int i = 0; i < Spawners.Length; i++)
		{
			if (Spawners[i] != null)
				Spawners[i].RoomManager = this;
		}
	}


	[ExposeMethodInEditorAttribute]
	public void OpenDoors(RoomDoor[] doors_)
	{
		for(int i = 0; i < doors_.Length; i++)
		{
			doors_[i].ChangeDoorState(true);
		}
	}

	[ExposeMethodInEditorAttribute]
	public void CloseDoors(RoomDoor[] doors_)
	{
		for(int i = 0; i < doors_.Length; i++)
		{
			doors_[i].ChangeDoorState(false);
		}	
	}

	void OnDrawGizmos()
	{
		// Desenha um quadrado com o tamanho da sala
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(transform.position, RoomSize);

		Gizmos.color = Color.magenta;
		Gizmos.DrawWireCube(transform.position, DoorActivationBox);

		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(transform.position, RoomActivationBox);

	}
}

public enum RoomManagerState
{
	NotActivated,
	Activated,
	Finished
}
	