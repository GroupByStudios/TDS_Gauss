using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour {

	public AudioClip musicaTheme;
	public AudioClip musicaCombate;
	AudioSource audio;
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

		audio = GetComponent<AudioSource> ();
		audio.clip = musicaTheme;
		audio.Play ();
		State = RoomManagerState.NotActivated;

		sizeX = this.RoomActivationBox.x * 0.9f / 2;
		sizeZ = this.RoomActivationBox.z * 0.9f / 2;
		MinX = this.transform.position.x - sizeX;
		MaxX = this.transform.position.x + sizeX;
		MinZ = this.transform.position.z - sizeZ;
		MaxZ = this.transform.position.z + sizeZ;
	}

	// Update is called once per frame
	void Update () {

		if (!audio.isPlaying) {
			audio.clip = musicaTheme;
			audio.Play ();
		}

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

			bool _finished = true;

			// Inicializa os spawners
			for(int i = 0; i < Spawners.Length; i++)
			{
				if (Spawners[i] != null)
				{
					switch(Spawners[i].State)
					{
					case RoomSpawnerState.NotActivate:
						Spawners [i].State = RoomSpawnerState.Activating;
						float volume = 1f;
						for (i = 100; i > 0; i--) {
							volume -= 0.01f;
							audio.volume = volume;
						}	
						audio.Stop ();
						audio.clip = musicaCombate;
						audio.Play ();
						audio.volume = 1f;
						break;
					default :break;
					}

					_finished = _finished && Spawners[i].State == RoomSpawnerState.Finished;
					if (_finished)
						this.State = RoomManagerState.Finished;
				}
			}

			break;
		case RoomManagerState.Finished:

			OpenDoors(ExitDoors);

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
