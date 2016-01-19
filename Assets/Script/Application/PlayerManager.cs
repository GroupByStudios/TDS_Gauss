using UnityEngine;
using System.Collections;
using InControl;

public class PlayerManager : MonoBehaviour {

	public Player[] PlayerAvatar;
	[HideInInspector] public InputDevicePlayer[] InputDevicePlayers;

	public static PlayerManager Instance;

	void Start()
	{
		Instance =  this;

		for(int i = 0; i < PlayerAvatar.Length; i++)
		{
			PlayerAvatar[i] = Instantiate(PlayerAvatar[i]) as Player;
			PlayerAvatar[i].gameObject.SetActive(false);
		}

		InputDevicePlayers = new InputDevicePlayer[4]; // 4 Players;

		InputManager.OnDeviceDetached += InputManager_OnDeviceDetached;
		//InputManager.OnDeviceAttached += InputManager_OnDeviceAttached;
	}

	void InputManager_OnDeviceDetached (InputDevice obj)
	{
		int _inputDeviceIndex = IsJoystickAssignedToGame(obj);
		int _playerAssignedId = IsJoystickAssignedToPlayer(obj);

		if (_inputDeviceIndex > -1)
			InputDevicePlayers[_inputDeviceIndex] = null;

		if (_playerAssignedId > -1){
			PlayerAvatar[_playerAssignedId].PlayerInputController.InputDeviceJoystick = null; // TODO MELHORAR O METODO PARA REMOVER O JOGADOR
			PlayerAvatar[_playerAssignedId].gameObject.SetActive(false);
		}
		
	}

	void Update()
	{
		AttachInputDeviceToGame();
		CheckInputDeviceAction();
	}

	void AttachInputDeviceToGame()
	{
		int freeInputDevicePosition;
		for (int i = 0; i < InputManager.Devices.Count ; i++)
		{
			if (InputManager.Devices[i].IsKnown && InputManager.Devices[i].IsSupportedOnThisPlatform)
			{
				if (IsJoystickAssignedToPlayer(InputManager.Devices[i]) == -1)
				{
					if (IsJoystickAssignedToGame(InputManager.Devices[i]) == -1)
					{
						freeInputDevicePosition = GetFreeInputDevicePosition();

						if (freeInputDevicePosition > -1)
						{
							InputDevicePlayer _inputDevicePlayer = new InputDevicePlayer();
							_inputDevicePlayer.InputId = i;
							_inputDevicePlayer.IsSelectingClass = true;
							_inputDevicePlayer.IsAssignedToPlayer = false;
							_inputDevicePlayer.SelectingPlayerClassID = 0;
							_inputDevicePlayer.SetInputDevice(InputManager.Devices[i]);
							InputDevicePlayers[freeInputDevicePosition] = _inputDevicePlayer;
						}						
					}
				}
			}
		}
	}

	void CheckInputDeviceAction()
	{
		for (int i = 0; i < InputDevicePlayers.Length; i++)
		{
			if (InputDevicePlayers[i] != null){
				if (!InputDevicePlayers[i].IsAssignedToPlayer)
				{
					if (InputDevicePlayers[i].GetInputDevice().DPadRight.WasPressed)
					{
						InputDevicePlayers[i].SelectingPlayerClassID = GetFreePlayerClassId(InputDevicePlayers[i].SelectingPlayerClassID, true);
					}
					if (InputDevicePlayers[i].GetInputDevice().DPadLeft.WasPressed)
					{
						InputDevicePlayers[i].SelectingPlayerClassID = GetFreePlayerClassId(InputDevicePlayers[i].SelectingPlayerClassID, false);
					}

					if (InputDevicePlayers[i].GetInputDevice().Action1.WasPressed)
					{

						for (int j = 0; j < PlayerAvatar.Length; j++)
						{
							if (PlayerAvatar[j].PlayerClassID == InputDevicePlayers[i].SelectingPlayerClassID)
							{
								PlayerInput _playerInput = PlayerAvatar[j].gameObject.GetComponent<PlayerInput>();
								_playerInput.InputDeviceJoystick = InputDevicePlayers[i].GetInputDevice();

								InputDevicePlayers[i].IsAssignedToPlayer = true;
								InputDevicePlayers[i].IsSelectingClass = false;
								InputDevicePlayers[i].Avatar = PlayerAvatar[j];

								PlayerAvatar[j].gameObject.SetActive(true);

								break;
							}
						}
					}
				}
			}
		}
	}



	int GetFreeInputDevicePosition()
	{
		for(int i = 0; i < InputDevicePlayers.Length; i++)
		{
			if (InputDevicePlayers[i] == null)
				return i;
		}

		return  -1;
	}

