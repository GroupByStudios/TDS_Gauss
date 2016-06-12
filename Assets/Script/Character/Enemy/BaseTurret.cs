using UnityEngine;
using System.Collections;

public class BaseTurret : Character {

    public float RotationSpeed;
    public float AttackAngle;
    public float FireAngle;

    public WeaponBase[] TurretWeapons;


    private int _currentWeaponIndex = 0;

	// Use this for initialization
	void Start () {
	
        // InitializeWeapons;
        for (int i = 0; i < TurretWeapons.Length;i++)
        {
            TurretWeapons[i].WeaponOwner = this;
        }

	}
	
	// Update is called once per frame
	void Update () {
	
        if (PlayerManager.Instance != null &&
            PlayerManager.Instance.ActivePlayers != null &&
            PlayerManager.Instance.ActivePlayers.Count > 0)
        {
            var target = PlayerManager.PlayerWithMoreAggro(PlayerManager.Instance.ActivePlayers.ToArray());

            if (target != null)
            {
                Vector3 targetDirection = target.transform.position - this.transform.position;
                float angle = Vector3.Angle(this.transform.forward, targetDirection);

                if (angle >= AttackAngle)
                {
                    Vector3 cross = Vector3.Cross(this.transform.forward, targetDirection);
                    float dot = Vector3.Dot(cross, Vector3.up);
                    this.transform.Rotate(new Vector3(0, RotationSpeed * Mathf.Sign(dot), 0) * Time.deltaTime);
                }

                if (angle <= FireAngle)
                {
                    if (TurretWeapons != null && TurretWeapons.Length > 0)
                    {
                        if (_currentWeaponIndex >= TurretWeapons.Length)
                            _currentWeaponIndex = 0;

                        TurretWeapons[_currentWeaponIndex].TriggerPressed();
                        TurretWeapons[_currentWeaponIndex].IsShooting = false;
                        _currentWeaponIndex++;
                    }
                }
            }
        }


	}
}
