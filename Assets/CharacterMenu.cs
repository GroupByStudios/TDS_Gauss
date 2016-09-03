using UnityEngine;
using System.Collections;

public class CharacterMenu : MonoBehaviour
{

    public GameObject ImageDisplay;

    public void SelectCharacter(bool selection)
    {
        if (ImageDisplay != null)
        {
            ImageDisplay.SetActive(selection);
        }
    }
}
