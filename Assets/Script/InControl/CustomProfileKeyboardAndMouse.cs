using System;
using UnityEngine;
using InControl;

public class CustomProfileKeyboardAndMouse : UnityInputDeviceProfile
{
	public CustomProfileKeyboardAndMouse()
	{
		Name = "Keyboard/Mouse";
		Meta = "A keyboard and mouse combination profile apropriate for the TopDown.";

		// This profile only works on desktops.
		SupportedPlatforms = new[]
		{
			"Windows",
			"Mac",
			"Linux"
		};

		Sensitivity = 1.0f;
		LowerDeadZone = 0.0f;
		UpperDeadZone = 1.0f;

		ButtonMappings = new[]
		{
			new InputControlMapping
			{
				Handle = "Shooting",
				Target = InputControlType.RightTrigger,
				Source = MouseButton0
			},
			new InputControlMapping
			{
				Handle = "Aim",
				Target = InputControlType.LeftTrigger,
				// KeyCodeButton fires when any of the provided KeyCode params are down.
				Source = MouseButton1
			},
			new InputControlMapping
			{
				Handle = "Reload",
				Target = InputControlType.Action3,
				Source = KeyCodeButton(KeyCode.R)
			},
			new InputControlMapping
			{
				Handle = "ThrowGranade",
				Target = InputControlType.RightBumper,
				Source = KeyCodeButton(KeyCode.G)
			},
			new InputControlMapping
			{
				Handle = "Skill_Slot1",
				Target = InputControlType.DPadLeft,
				Source = KeyCodeButton(KeyCode.Keypad1)
			},
			new InputControlMapping
			{
				Handle = "Skill_Slot2",
				Target = InputControlType.DPadRight,
				Source = KeyCodeButton(KeyCode.Keypad1)
			}
		};

		AnalogMappings = new[]
		{
			new InputControlMapping
			{
				Handle = "Move X",
				Target = InputControlType.LeftStickX,
				// KeyCodeAxis splits the two KeyCodes over an axis. The first is negative, the second positive.
				Source = KeyCodeAxis( KeyCode.A, KeyCode.D )
			},
			new InputControlMapping
			{
				Handle = "Move Y",
				Target = InputControlType.LeftStickY,
				// Notes that up is positive in Unity, therefore the order of KeyCodes is down, up.
				Source = KeyCodeAxis( KeyCode.S, KeyCode.W )
			},
			new InputControlMapping
			{
				Handle = "Look X",
				Target = InputControlType.RightStickX,
				Source = MouseXAxis,
				Raw    = true,
				Scale  = 0.1f
			},
			new InputControlMapping
			{
				Handle = "Look Y",
				Target = InputControlType.RightStickY,
				Source = MouseYAxis,
				Raw    = true,
				Scale  = 0.1f
			},
		};
	}
}

