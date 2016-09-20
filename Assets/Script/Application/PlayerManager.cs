using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using InControl;
using UnityEngine.SceneManagement;
using System;

public class PlayerManager : MonoBehaviour
{
	public AudioClip[] sound; 
	public AudioSource audio;
    public Player[] myPlayerAvatarList;
    public Color[] myPlayerColorList;
    public PlayerStatusHUD[] myPlayerStatusHUDList;
    public LayerMask PlayerSpawnLayer;
    public float GameStartCountDown = 5f;
    public Text GameStartCountDownText;
    public Text GameStartText;
    public ENUMERATORS.Player.PlayerClass[] PlayerSelectionOrder;

    [HideInInspector]
    public InputDevicePlayer[] myInputDevicePlayers;
    [HideInInspector]
    public List<Player> ActivePlayers = new List<Player>();

    public static PlayerManager Instance;

    private float currentGameStartCountDown;

    #region Metodos para utilizacao dentro do jogo

    /// <summary>
    /// Retorn uma lista com os jogadores ativos
    /// </summary>
    /// <returns>The active players.</returns>
    private void UpdateActivePlayers()
    {
        ActivePlayers.Clear();

        for (int i = 0; i < myPlayerAvatarList.Length; i++)
        {
            if (myPlayerAvatarList[i].gameObject.activeInHierarchy)
            {
                ActivePlayers.Add(myPlayerAvatarList[i]);
            }
        }

    }

