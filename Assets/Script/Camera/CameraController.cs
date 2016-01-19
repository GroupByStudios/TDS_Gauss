using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public float CameraBoom_Z;

	// Update is called once per frame
	void Update () {

		float positionX = 0f;
		float positionZ = 0f;
		int activePlayerCount = 0;

		for (int i = 0; i < PlayerManager.Instance.myPlayerAvatarList.Length; i++)
		{
			if (PlayerManager.Instance.myPlayerAvatarList[i].gameObject.activeInHierarchy){
				activePlayerCount++;
				positionX += PlayerManager.Instance.myPlayerAvatarList[i].transform.position.x;
				positionZ += PlayerManager.Instance.myPlayerAvatarList[i].transform.position.z;
			}
		}

		if (activePlayerCount > 0){

			positionX /= activePlayerCount;
			positionZ /= activePlayerCount;

			transform.position = new Vector3(positionX, transform.position.y, positionZ - CameraBoom_Z);
		}
	}
}
