using UnityEngine;
using System.Collections;

public class WeaponConfigurationItem
{
    public int ID { get;set; }
    public int ProjectileID { get; set; }

    public int WeaponType { get; set; }
    public string WeaponName { get; set; }
    public bool Automatic { get; set; }
    public int AmmoMax { get; set; }
    public int Damage { get; set; }
    public float FirePerSecond { get; set; }
    public float ReloadTimeInSeconds { get; set; }

    public string PrefabPath { get; set; }



}

