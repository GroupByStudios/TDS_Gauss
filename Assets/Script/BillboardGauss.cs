using UnityEngine;
using System.Collections;

public class BillboardGauss : MonoBehaviour
{
    public Transform[] BillBoardObjects;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < BillBoardObjects.Length; i++)
        {
            BillBoardObjects[i].LookAt(new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z));
        }
    }
}
