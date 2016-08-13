using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{

    public bool StarShake;
    public float ShakeForce = 0.25f;
    public float CameraBoom_Z;
    public int ShakeForFrames = 5;

    private int currentFrameShaking;

    void Start()
    {
        currentFrameShaking = ShakeForFrames;
    }

    // Update is called once per frame
    void Update()
    {

        float positionX = 0f;
        float positionZ = 0f;
        int activePlayerCount = 0;

        for (int i = 0; i < PlayerManager.Instance.myPlayerAvatarList.Length; i++)
        {
            if (PlayerManager.Instance.myPlayerAvatarList[i].gameObject.activeInHierarchy)
            {
                activePlayerCount++;
                positionX += PlayerManager.Instance.myPlayerAvatarList[i].transform.position.x;
                positionZ += PlayerManager.Instance.myPlayerAvatarList[i].transform.position.z;
            }
        }

        if (activePlayerCount > 0)
        {

            positionX /= activePlayerCount;
            positionZ /= activePlayerCount;

            transform.position = new Vector3(positionX, transform.position.y, positionZ - CameraBoom_Z);
        }

        if (StarShake)
            Shake();
    }

    void Shake()
    {
        if (currentFrameShaking > 0)
        {
            transform.position = new Vector3(transform.position.x + Random.Range(currentFrameShaking * (ShakeForce * -1), currentFrameShaking * ShakeForce), transform.position.y, transform.position.z + Random.Range(currentFrameShaking * (ShakeForce * -1), currentFrameShaking * ShakeForce));
            currentFrameShaking--;
        }
        else
        {
            StarShake = false;
            currentFrameShaking = ShakeForFrames;
        }
    }
}