    /// <summary>
    /// Determines whether this instance is player inside the specified position_ size_ allPlayers_.
    /// </summary>
    /// <returns><c>true</c> if this instance is player inside the specified position_ size_ allPlayers_; otherwise, <c>false</c>.</returns>
    /// <param name="position_">Position.</param>
    /// <param name="size_">Size.</param>
    /// <param name="allPlayers_">If set to <c>true</c> all players.</param>
    public bool IsPlayerInside(Vector2 position_, Vector2 size_, bool allPlayers_)
    {
        bool _allPlayersInside = false;
        Rect _rectCalc = new Rect(new Vector2(position_.x - size_.x / 2, position_.y - size_.y / 2), size_);
        List<Player> _activePlayers = ActivePlayers;

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

    public static Player PlayerWithMoreAggro(Player[] players_)
    {
        Player _playerWithMoreAggro = null;

        for (int i = 0; i < players_.Length; i++)
        {
            if (players_[i] != null)
            {
                if (_playerWithMoreAggro == null)
                {
                    _playerWithMoreAggro = players_[i];
                    continue;
                }

                if (players_[i].Aggro.MaxWithModifiers > _playerWithMoreAggro.Aggro.MaxWithModifiers)
                {
                    _playerWithMoreAggro = players_[i];
                }
            }
        }

        return _playerWithMoreAggro;
    }

    #endregion

    void Start()
    {
        Instance = this;
		audio = GetComponent<AudioSource> ();
		audio.clip = sound[0];

        for (int i = 0; i < myPlayerAvatarList.Length; i++)
        {
            myPlayerAvatarList[i] = Instantiate(myPlayerAvatarList[i]) as Player;
            myPlayerAvatarList[i].gameObject.SetActive(false);
        }

        myInputDevicePlayers = new InputDevicePlayer[4]; // 4 Players;

        InputManager.OnDeviceDetached += InputManager_OnDeviceDetached;
        //InputManager.OnDeviceAttached += InputManager_OnDeviceAttached;
    }

    void InputManager_OnDeviceDetached(InputDevice obj)
    {
        int _inputDeviceIndex = IsJoystickAssignedToGame(obj);
        int _playerAssignedId = IsJoystickAssignedToPlayer(obj);

        if (_inputDeviceIndex > -1)
        {
			if (ApplicationModel.Instance.MenuCharacter[myInputDevicePlayers[_inputDeviceIndex].AvatarClassId] != null)
				ApplicationModel.Instance.MenuCharacter[myInputDevicePlayers[_inputDeviceIndex].AvatarClassId].IsSelected = false;

            myInputDevicePlayers[_inputDeviceIndex] = null;
        }

        if (_playerAssignedId > -1)
        {
			myPlayerStatusHUDList[_playerAssignedId].myPlayer = null;
			myPlayerStatusHUDList[_playerAssignedId].gameObject.SetActive(false);
			myPlayerStatusHUDList[_playerAssignedId].transform.parent.gameObject.SetActive(false);

            myPlayerAvatarList[_playerAssignedId].PlayerInputController.InputDeviceJoystick = null; // TODO MELHORAR O METODO PARA REMOVER O JOGADOR
            myPlayerAvatarList[_playerAssignedId].gameObject.SetActive(false);
        }

        UpdateActivePlayers();
    }



    void Update()
    {
        switch (ApplicationModel.Instance.State)
        {
            case GameState.PressStartMenu:
                AttachInputDeviceToGame();
                break;
            case GameState.CharacterSelection:
                AttachInputDeviceToGame();
                CheckInpuDeviceActionCharacterSelection();
                CharacterSelectionCountDown();
                break;
            case GameState.StartGame:
                SpawnPlayers();
                ApplicationModel.Instance.State = GameState.InGame;
                break;
            case GameState.InGame:
                AttachInputDeviceToGame();
                CheckInputDeviceActionInGame();
                break;
        }
    }

    private void SpawnPlayers()
    {
        for (int i = 0; i < myInputDevicePlayers.Length; i++)
        {
            if (myInputDevicePlayers[i] != null && myInputDevicePlayers[i].IsAssignedToPlayer && myInputDevicePlayers[i].Avatar == null)
            {
                for (int j = 0; j < myPlayerAvatarList.Length; j++)
                {
                    if (myPlayerAvatarList[j].PlayerClass == myInputDevicePlayers[i].SelectingPlayerClassID)
                    {
                        PlayerInput _playerInput = myPlayerAvatarList[j].gameObject.GetComponent<PlayerInput>();
                        _playerInput.InputDeviceJoystick = myInputDevicePlayers[i].myInputDevice;

                        myInputDevicePlayers[i].IsAssignedToPlayer = true;
                        myInputDevicePlayers[i].IsSelectingClass_InGame = false;
                        myInputDevicePlayers[i].Avatar = myPlayerAvatarList[j];

                        myPlayerAvatarList[j].gameObject.SetActive(true);

                        PlayerStatusHUD _hud = GetPlayerStatusHud(myPlayerAvatarList[j].PlayerClass);
                        _hud.myPlayer = myPlayerAvatarList[j];
                        _hud.gameObject.SetActive(true);
                        _hud.transform.parent.gameObject.SetActive(true);

                        /* Faz o Spawn do Jogador para o Centro da Camera */
                        RaycastHit _hit;
                        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out _hit, 50f, PlayerSpawnLayer))
                        {
                            myPlayerAvatarList[j].transform.position = _hit.point;
                        }

                        UpdateActivePlayers();
                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Associa um determinado controle ao Jogo
    /// </summary>
    void AttachInputDeviceToGame()
    {
        int freeInputDevicePosition;
        UnityInputDevice _currentDevice = null;

        for (int i = 0; i < InputManager.Devices.Count; i++)
        {
            _currentDevice = InputManager.Devices[i] as UnityInputDevice;

            if (_currentDevice != null && _currentDevice.IsKnown /* && _currentDevice.IsSupportedOnThisPlatform */ )
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

                            if (ApplicationModel.Instance.State == GameState.InGame)
                                _inputDevicePlayer.IsSelectingClass_InGame = true;

                            _inputDevicePlayer.IsAssignedToPlayer = false;
                            _inputDevicePlayer.SelectingPlayerClassID = GetFreePlayerClassId(ENUMERATORS.Player.PlayerClass.MEDIC, true);
                            _inputDevicePlayer.myInputDevice = _currentDevice;
                            myInputDevicePlayers[freeInputDevicePosition] = _inputDevicePlayer;
                        }
                    }
                }
            }
        }
    }

