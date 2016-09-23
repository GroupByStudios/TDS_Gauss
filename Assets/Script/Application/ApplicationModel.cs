using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ApplicationModel : MonoBehaviour
{
	public AudioClip[] vulto;
	AudioSource som;
	public static ApplicationModel Instance; // Singleton Pattern

    public GameState State = GameState.Initializing;
    public Animator MenuStartAnimator;
    public CharacterMenu[] MenuCharacter;

    [HideInInspector]
    public AudioManager myAudioManager;
    [HideInInspector]
    public ConfigurationManager myConfigurationManager;

    public SkillBase[] SpellTable;
    [HideInInspector]
    public Projectile[] ProjectileTable;
    public GranadeBase[] GranadeTable;

    public CameraController CameraController;
	bool pressEnter = false;

    void Start()
    {
		som = GetComponent<AudioSource> ();
		som.clip = vulto[0];
        Application.logMessageReceived += OnApplication_LogCallBack;
        Application.logMessageReceivedThreaded += OnApplication_LogCallBack_Threaded;
    }

    /// <summary>
    /// Awake this instance.
    /// Implementa o conceito do Singleton para o GameObject, SEMPRE UTILIZAR A VARIAVEL INSTANCE para acessar o objeto por fora da classe por exemplo: ApplicationModel.Instance
    /// </summary>
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            myAudioManager = GetComponentInChildren<AudioManager>();

            myConfigurationManager = new ConfigurationManager();
            myConfigurationManager.InitializeConfiguration();

            LoadProjectileTable();

            CreatePoolTable(SpellTable, 25);
            CreatePoolTable(GranadeTable, 20);
        }
        /*else
        {
            Destroy(gameObject);
        }*/
    }

    void LoadProjectileTable()
    {
        // Load Projectile Table From Config File
        if (myConfigurationManager.ProjectileConfiguration != null &&
            myConfigurationManager.ProjectileConfiguration.SectionCount > 0)
        {
            ProjectileTable = new Projectile[myConfigurationManager.ProjectileConfiguration.SectionCount];

            for (int i = 0; i < ProjectileTable.Length; i++)
            {
                ProjectileConfigItem _configItem = myConfigurationManager.ProjectileConfiguration[i].CreateObject<ProjectileConfigItem>();

                if (_configItem == null)
                {
                    Debug.Log("Erro ao carregar ao converter as configuracoes do projetil");
                    continue;
                }

                if (string.IsNullOrEmpty(_configItem.PrefabPath))
                {
                    Debug.Log(string.Format("Caminho do prefab nao informado para o ID: {0}", _configItem.ID));
                    continue;
                }

                GameObject _prefab = Resources.Load<GameObject>(_configItem.PrefabPath);

                if (_prefab == null)
                {
                    Debug.Log(string.Format("Prefab '{0}' nao encontrado para o ID '{1}'", _configItem.PrefabPath, _configItem.ID));
                    continue;
                }

                Projectile _prefabComponent = _prefab.GetComponent<Projectile>();

                if (_prefabComponent == null)
                {
                    Debug.Log(string.Format("Prefab '{0}' nao contem o componente Projectile", _prefab.ToString()));
                    continue;
                }

                _prefabComponent.ID = _configItem.ID;
                ProjectileTable[i] = _prefabComponent;
            }

            CreatePoolTable(ProjectileTable, 100);
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (State)
        {
            case GameState.Initializing:
                State = GameState.PressStartMenu;
                break;
		case GameState.PressStartMenu:
			if (Input.anyKeyDown && MenuStartAnimator != null && !pressEnter) {
				MenuStartAnimator.SetTrigger ("MoveCamera");
				pressEnter = true;
				som.Play ();
			}
				break;
            case GameState.CharacterSelection:
                break;
            case GameState.StartGame:
                break;
            case GameState.InGame:
                break;
        }
    }

    void CreatePoolTable(PoolObject[] poolTable, int initialPoolAmmout)
    {
        for (int i = 0; i < poolTable.Length; i++)
        {
            poolTable[i] = Instantiate(poolTable[i], Vector3.zero, Quaternion.identity) as PoolObject;

            /*SkillBase _skillBase = poolTable[i] as SkillBase;
			if (_skillBase != null)
				_skillBase.SetupSkill();*/

            poolTable[i].name += " Table";
            poolTable[i].gameObject.SetActive(false);
            poolTable[i].Pool = new PoolManager();
            poolTable[i].Pool.Initialize(initialPoolAmmout, poolTable[i].transform);
            poolTable[i].Pool.AddObjectToPool(poolTable[i], initialPoolAmmout);

            /*if (_skillBase != null)
			{
				for (int j = 0; j < _skillBase.Pool.Length; j++)
				{
					((SkillBase)_skillBase.Pool[j]).SetupSkill();
				}
			}*/

            poolTable[i].transform.parent = this.transform;
        }
    }

    /// <summary>
    /// Retorna a referencia da tabela de objetos 
    /// </summary>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    /*public SkillBase GetSpellPool<T>()
	{
		for (int i = 0;  i < SpellTable.Length; i++)
		{
			if (SpellTable[i] is T)
				return SpellTable[i];
		}

		return null;
	}*/

    public SkillBase GetSpellPool(ENUMERATORS.Spell.SkillID spellId_)
    {
        for (int i = 0; i < SpellTable.Length; i++)
        {
            if ((int)SpellTable[i].SkillID == (int)spellId_)
                return SpellTable[i];
        }

        return null;
    }

    void OnApplication_LogCallBack(string condition, string stackTrace, LogType type)
    {
        Debug.Log(stackTrace);
    }

    void OnApplication_LogCallBack_Threaded(string condition, string stackTrace, LogType type)
    {
        Debug.Log(stackTrace);
    }


    #region Time Section


    public float GlobalTime
    {
        get { return Time.time; }
    }

    #endregion


    #region Camera Methods

    public void ShakeCamera()
    {
        if (CameraController != null)
            CameraController.StarShake = true;
    }


    #endregion

}

public enum GameState
{
    Initializing = 0,
    PressStartMenu = 1,
    CharacterSelection = 2,
    InGame = 3,
    StartGame = 4,
}