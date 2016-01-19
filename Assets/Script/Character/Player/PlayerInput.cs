using UnityEngine;
using System.Collections;
using InControl;

public class PlayerInput : MonoBehaviour {

	[Range(1, 10)]
	public float MouseSensitivity = 1f;
	public float Move_X;
	public float Move_X_Converted;
	public float Move_Y;
	public float Move_Y_Converted;
	public Vector3 MouseInput;
	public Vector3 MouseInputRaw;

	public bool ShootActionIsPressed;
	public bool ShootActionWasPressed;
	public bool AimActionIsPressed;
	public bool AimActionWasPressed;
	public bool ReloadActionIsPressed;
	public bool ReloadActionWasPressed;
	public bool ThrowGranadeActionIsPressed;
	public bool ThrowGranadeActionWasRelease;

/*	public bool Action1IsPressed;
	public bool Action1WasPressed;
	public bool Action2IsPressed;
	public bool Action2WasPressed;
	public bool ActionPressed;

	public bool ReloadWasPressed;

	public bool Weapon1Pressed;
	public bool Weapon2Preesed;
	public bool Weapon3Pressed;

	public bool GranadePressed;
	public bool GranadeWasPressed;

	public bool PlayerSelected1;
	public bool PlayerSelected2;*/

	private bool DebugEnabled;

	public InputDevice InputDeviceJoystick;

	// Update is called once per frame
	void Update () {

		if (InputDeviceJoystick == null){
		
		Move_X = Input.GetAxisRaw(CONSTANTS.INPUT.HORIZONTAL_AXIS);
		Move_Y = Input.GetAxisRaw(CONSTANTS.INPUT.VERTICAL_AXIS);

		MouseInputRaw = Input.mousePosition;
		MouseInput = MouseInputRaw * MouseSensitivity;

		ShootActionIsPressed = Input.GetMouseButton(CONSTANTS.INPUT.MOUSE_LEFT_BUTTON);
		ShootActionWasPressed = Input.GetMouseButtonDown(CONSTANTS.INPUT.MOUSE_LEFT_BUTTON);

		AimActionIsPressed = Input.GetMouseButton(CONSTANTS.INPUT.MOUSE_RIGHT_BUTTON);
		AimActionWasPressed = Input.GetMouseButtonDown(CONSTANTS.INPUT.MOUSE_RIGHT_BUTTON);

		ReloadActionIsPressed = Input.GetKey(KeyCode.R);
		ReloadActionWasPressed = Input.GetKeyDown(KeyCode.R);

		ThrowGranadeActionIsPressed = Input.GetKey(KeyCode.G);
			ThrowGranadeActionWasRelease = Input.GetKeyUp(KeyCode.G);


		}
		else
		{
			Move_X = InputDeviceJoystick.LeftStickX;
			Move_Y = InputDeviceJoystick.LeftStickY;

			MouseInputRaw = new Vector3(InputDeviceJoystick.RightStick.X, InputDeviceJoystick.RightStick.Y, 0);
			MouseInput = MouseInputRaw * MouseSensitivity;

			ShootActionIsPressed = InputDeviceJoystick.RightTrigger.IsPressed;
			ShootActionWasPressed = InputDeviceJoystick.RightTrigger.WasPressed;

			AimActionIsPressed = InputDeviceJoystick.LeftTrigger.IsPressed;
			AimActionWasPressed = InputDeviceJoystick.LeftTrigger.WasPressed;

			ReloadActionIsPressed = InputDeviceJoystick.Action3.IsPressed;
			ReloadActionWasPressed = InputDeviceJoystick.Action3.WasPressed;

			ThrowGranadeActionIsPressed = InputDeviceJoystick.RightBumper.IsPressed;
			ThrowGranadeActionWasRelease = InputDeviceJoystick.RightBumper.WasReleased;
		}

		//ActionPressed = Input.GetAxisRaw("Action_KeyBoard") == 1;

		// Habilitar o Debug das Variaveis de Controle
		if (Input.GetKeyDown(KeyCode.F1))
			DebugEnabled = !DebugEnabled;
	}

	/// <summary>
	/// Metodo responsavel por converte o input Horizontal e Vertical nas coordenadas do Jogador
	/// </summary>
	public void ConvertMoveInput(Vector3 movement_)
	{
		Vector3 localMove = transform.InverseTransformDirection(movement_);

		Move_X_Converted = localMove.x;
		if (Move_X_Converted > 0.99f)
			Move_X_Converted = 1;
		if (Move_X_Converted < -0.99f)
			Move_X_Converted = -1;		

		Move_Y_Converted = localMove.z;
		if (Move_Y_Converted > 0.99f)
			Move_Y_Converted = 1;
		if (Move_Y_Converted < -0.99f)
			Move_Y_Converted = -1;		
	}

