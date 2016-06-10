using UnityEngine;
using System.Collections;

public class BaseTurret : MonoBehaviour {

    public float RotationSpeed;
    public float AttackAngle;

	// Use this for initialization
	void Start () {
	
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
                Vector3 cross = Vector3.Cross(this.transform.forward, targetDirection);
                float dot = Vector3.Dot(cross, Vector3.up);

                if (angle >= AttackAngle)
                {
                    this.transform.Rotate(new Vector3(0, RotationSpeed * Mathf.Sign(dot), 0) * Time.deltaTime);
                }
            }
        }
	}
}
