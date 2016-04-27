using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour {

	public RoomDoor[] Doors;
	public Vector3 RoomSize;
	public Vector3 DoorActivationBox;
	public Vector3 RoomActivationBox;

	void Awake() {

	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {


		// Trocar para Eventos e Maquina de Estado
		if (IsPlayerInside(new Vector2(transform.position.x, transform.position.z), new Vector2(DoorActivationBox.x, DoorActivationBox.z), false))
		{
			OpenDoors();

			if (IsPlayerInside(new Vector2(transform.position.x, transform.position.z), new Vector2(RoomActivationBox.x, RoomActivationBox.z), true))
			{				
				CloseDoors();

				// ATIVA A SALA
			}
		}
		else
		{
			CloseDoors();
		}
	}


	public bool IsPlayerInside(Vector2 position_, Vector2 size_, bool allPlayers_)
	{
		bool _allPlayersInside = false;
		Rect _rectCalc = new Rect(new Vector2(position_.x - size_.x/2, position_.y - size_.y/2), size_);
		List<Player> _activePlayers = PlayerManager.Instance.ActivePlayers;

		for (int i = 0; i < _activePlayers.Count; i++)
		{
			// Verifica se o jogador esta dentro da area
			_allPlayersInside = _rectCalc.Contains(new Vector2(_activePlayers[i].transform.position.x, _activePlayers[i].transform.position.z));

			// Se nem todos os jogadores devem estar na area e pelo menos um jogador esta na area, retorna verdadeiro
			if (!allPlayers_ && _allPlayersInside)
				return true;

			// Se todos os jogadores estavam na area e pelo menos um jogar nao estiver na area, retorna falso
			if (allPlayers_ && !_allPlayersInside)
				return false;
		}

		// Se todos os jogadores devem estar na area espera pelo processamento de todas as posicoes e retorna
		return _allPlayersInside;
	}



	[ExposeMethodInEditorAttribute]
	public void OpenDoors()
	{
		for(int i = 0; i < Doors.Length; i++)
		{
			Doors[i].ChangeDoorState(true);
		}
	}

	[ExposeMethodInEditorAttribute]
	public void CloseDoors()
	{
		for(int i = 0; i < Doors.Length; i++)
		{
			Doors[i].ChangeDoorState(false);
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
