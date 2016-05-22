using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Classe responsavel por manter todas as constantes do jogo
/// </summary>
public class CONSTANTS
{

	public class PLAYER
	{
		public const int MEDIC = 0;
		public const int DEFENDER = 1;
		public const int ENGINEER = 2;
		public const int ASSAULT = 3;
		public const int SPECIALIST = 4;

		public const int PLAYER_CLASS_COUNT = 5;

		public static string[] CLASS_NAME = new string[PLAYER_CLASS_COUNT]{
			"Medic",
			"Defender",
			"Engenheiro",
			"Assault",
			"Specialist"
		};
	}

	/// <summary>
	/// Classe responsavel por manter as constantes de atributos
	/// </summary>
	public class ATTRIBUTES
	{
		public const int ATTRIBUTE_MODIFIERS_COUNT = 100;

		public const int CHARACTER_ATTRIBUTE_COUNT = 6;
		public static string[] CHARACTER_TYPE_NAMES = new string[]{
			"HP",
			"EP",
			"Stamina",
			"Speed",
			"Armor",
			"Aggro"
		};

		public const int WEAPON_ATTRIBUTE_COUNT = 6;
		public static string[] WEAPON_ATTRIBUTE_NAMES = new string[]{
			"Damage" ,
			"Ammo", 
			"Fire Rate",
			"Reload Speed",
			"Critic Chance",
			"Critic Multiplier"
		};
	}

	public class INPUT
	{
		public const string HORIZONTAL_AXIS = "Horizontal";
		public const string VERTICAL_AXIS = "Vertical";
		public const string MOUSE_X = "Mouse X";
		public const int MOUSE_LEFT_BUTTON = 0;
		public const int MOUSE_RIGHT_BUTTON = 1;
		public const string LEFT_SHIFT = "Fire3";
		public const string BUTTON_1 = "Button_1";
		public const string BUTTON_2 = "Button_2";
		public const string BUTTON_3 = "Button_3";
		public const string RELOAD = "Reload";
	}

	public class TAGS
	{
		public const string PLAYER = "Player";
		public const string PlayerStatusHUD_HealthBar = "PlayerStatusHUD_HealthBar";
		public const string PlayerStatusHUD_WeaponName = "PlayerStatusHUD_WeaponName";
		public const string PlayerStatusHUD_WeaponAmmo = "PlayerStatusHUD_WeaponAmmo";
		public const string PlayerStatusHUD_WeaponMaxAmmo = "PlayerStatusHUD_WeaponMaxAmmo";
		public const string PlayerStatusHUD_GranadeName = "PlayerStatusHUD_GranadeName";
		public const string PlayerStatusHUD_GranadeAmmo = "PlayerStatusHUD_GranadeAmmo";
	}

	public class ANIMATION
	{
//		public const string SPEED = "Speed";
		public const string FORWARD = "Forward";
		public const string SIDEWAYS = "Sideways";
		public const string ISIRONSIGHT = "IsIronSight";
		public const string ISSHOOTING = "IsShooting";
		public const string ISRELOADING = "IsReloading";
		public const string SHOOTINGMULTIPLIER = "ShootingMultiplier";
		public const string RELOADINGMULTIPLIER = "ReloadingMultiplier";
		public const string WEAPONTYPE = "WeaponType";
	}

	public class SPELL
	{
		public const int COUNT = 10;
	}

	public class ITEM
	{
		public const int PROJECTILE_COUNT = 10;
	}

	public class RESOURCES_PATH
	{
		public const string PROJECTILE_RIFLE = "Prefab/Projectile/Prototype_Pistol_Projectile";
	}
}