	void OnGUI()
	{
		if (DebugEnabled)
			DrawDebugMode();		
	}

	void DrawDebugMode()
	{
		float newLineSize = 15f;
		float LabelSize = 250f;
		float sizeXPercent = Screen.width * Mathf.Abs(((LabelSize / Screen.width) - 1));

		GUIStyle _guiStyle = new GUIStyle();
		_guiStyle.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
		_guiStyle.fontStyle = FontStyle.Bold;

		Vector2 StarPosition = new Vector2(sizeXPercent, Screen.height * 0.10f);
		Vector2 CurrentPosition = StarPosition;

		GUI.Label(new Rect(CurrentPosition.x, CurrentPosition.y, LabelSize, LabelSize), "Player Input DEBUG MODE", _guiStyle);
		CurrentPosition.y += newLineSize * 2;
		GUI.Label(new Rect(CurrentPosition.x, CurrentPosition.y, LabelSize, LabelSize), string.Format("Move X: {0}", Move_X.ToString()), _guiStyle);
		CurrentPosition.y += newLineSize;
		GUI.Label(new Rect(CurrentPosition.x, CurrentPosition.y, LabelSize, LabelSize), string.Format("Move X Converted: {0}", Move_X_Converted.ToString()), _guiStyle);
		CurrentPosition.y += newLineSize;
		GUI.Label(new Rect(CurrentPosition.x, CurrentPosition.y, LabelSize, LabelSize), string.Format("Move Y: {0}", Move_Y.ToString()), _guiStyle);
		CurrentPosition.y += newLineSize;
		GUI.Label(new Rect(CurrentPosition.x, CurrentPosition.y, LabelSize, LabelSize), string.Format("Move Y Converted: {0}", Move_Y_Converted.ToString()), _guiStyle);
		CurrentPosition.y += newLineSize;
		GUI.Label(new Rect(CurrentPosition.x, CurrentPosition.y, LabelSize, LabelSize), string.Format("Mouse Input Raw: {0}", MouseInputRaw.ToString()), _guiStyle);
		CurrentPosition.y += newLineSize;
		GUI.Label(new Rect(CurrentPosition.x, CurrentPosition.y, LabelSize, LabelSize), string.Format("Mouse Sensitivy: {0}", MouseSensitivity.ToString()), _guiStyle);
		CurrentPosition.y += newLineSize;
		GUI.Label(new Rect(CurrentPosition.x, CurrentPosition.y, LabelSize, LabelSize), string.Format("Mouse Input: {0}", MouseInput.ToString()), _guiStyle);
		CurrentPosition.y += newLineSize;
		GUI.Label(new Rect(CurrentPosition.x, CurrentPosition.y, LabelSize, LabelSize), string.Format("ShootActionIsPressed: {0}", ShootActionIsPressed.ToString()), _guiStyle);
		CurrentPosition.y += newLineSize;
		GUI.Label(new Rect(CurrentPosition.x, CurrentPosition.y, LabelSize, LabelSize), string.Format("ShootActionWasPressed: {0}", ShootActionWasPressed.ToString()), _guiStyle);
		CurrentPosition.y += newLineSize;
		GUI.Label(new Rect(CurrentPosition.x, CurrentPosition.y, LabelSize, LabelSize), string.Format("AimActionIsPressed: {0}", AimActionIsPressed.ToString()), _guiStyle);
		CurrentPosition.y += newLineSize;
		GUI.Label(new Rect(CurrentPosition.x, CurrentPosition.y, LabelSize, LabelSize), string.Format("AimActionWasPressed: {0}", AimActionWasPressed.ToString()), _guiStyle);
		CurrentPosition.y += newLineSize;
		GUI.Label(new Rect(CurrentPosition.x, CurrentPosition.y, LabelSize, LabelSize), string.Format("ReloadActionIsPressed: {0}", ReloadActionIsPressed.ToString()), _guiStyle);
		CurrentPosition.y += newLineSize;
		GUI.Label(new Rect(CurrentPosition.x, CurrentPosition.y, LabelSize, LabelSize), string.Format("ReloadActionWasPressed: {0}", ReloadActionWasPressed.ToString()), _guiStyle);
		CurrentPosition.y += newLineSize;
		GUI.Label(new Rect(CurrentPosition.x, CurrentPosition.y, LabelSize, LabelSize), string.Format("ThrowGranadeActionIsPressed: {0}", ThrowGranadeActionIsPressed.ToString()), _guiStyle);
		CurrentPosition.y += newLineSize;
		GUI.Label(new Rect(CurrentPosition.x, CurrentPosition.y, LabelSize, LabelSize), string.Format("ThrowGranadeActionWasRelease: {0}", ThrowGranadeActionWasRelease.ToString()), _guiStyle);
		CurrentPosition.y += newLineSize;
	}
}