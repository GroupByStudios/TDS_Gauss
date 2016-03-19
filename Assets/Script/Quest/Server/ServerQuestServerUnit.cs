using UnityEngine;
using System.Collections;

public class ServerQuestServerUnit : MonoBehaviour
{
    public Color ServerDefaultColor;
    public Color ServerCurrentColor;
    public bool ServerActivated;
    public ServerQuestManager QuestManagerInstance;

    Renderer _instanceMaterial;
    bool _triggerProcessed;

    void Awake()
    {
        _instanceMaterial = GetComponent<Renderer>();

        if (_instanceMaterial != null)
        {
            ServerDefaultColor = _instanceMaterial.material.color;
        }
    }

    public void ResetServer()
    {
        if (_instanceMaterial != null)
        {
            _instanceMaterial.material.color = ServerDefaultColor;
        }

        _triggerProcessed = false;
    }

    public void SetServerCurrentColor(Color currentColor_)
    {
        ServerCurrentColor = currentColor_;

        if (_instanceMaterial != null)
        {
            _instanceMaterial.material.color = ServerCurrentColor;
        }
    }

    void OnTriggerStay(Collider other_)
    {
        if (ServerActivated){
            if (Input.GetKeyUp(KeyCode.E))
            {
                if (_triggerProcessed)
                {
                    if (QuestManagerInstance != null)
                        QuestManagerInstance.ActivateServerUnit(this);
                }
                else
                {
                    _triggerProcessed = true;
                }
            }
        }
    }
}

