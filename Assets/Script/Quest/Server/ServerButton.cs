using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
public class ServerButton : MonoBehaviour
{
    public int ID;
    public Light ButtonLight;
    public Color ButtonColor;
    Color ButtonDefaultColor;
    Color ButtonDefaultLightColor;
    public Renderer ButtonInstanceMaterial;
    public float BlinkTimeInSeconds;
    public bool Start;

    bool LerpToBlink;

    void Awake()
    {
        ButtonLight = GetComponent<Light>();
        ButtonInstanceMaterial = GetComponent<Renderer>();

        ButtonDefaultColor = ButtonInstanceMaterial.material.color;

        if (ButtonLight != null)
        {
            ButtonDefaultLightColor = ButtonLight.color;
        }
    }

    void Update()
    {
        if (Start)
            BlinkButton();
    }

    public void StartButton()
    {
        Start = true;
        StartCoroutine(LerpColorByTime());
    }

    IEnumerator LerpColorByTime()
    {     
        LerpToBlink = !LerpToBlink;

        yield return new WaitForSeconds(BlinkTimeInSeconds);

        if (Start)
            StartCoroutine(LerpColorByTime());
        else
            StopCoroutine(LerpColorByTime());
    }

    public void ResetButton()
    {
        StopCoroutine(LerpColorByTime());

        Start = false;
        LerpToBlink = false;

        ButtonInstanceMaterial.material.color = ButtonDefaultColor;

        if (ButtonLight != null)
        {
            ButtonLight.color = ButtonDefaultLightColor;
        }
    }

    public void FixButton()
    {
        StopCoroutine(LerpColorByTime());

        Start = false;

        ButtonInstanceMaterial.material.color = ButtonColor;

        if (ButtonLight != null)
        {
            ButtonLight.color = ButtonColor;
        }
    }

    void BlinkButton()
    {
        ButtonInstanceMaterial.material.color = 
            Color.Lerp(ButtonInstanceMaterial.material.color, 
                LerpToBlink ? ButtonColor : ButtonDefaultColor, 
                Mathf.PingPong(Time.time, 1));

        ButtonLight.color = 
            Color.Lerp(ButtonLight.color, 
                LerpToBlink ? ButtonColor : ButtonDefaultLightColor,
                Mathf.PingPong(Time.time, 1));
    }
}

