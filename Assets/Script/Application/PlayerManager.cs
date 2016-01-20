using UnityEngine;
using System.Collections;
using InControl;

public class PlayerManager : MonoBehaviour {

	public Player[] myPlayerAvatarList;
	public Color[] myPlayerColorList;
	public PlayerStatusHUD[] myPlayerStatusHUDList;
	[HideInInspector] public InputDevicePlayer[] myInputDevicePlayers;

	public static PlayerManager Instance;

	void Start()
	{
		Instance =  this;

		for(int i = 0; i < myPlayerAvatarList.Length; i++)
		{
			myPlayerAvatarList[i] = Instantiate(myPlayerAvatarList[i]) as Player;
			myPlayerAvatarList[i].gameObject.SetActive(false);
		}

		myInputDevicePlayers = new InputDevicePlayer[4]; // 4 Players;

		InputManager.OnDeviceDetached += InputManager_OnDeviceDetached;
		//InputManager.OnDeviceAttached += InputManager_OnDeviceAttached;
	}

	void InputManager_OnDeviceDetached (InputDevice obj)
	{
		int _inputDeviceIndex = IsJoystickAssignedToGame(obj);
		int _playerAssignedId = IsJoystickAssignedToPlayer(obj);

		if (_inputDeviceIndex > -1){
			myInputDevicePlayers[_inputDeviceIndex] = null;
		}

		if (_playerAssignedId > -1){
			myPlayerStatusHUDList[_inputDeviceIndex].myPlayer = null;
			myPlayerStatusHUDList[_inputDeviceIndex].gameObject.SetActive(false);

			myPlayerAvatarList[_playerAssignedId].PlayerInputController.InputDeviceJoystick = null; // TODO MELHORAR O METODO PARA REMOVER O JOGADOR
			myPlayerAvatarList[_playerAssignedId].gameObject.SetActive(false);
		}
		
	}

	void Update()
	{
		AttachInputDeviceToGame();
		CheckInputDeviceAction();
	}

	/// <summary>
	/// Associa um determinado controle ao Jogo
	/// </summary>
	void AttachInputDeviceToGame()
	{
		int freeInputDevicePosition;
		UnityInputDevice _currentDevice = null;

		for (int i = 0; i < InputManager.Devices.Count ; i++)
		{
			_currentDevice = InputManager.Devices[i] as UnityInputDevice;

			if (_currentDevice != null && _currentDevice.IsKnown && _currentDevice.IsSupportedOnThisPlatform)
			{
				if (IsJoystickAssignedToPlayer(_currentDevice) == -1)
				{
					if (IsJoystickAssignedToGame(_currentDevice) == -1)
					{
						freeInputDevicePosition = GetFreeInputDevicePosition();

						if (freeInputDevicePosition > -1)
						{
							InputDevicePlayer _inputDevicePlayer = new InputDevicePlayer();
							_inputDevicePlayer.InputId = _currentDevice.JoystickId;
							_inputDevicePlayer.IsSelectingClass = true;
							_inputDevicePlayer.IsAssignedToPlayer = false;
							_inputDevicePlayer.SelectingPlayerClassID = 0;
							_inputDevicePlayer.myInputDevice = _currentDevice;
							myInputDevicePlayers[freeInputDevicePosition] = _inputDevicePlayer;
						}						
					}
				}
			}
		}
	}

