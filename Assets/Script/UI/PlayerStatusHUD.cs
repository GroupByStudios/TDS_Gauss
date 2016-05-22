using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerStatusHUD : MonoBehaviour {

	[HideInInspector] public Player myPlayer;

	private Slider mySlider;
	private Image mySliderFillImage;
	private Text myTextWeaponName;
	private Text myTextWeaponAmmo;
	private Text myTextWeaponMaxAmmo;
	private Text myTextGranadeName;
	private Text myTextGranadeAmmo;

	// Use this for initialization
	void Start () {

		if (mySlider == null)
			mySlider = GetComponentInChildren<Slider>();

		Image[] _images = mySlider.GetComponentsInChildren<Image>();
		Text[] _texts = GetComponentsInChildren<Text>();

		for(int i = 0; i < _images.Length; i++)
		{
			if (_images[i].CompareTag(CONSTANTS.TAGS.PlayerStatusHUD_HealthBar)){
				mySliderFillImage = _images[i];
				break;
			}
		}

		for (int i = 0; i < _texts.Length; i++)
		{
			switch(_texts[i].tag)
			{
			case CONSTANTS.TAGS.PlayerStatusHUD_WeaponName:
				myTextWeaponName = _texts[i];
				break;
			case CONSTANTS.TAGS.PlayerStatusHUD_WeaponAmmo:
				myTextWeaponAmmo = _texts[i];
				break;
			case CONSTANTS.TAGS.PlayerStatusHUD_WeaponMaxAmmo:
				myTextWeaponMaxAmmo = _texts[i];
				break;
			case CONSTANTS.TAGS.PlayerStatusHUD_GranadeName:
				myTextGranadeName = _texts[i];
				break;
			case CONSTANTS.TAGS.PlayerStatusHUD_GranadeAmmo:
				myTextGranadeAmmo = _texts[i];
				break;			
			}
		}
	}

	void Update()
	{
		if (myPlayer != null)
		{
			if (mySlider != null)
			{
				mySliderFillImage.color = PlayerManager.Instance.myPlayerColorList[myPlayer.PlayerClassID];

				if (mySlider.maxValue != myPlayer.Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.HitPoint].MaxWithModifiers)					
					mySlider.maxValue = myPlayer.Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.HitPoint].MaxWithModifiers;
				
				mySlider.value = myPlayer.Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.HitPoint].CurrentWithModifiers;
			}

			if (myPlayer.CurrentWeapon != null)
			{
				myTextWeaponName.text = myPlayer.CurrentWeapon.WeaponName;
				AttributeBase _ammoAttributeBase = myPlayer.CurrentWeapon.Attributes[(int)ENUMERATORS.Attribute.WeaponAttributeTypeEnum.Ammo];

				myTextWeaponAmmo.text = _ammoAttributeBase.CurrentWithModifiers.ToString("000");
				myTextWeaponMaxAmmo.text = _ammoAttributeBase.MaxWithModifiers.ToString("000");
			}
		}
	}

}
