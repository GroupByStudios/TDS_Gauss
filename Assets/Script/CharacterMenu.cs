using UnityEngine;
using System.Collections;

public class CharacterMenu : MonoBehaviour
{
    public int TipoAnimacao;
    public int TipoAnimacaoEspecializada;
    public GameObject ImageDisplay;
	public GameObject DisplayKeyboardDevice;
	public GameObject DisplayJoystickDevice;
    public bool IsSelected;
	public bool IsKeyboard;

    private Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();

        if (_animator != null)
        {
            _animator.SetInteger("TipoAnimacao", TipoAnimacao);
            _animator.SetInteger("TipoAnimEspecializacao", TipoAnimacaoEspecializada);
        }

		if (DisplayKeyboardDevice != null)
			DisplayKeyboardDevice.SetActive(false);

		if (DisplayJoystickDevice != null)
		{
			DisplayJoystickDevice.SetActive(false);
		}
    }


	public void SelectCharacter(bool selection, bool IsKeyboard, int playerNumber)
    {
		if (!selection)
		{
			if (ImageDisplay != null)
			{
				ImageDisplay.SetActive(false);
			}

			if (DisplayKeyboardDevice != null)
				DisplayKeyboardDevice.SetActive(false);

			if (DisplayJoystickDevice != null)
				DisplayJoystickDevice.SetActive(false);
		}
		else
		{
			if (ImageDisplay != null)
				ImageDisplay.SetActive(true);

			if (IsKeyboard && DisplayKeyboardDevice != null)
				DisplayKeyboardDevice.SetActive(true);

			if (!IsKeyboard && DisplayJoystickDevice != null)
				DisplayJoystickDevice.SetActive(true);
		}
    }
}