	/// <summary>
	/// Verifica as acoes dos controles que ainda nao estao associados a um jogador
	/// </summary>
	void CheckInputDeviceAction()
	{
		int _nextClass = 0;
		bool _choosen = false;

		for (int i = 0; i < myInputDevicePlayers.Length; i++)
		{
			if (myInputDevicePlayers[i] != null){
				if (!myInputDevicePlayers[i].IsAssignedToPlayer)
				{
					_nextClass = 0;
					_choosen = false;

					if (myInputDevicePlayers[i].myInputDevice.Profile is CustomProfileKeyboardAndMouse)
					{
						if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
							_nextClass = 1;
						else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
							_nextClass = -1;
						else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
							_choosen = true;
					}
					else
					{
						if (myInputDevicePlayers[i].myInputDevice.DPadRight.WasPressed)
							_nextClass = 1;
						else if (myInputDevicePlayers[i].myInputDevice.DPadLeft.WasPressed)
							_nextClass = -1;
						else if (myInputDevicePlayers[i].myInputDevice.Action1.WasPressed)
							_choosen = true;
					}


					if (_nextClass == 1)
					{
						myInputDevicePlayers[i].SelectingPlayerClassID = GetFreePlayerClassId(myInputDevicePlayers[i].SelectingPlayerClassID, true);
					}
					if (_nextClass == -1)
					{
						myInputDevicePlayers[i].SelectingPlayerClassID = GetFreePlayerClassId(myInputDevicePlayers[i].SelectingPlayerClassID, false);
					}

					if (_choosen)
					{

						for (int j = 0; j < myPlayerAvatarList.Length; j++)
						{
							if (myPlayerAvatarList[j].PlayerClassID == myInputDevicePlayers[i].SelectingPlayerClassID)
							{
								PlayerInput _playerInput = myPlayerAvatarList[j].gameObject.GetComponent<PlayerInput>();
								_playerInput.InputDeviceJoystick = myInputDevicePlayers[i].myInputDevice;

								myInputDevicePlayers[i].IsAssignedToPlayer = true;
								myInputDevicePlayers[i].IsSelectingClass = false;
								myInputDevicePlayers[i].Avatar = myPlayerAvatarList[j];

								myPlayerAvatarList[j].gameObject.SetActive(true);

								myPlayerStatusHUDList[i].myPlayer = myPlayerAvatarList[j];
								myPlayerStatusHUDList[i].gameObject.SetActive(true);

								break;
							}
						}
					}
				}
			}
		}
	}
		
	/// <summary>
	/// Recupera uma posicao livre para colocar o controle conectado
	/// </summary>
	/// <returns>The free input device position.</returns>
	int GetFreeInputDevicePosition()
	{
		for(int i = 0; i < myInputDevicePlayers.Length; i++)
		{
			if (myInputDevicePlayers[i] == null)
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
		for(int i = 0; i < myPlayerAvatarList.Length; i++)
		{
			if (myPlayerAvatarList[i].gameObject.activeInHierarchy)
			{
				if (myPlayerAvatarList[i].PlayerInputController != null)
				{
					if (myPlayerAvatarList[i].PlayerInputController.InputDeviceJoystick != null){
						if (myPlayerAvatarList[i].PlayerInputController.InputDeviceJoystick.GetHashCode() == inputDevice_.GetHashCode())
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
		for(int i = 0; i < myInputDevicePlayers.Length; i++)
		{
			if (myInputDevicePlayers[i] != null	&& myInputDevicePlayers[i].myInputDevice.GetHashCode() == inputDevice_.GetHashCode())
				return i;
		}

		return -1;	
	}

	/// <summary>
	/// Recupera uma classe disponivel para uso
	/// </summary>
	/// <returns>The free player class identifier.</returns>
	/// <param name="currentPlayerClassId">Current player class identifier.</param>
	/// <param name="ascend_">If set to <c>true</c> ascend.</param>
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

	/// <summary>
	/// Verifica se uma determinada classe esta livre para ser utilizada
	/// </summary>
	/// <returns><c>true</c> if this instance is player class identifier free the specified playerClassId_; otherwise, <c>false</c>.</returns>
	/// <param name="playerClassId_">Player class identifier.</param>
	bool IsPlayerClassIdFree(int playerClassId_)
	{
		for(int i = 0; i < myInputDevicePlayers.Length; i++)
		{
			if (myInputDevicePlayers[i] != null)
			{
				if(myInputDevicePlayers[i].IsSelectingClass)
				{
					if (myInputDevicePlayers[i].SelectingPlayerClassID == playerClassId_)
						return false;
				}
				else if (myInputDevicePlayers[i].IsAssignedToPlayer)
				{
					if (myInputDevicePlayers[i].Avatar.PlayerClassID == playerClassId_)
						return false;
				}
			}
		}

		return true;
	}

	#region OnGUI Methods

	void OnGUI()
	{
		for(int i = 0; i < myInputDevicePlayers.Length; i++)
		{
			if (myInputDevicePlayers[i] != null)
			{
				if (myInputDevicePlayers[i].IsSelectingClass){
					DrawPlayerAvatarChoice(myInputDevicePlayers[i]);
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

	private UnityInputDevice _myInputDevice;

	public UnityInputDevice myInputDevice
	{
		get{ 
			return _myInputDevice; 
		}
		set
		{
			_myInputDevice = value;
		}
	}

	public InputDevicePlayer()
	{
		SelectingPlayerClassID = -1;
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