    void CheckInpuDeviceActionCharacterSelection()
    {
        for (int i = 0; i < ApplicationModel.Instance.MenuCharacter.Length; i++)
        {
            if (!ApplicationModel.Instance.MenuCharacter[i].IsSelected)
				ApplicationModel.Instance.MenuCharacter[i].SelectCharacter(false, true, 0);
        }

        for (int i = 0; i < myInputDevicePlayers.Length; i++)
        {
            if (myInputDevicePlayers[i] != null)
            {
                if (!myInputDevicePlayers[i].IsAssignedToPlayer)
                {
                    int _nextClass = 0;
                    bool _choosen = false;


                    if (myInputDevicePlayers[i].myInputDevice.Profile is CustomProfileKeyboardAndMouse)
                    {
						if (Input.GetKeyDown (KeyCode.D) || Input.GetKeyDown (KeyCode.RightArrow)) {
							_nextClass = 1;
							audio.clip = sound [0];
							audio.Play() ;
						} else if (Input.GetKeyDown (KeyCode.A) || Input.GetKeyDown (KeyCode.LeftArrow)) {
							_nextClass = -1;
							audio.clip = sound [0];
							audio.Play ();
						} else if (Input.GetKeyDown (KeyCode.Return) || Input.GetKeyDown (KeyCode.KeypadEnter)) {
							_choosen = true;
							switch(myInputDevicePlayers[i].SelectingPlayerClassID.ToString()){

							case "ENGINEER":
								audio.clip = sound [1];
								audio.Play ();
								break;
							case "SPECIALIST":
								audio.clip = sound [2];
								audio.Play ();
								break;
							case "DEFENDER":
								audio.clip = sound [3];
								audio.Play ();
								break;
							case "MEDIC":
								audio.clip = sound [4];
								audio.Play ();
								break;
							case "ASSAULT":
								audio.clip = sound [5];
								audio.Play ();
								break;
							}
						}
                    }
                    else
                    {
						if (myInputDevicePlayers [i].myInputDevice.DPadRight.WasPressed) {
							_nextClass = 1;
							audio.clip = sound [0];
							audio.Play ();
						} else if (myInputDevicePlayers [i].myInputDevice.DPadLeft.WasPressed) {
							_nextClass = -1;
							audio.clip = sound [0];
							audio.Play ();
						} else if (myInputDevicePlayers [i].myInputDevice.Action1.WasPressed) {
							_choosen = true;
		
							switch(myInputDevicePlayers[i].SelectingPlayerClassID.ToString()){

							case "ENGINEER":
								audio.clip = sound [1];
								audio.Play ();
								break;
							case "SPECIALIST":
								audio.clip = sound [2];
								audio.Play ();
								break;
							case "DEFENDER":
								audio.clip = sound [3];
								audio.Play ();
								break;
							case "MEDIC":
								audio.clip = sound [4];
								audio.Play ();
								break;
							case "ASSAULT":
								audio.clip = sound [5];
								audio.Play ();
								break;
							}


						}
                    }

                    if (_nextClass == 1)
                    {
                        myInputDevicePlayers[i].SelectingPlayerClassID = GetFreePlayerClassId(myInputDevicePlayers[i].SelectingPlayerClassID, true);
                    }
                    if (_nextClass == -1)
                    {
                        myInputDevicePlayers[i].SelectingPlayerClassID = GetFreePlayerClassId(myInputDevicePlayers[i].SelectingPlayerClassID, false);
                    }

                    int characterId = (int)myInputDevicePlayers[i].SelectingPlayerClassID;

                    if (characterId > -1 && characterId < ApplicationModel.Instance.MenuCharacter.Length)
						ApplicationModel.Instance.MenuCharacter[characterId].SelectCharacter(true, myInputDevicePlayers[i].myInputDevice.Profile is CustomProfileKeyboardAndMouse, i+1);

                    if (_choosen)
                    {
                        myInputDevicePlayers[i].IsAssignedToPlayer = true;
                        myInputDevicePlayers[i].AvatarClassId = characterId;
                        ApplicationModel.Instance.MenuCharacter[characterId].IsSelected = true;
                    }
                }
                else
                {
                    if (myInputDevicePlayers[i].myInputDevice.Profile is CustomProfileKeyboardAndMouse)
                    {
                        if (Input.GetKeyDown(KeyCode.Escape))
                        {
                            myInputDevicePlayers[i].IsAssignedToPlayer = false;
                            ApplicationModel.Instance.MenuCharacter[myInputDevicePlayers[i].AvatarClassId].IsSelected = false;
                        }
                    }
                    else
                    {
                        if (myInputDevicePlayers[i].myInputDevice.Action3)
                        {
                            myInputDevicePlayers[i].IsAssignedToPlayer = false;
                            ApplicationModel.Instance.MenuCharacter[myInputDevicePlayers[i].AvatarClassId].IsSelected = false;
                        }
                    }

                }
            }
        }
    }