	/// <summary>
	/// Verifica se o Device esta associado a algum jogador
	/// </summary>
	/// <returns><c>true</c> if this instance is joystick assigned to player the specified inputDevice_; otherwise, <c>false</c>.</returns>
	/// <param name="inputDevice_">Input device.</param>
	int IsJoystickAssignedToPlayer(InputDevice inputDevice_)
	{
		for(int i = 0; i < PlayerAvatar.Length; i++)
		{
			if (PlayerAvatar[i].gameObject.activeInHierarchy)
			{
				if (PlayerAvatar[i].PlayerInputController != null)
				{
					if (PlayerAvatar[i].PlayerInputController.InputDeviceJoystick != null){
						if (PlayerAvatar[i].PlayerInputController.InputDeviceJoystick.GetHashCode() == inputDevice_.GetHashCode())
							return i;
					}
				}
			}
		}
		return -1;
	}

	/// <summary>
	/// Verifica se o controle esta associado ao jogo de qualquer forma
	/// </summary>
	/// <returns><c>true</c> if this instance is joystick assigned to game the specified inputDevice_; otherwise, <c>false</c>.</returns>
	/// <param name="inputDevice_">Input device.</param>
	int IsJoystickAssignedToGame(InputDevice inputDevice_)
	{
		for(int i = 0; i < InputDevicePlayers.Length; i++)
		{
			if (InputDevicePlayers[i] != null	&& InputDevicePlayers[i].GetInputDevice().GetHashCode() == inputDevice_.GetHashCode())
				return i;
		}

		return -1;	
	}

	int GetFreePlayerClassId(int currentPlayerClassId, bool ascend_)
	{
		if (ascend_)
		{
			while(true)
			{
				currentPlayerClassId++;

				if (currentPlayerClassId > PlayerClass.PlayerClassCount - 1)
					currentPlayerClassId = 0;

				if (IsPlayerClassIdFree(currentPlayerClassId))
					break;
			}
		}
		else
		{
			while(true)
			{
				currentPlayerClassId--;

				if (currentPlayerClassId < 0)
					currentPlayerClassId = PlayerClass.PlayerClassCount - 1;							

				if (IsPlayerClassIdFree(currentPlayerClassId))
					break;
			}				
		}

		return currentPlayerClassId;
	}

	bool IsPlayerClassIdFree(int playerClassId_)
	{
		for(int i = 0; i < InputDevicePlayers.Length; i++)
		{
			if (InputDevicePlayers[i] != null)
			{
				if(InputDevicePlayers[i].IsSelectingClass)
				{
					if (InputDevicePlayers[i].SelectingPlayerClassID == playerClassId_)
						return false;
				}
				else if (InputDevicePlayers[i].IsAssignedToPlayer)
				{
					if (InputDevicePlayers[i].Avatar.PlayerClassID == playerClassId_)
						return false;
				}
			}
		}

		return true;
	}

	#region OnGUI Methods

	void OnGUI()
	{
			for(int i = 0; i < InputDevicePlayers.Length; i++)
		{
			if (InputDevicePlayers[i] != null)
			{
				if (InputDevicePlayers[i].IsSelectingClass){
					DrawPlayerAvatarChoice(InputDevicePlayers[i]);
				}
			}
		}
	}

	void DrawPlayerAvatarChoice(InputDevicePlayer inputDevicePlayer_)
	{
		float Size_Width = 200f;
		float Line_Size = 15f;
		Vector2 CurrentPosition = new Vector2(10f + (Size_Width *  inputDevicePlayer_.InputId), 100f);
		Vector2 CurrentSize = new Vector2(Size_Width, Size_Width / 2);

		GUI.Label(new Rect(CurrentPosition, CurrentSize), "Escolha um Jogador");
		CurrentPosition.y += Line_Size;
		GUI.Label(new Rect(CurrentPosition, CurrentSize), PlayerClass.ClassName[inputDevicePlayer_.SelectingPlayerClassID]);
	}

	#endregion
}


public class InputDevicePlayer
{
	public int InputId;
	public int SelectingPlayerClassID;
	public bool IsSelectingClass;
	public bool IsAssignedToPlayer;
	public Player Avatar;

	private InputDevice inputDevice;

	public InputDevicePlayer()
	{
		SelectingPlayerClassID = -1;
	}

	public void SetInputDevice(InputDevice inputDevice_)
	{
		inputDevice = inputDevice_;
	}

	public InputDevice GetInputDevice()
	{
		return inputDevice;
	}

}


public class PlayerClass
{
	public const int Medic = 0;
	public const int Defender = 1;
	public const int Engineer = 2;
	public const int Assault = 3;
	public const int Specialist = 4;

	public const int PlayerClassCount = 5;

	public static string[] ClassName = new string[PlayerClassCount]{
		"Medic",
		"Defender",
		"Engenheiro",
		"Assault",
		"Specialist"
	};
}


