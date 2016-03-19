using UnityEngine;
using System.Collections;

public class ServerQuestManager : MonoBehaviour
{
    public ServerButton[] ButtonList;
    public ServerQuestServerUnit[] ServerUnitList;

    public Color ColorGreen = Color.green;
    public Color ColorRed = Color.red;
    public Color ColorBlue = Color.blue;
    public Color ColorYellow = Color.yellow;

    public int CurrentRedButton = 0;
    public int CurrentBlueButton = 0;
    public int CurrentGreenButton = 0;
    public int CurrentYellowButton = 0;

    public bool RedServer, BlueServer, YellowServer, GreenServer;

    [ReadOnlyInInspectorAttribute]
    public int MaxPerColor;

    int _currentIndex;
    System.Random _pseudoRandom;
    Color _nextColor;

    // Use this for initialization
    void Start()
    {
        _pseudoRandom = new System.Random((int)System.DateTime.Now.Ticks);
        MaxPerColor = System.Convert.ToInt32(ButtonList.Length / 4);
        _currentIndex = 0;

        StartQuest();
    }

    void StartQuest()
    {
        DisableAllServers();
        ShuffleServers();
        ProcessNextStep();
    }

    void DisableAllServers()
    {
        BlueServer = false;
        YellowServer = false;
        RedServer = false;
        GreenServer = false;

        for (int i = 0; i < ServerUnitList.Length; i++)
        {
            ServerUnitList[i].ResetServer();
            ServerUnitList[i].ServerActivated = false;
        }
    }

    void ShuffleServers()
    {
        for(int i = 0; i < ServerUnitList.Length; i++)
        {
            ServerUnitList[i].SetServerCurrentColor(NextServerColor());
            ServerUnitList[i].ServerActivated = true;
            ServerUnitList[i].QuestManagerInstance = this;
        }
    }

    Color NextServerColor()
    {
        switch(_pseudoRandom.Next(1, 5))
        {
            case 1:
                if (RedServer)
                    return NextServerColor();
                else
                {
                    RedServer = true;
                    return Color.red;
                }
            case 2:
                if (BlueServer)
                    return NextServerColor();
                else
                {
                    BlueServer = true;
                    return Color.blue;
                }
            case 3:
                if (YellowServer)
                    return NextServerColor();
                else
                {
                    YellowServer = true;
                    return Color.yellow;
                }
            case 4:
                if (GreenServer)
                    return NextServerColor();
                else
                {
                    GreenServer = true;
                    return Color.green;
                }
        }

        return NextServerColor();
    }
        

    void ProcessStep(ServerQuestServerUnit serverUnit_)
    {
        if (serverUnit_.ServerCurrentColor == _nextColor &&
            serverUnit_.ServerActivated)
        {
            serverUnit_.ServerActivated = false;

            DisableAllServers();
            ShuffleServers();
            ProcessNextStep();
        }
        else
        {
            ClearAll();
            DisableAllServers();
            ShuffleServers();
            ProcessNextStep();
        }
    }

    void ProcessNextStep()
    {
        if (_currentIndex != 0)
            ButtonList[_currentIndex - 1].FixButton();
        
        ServerButton _button = GetNextButton();

        if (_button != null){
            _nextColor = GetNextColor();
            _button.ButtonColor = _nextColor;
            _button.BlinkTimeInSeconds = 2f;
            _button.StartButton();
        }
        else
        {
            DisableAllServers();
        }
    }
       
    void ClearAll()
    {
        _currentIndex = 0;
        _nextColor = Color.white;

        for (int i = 0; i < ButtonList.Length; i++)
        {
            ButtonList[i].ResetButton();
        }

        CurrentBlueButton = 0;
        CurrentGreenButton = 0;
        CurrentYellowButton = 0;
        CurrentRedButton = 0;
    }

    ServerButton GetNextButton()
    {
        if (_currentIndex <= ButtonList.Length -1)
        {
            _currentIndex++;
            return ButtonList[_currentIndex-1];
        }
        else
            return null;
    }

    Color GetNextColor()
    {
        switch(_pseudoRandom.Next(1, 5))
        {
            case 1: 
                if (CurrentRedButton < MaxPerColor){
                    CurrentRedButton++;
                    return Color.red;
                }
                else
                    return GetNextColor();
            case 2:
                if (CurrentBlueButton < MaxPerColor){
                    CurrentBlueButton++;
                    return Color.blue;
                }
                else
                    return GetNextColor();
            case 3: 
                if (CurrentYellowButton < MaxPerColor){
                    CurrentYellowButton++;
                    return Color.yellow;
                }
                else
                    return GetNextColor();
            case 4: 
                if (CurrentGreenButton < MaxPerColor){
                    CurrentGreenButton++;
                    return Color.green;
                }
                else
                    return GetNextColor();
        }

        return GetNextColor();
    }

    public void ActivateServerUnit(ServerQuestServerUnit serverUnit_)
    {
        if (serverUnit_ != null)
        {
            Debug.Log("ProcessarPasso");
            ProcessStep(serverUnit_);
        }
    }
}

