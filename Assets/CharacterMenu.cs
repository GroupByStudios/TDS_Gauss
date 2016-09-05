using UnityEngine;
using System.Collections;

public class CharacterMenu : MonoBehaviour
{
    public int TipoAnimacao;
    public int TipoAnimacaoEspecializada;
    public GameObject ImageDisplay;

    private Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();

        if (_animator != null)
        {
            _animator.SetInteger("TipoAnimacao", TipoAnimacao);
            _animator.SetInteger("TipoAnimEspecializacao", TipoAnimacaoEspecializada);
        }
    }


    public void SelectCharacter(bool selection)
    {
        if (ImageDisplay != null)
        {
            ImageDisplay.SetActive(selection);
        }
    }
}