    void CharacterSelectionCountDown()
    {
        bool isAllPlayerAssigned = true;
        // Is everyone assigned? Start Countdown
        for (short i = 0; i < myInputDevicePlayers.Length; i++)
        {
            if (myInputDevicePlayers[i] != null)
            {
                isAllPlayerAssigned = myInputDevicePlayers[i].IsAssignedToPlayer && isAllPlayerAssigned;
            }
        }

        if (isAllPlayerAssigned)
        {
            if (GameStartText != null)
            {
                GameStartText.gameObject.SetActive(true);
            }

            if (GameStartCountDownText != null)
            {
                GameStartCountDownText.gameObject.SetActive(true);
                GameStartCountDownText.text = currentGameStartCountDown.ToString("0");
            }

            currentGameStartCountDown -= Time.deltaTime;

            if (currentGameStartCountDown <= 0)
            {
                currentGameStartCountDown = GameStartCountDown;
                GameStartCountDownText.gameObject.SetActive(false);
                GameStartText.gameObject.SetActive(false);
                ApplicationModel.Instance.State = GameState.StartGame;
                SceneManager.LoadScene("NV_99");
            }
        }
        else
        {
            currentGameStartCountDown = GameStartCountDown;
            GameStartCountDownText.gameObject.SetActive(false);
            GameStartText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Verifica as acoes dos controles que ainda nao estao associados a um jogador
    /// </summary>
    void CheckInputDeviceActionInGame()
    {
        int _nextClass = 0;
        bool _choosen = false;

        for (int i = 0; i < myInputDevicePlayers.Length; i++)
        {
            if (myInputDevicePlayers[i] != null)
            {
                if (!myInputDevicePlayers[i].IsAssignedToPlayer)
                {
                    _nextClass = 0;
                    _choosen = false;

                    if (myInputDevicePlayers[i].myInputDevice.Profile is CustomProfileKeyboardAndMouse)
                    {
						if (Input.GetKeyDown (KeyCode.D) || Input.GetKeyDown (KeyCode.RightArrow)) {
							_nextClass = 1;
							audio.clip = sound [0];
							audio.Play ();
						} else if (Input.GetKeyDown (KeyCode.A) || Input.GetKeyDown (KeyCode.LeftArrow)) {
							_nextClass = -1;
							audio.clip = sound [0];
							audio.Play ();
						} else if (Input.GetKeyDown (KeyCode.Return) || Input.GetKeyDown (KeyCode.KeypadEnter)) {
							_choosen = true;
							switch(myInputDevicePlayers[i].SelectingPlayerClassID.ToString()){

							case "ENGINEER":
								audio.clip = sound [1];
								audio.Play ();
								break;
							case "SPECIALIST":
								audio.clip = sound [2];
								audio.Play ();
								break;
							case "DEFENDER":
								audio.clip = sound [3];
								audio.Play ();
								break;
							case "MEDIC":
								audio.clip = sound [4];
								audio.Play ();
								break;
							case "ASSAULT":
								audio.clip = sound [5];
								audio.Play ();
								break;
							}

						}
                    }
                    else
                    {
						if (myInputDevicePlayers [i].myInputDevice.DPadRight.WasPressed) {
							_nextClass = 1;
							audio.clip = sound[0];
							audio.Play ();
						} else if (myInputDevicePlayers [i].myInputDevice.DPadLeft.WasPressed) {
							_nextClass = -1;
							audio.clip = sound[0];
							audio.Play ();
						} else if (myInputDevicePlayers [i].myInputDevice.Action1.WasPressed) {
							_choosen = true;
							switch(myInputDevicePlayers[i].SelectingPlayerClassID.ToString()){

							case "ENGINEER":
								audio.clip = sound [1];
								audio.Play ();
								break;
							case "SPECIALIST":
								audio.clip = sound [2];
								audio.Play ();
								break;
							case "DEFENDER":
								audio.clip = sound [3];
								audio.Play ();
								break;
							case "MEDIC":
								audio.clip = sound [4];
								audio.Play ();
								break;
							case "ASSAULT":
								audio.clip = sound [5];
								audio.Play ();
								break;
							}

						}
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
                        myInputDevicePlayers[i].IsAssignedToPlayer = true;

                        for (int j = 0; j < myPlayerAvatarList.Length; j++)
                        {
                            if (myPlayerAvatarList[j].PlayerClass == myInputDevicePlayers[i].SelectingPlayerClassID)
                            {
                                PlayerInput _playerInput = myPlayerAvatarList[j].gameObject.GetComponent<PlayerInput>();
                                _playerInput.InputDeviceJoystick = myInputDevicePlayers[i].myInputDevice;

                                myInputDevicePlayers[i].IsAssignedToPlayer = true;
                                myInputDevicePlayers[i].IsSelectingClass_InGame = false;
                                myInputDevicePlayers[i].Avatar = myPlayerAvatarList[j];

                                myPlayerAvatarList[j].gameObject.SetActive(true);


                                PlayerStatusHUD _hud = GetPlayerStatusHud(myPlayerAvatarList[j].PlayerClass);
                                _hud.myPlayer = myPlayerAvatarList[j];
                                _hud.gameObject.SetActive(true);
                                _hud.transform.parent.gameObject.SetActive(true);

                                /* Faz o Spawn do Jogador para o Centro da Camera */
                                RaycastHit _hit;
                                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out _hit, 50f, PlayerSpawnLayer))
                                {
                                    myPlayerAvatarList[j].transform.position = _hit.point;
                                }

                                UpdateActivePlayers();

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
        for (int i = 0; i < myInputDevicePlayers.Length; i++)
        {
            if (myInputDevicePlayers[i] == null)
                return i;
        }

        return -1;
    }

    PlayerStatusHUD GetPlayerStatusHud(ENUMERATORS.Player.PlayerClass playerClass_)
    {
        for (int i = 0; i < myPlayerStatusHUDList.Length; i++)
        {
            if (myPlayerStatusHUDList[i] != null && myPlayerStatusHUDList[i].Playerclass == playerClass_)
                return myPlayerStatusHUDList[i];
        }

        return null;
    }

    /// <summary>
    /// Verifica se o Device esta associado a algum jogador
    /// </summary>
    /// <returns><c>true</c> if this instance is joystick assigned to player the specified inputDevice_; otherwise, <c>false</c>.</returns>
    /// <param name="inputDevice_">Input device.</param>
    int IsJoystickAssignedToPlayer(InputDevice inputDevice_)
    {
        for (int i = 0; i < myPlayerAvatarList.Length; i++)
        {
            if (myPlayerAvatarList[i].gameObject.activeInHierarchy)
            {
                if (myPlayerAvatarList[i].PlayerInputController != null)
                {
                    if (myPlayerAvatarList[i].PlayerInputController.InputDeviceJoystick != null)
                    {
                        if (myPlayerAvatarList[i].PlayerInputController.InputDeviceJoystick.GetHashCode() == inputDevice_.GetHashCode())
							return (int)myPlayerAvatarList[i].PlayerClass;
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
        for (int i = 0; i < myInputDevicePlayers.Length; i++)
        {
            if (myInputDevicePlayers[i] != null && myInputDevicePlayers[i].myInputDevice.GetHashCode() == inputDevice_.GetHashCode())
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
    ENUMERATORS.Player.PlayerClass GetFreePlayerClassId(ENUMERATORS.Player.PlayerClass currentPlayerClassId, bool ascend_)
    {
        if (ascend_)
        {
            while (true)
            {
                // Percorre as classes ordenadas
                for (int i = 0; i < PlayerSelectionOrder.Length; i++)
                {
                    // Encontrou a classe atual
                    if (currentPlayerClassId == PlayerSelectionOrder[i])
                    {
                        // Faz o loop ou seleciona a proxima da ordem
                        if (i + 1 == PlayerSelectionOrder.Length)
                            currentPlayerClassId = PlayerSelectionOrder[0];
                        else
                            currentPlayerClassId = PlayerSelectionOrder[i + 1];
						
						break;
                    }
                }

                if (IsPlayerClassIdFree(currentPlayerClassId))
                    break;
            }
        }
        else
        {
            while (true)
            {
                // Percorre as classes ordenadas
                for (int i = 0; i < PlayerSelectionOrder.Length; i++)
                {
                    // Encontrou a classe atual
                    if (currentPlayerClassId == PlayerSelectionOrder[i])
                    {
                        // Faz o loop ou seleciona a proxima da ordem
                        if (i - 1 == -1)
                            currentPlayerClassId = PlayerSelectionOrder[PlayerSelectionOrder.Length - 1];
                        else
                            currentPlayerClassId = PlayerSelectionOrder[i - 1];

						break;
                    }
                }

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
    bool IsPlayerClassIdFree(ENUMERATORS.Player.PlayerClass playerClassId_)
    {
        for (int i = 0; i < myInputDevicePlayers.Length; i++)
        {
            if (myInputDevicePlayers[i] != null)
            {
                if (myInputDevicePlayers[i].IsSelectingClass_InGame)
                {
                    if (myInputDevicePlayers[i].SelectingPlayerClassID == playerClassId_)
                        return false;
                }
                else
                {
                    if (myInputDevicePlayers[i].Avatar != null && myInputDevicePlayers[i].Avatar.PlayerClass == playerClassId_)
                        return false;
                    else if (myInputDevicePlayers[i].SelectingPlayerClassID == playerClassId_)
                        return false;
                }
            }
        }

        return true;
    }

    #region OnGUI Methods

    void OnGUI()
    {
        for (int i = 0; i < myInputDevicePlayers.Length; i++)
        {
            if (myInputDevicePlayers[i] != null)
            {
                if (myInputDevicePlayers[i].IsSelectingClass_InGame)
                {
                    DrawPlayerAvatarChoice(myInputDevicePlayers[i]);
                }
            }
        }
    }

    void DrawPlayerAvatarChoice(InputDevicePlayer inputDevicePlayer_)
    {
        float Size_Width = 200f;
        float Line_Size = 15f;
        Vector2 CurrentPosition = new Vector2(10f + (Size_Width * inputDevicePlayer_.InputId), 100f);
        Vector2 CurrentSize = new Vector2(Size_Width, Size_Width / 2);

        GUI.Label(new Rect(CurrentPosition, CurrentSize), "Escolha um Jogador");
        CurrentPosition.y += Line_Size;
        GUI.Label(new Rect(CurrentPosition, CurrentSize), CONSTANTS.PLAYER.CLASS_NAME[(int)inputDevicePlayer_.SelectingPlayerClassID]);
    }

    #endregion
}


public class InputDevicePlayer
{
    public int InputId;
    public ENUMERATORS.Player.PlayerClass SelectingPlayerClassID;
    public bool IsSelectingClass_InGame;
    public bool IsSelectingClass_InMenu;
    public bool IsAssignedToPlayer;
    public int AvatarClassId;
    public Player Avatar;

    private UnityInputDevice _myInputDevice;

    public UnityInputDevice myInputDevice
    {
        get
        {
            return _myInputDevice;
        }
        set
        {
            _myInputDevice = value;
        }
    }

    public InputDevicePlayer()
    {
        SelectingPlayerClassID = ENUMERATORS.Player.PlayerClass.UNDEFINED;
    }
}





