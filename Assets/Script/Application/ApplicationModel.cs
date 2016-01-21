using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ApplicationModel : MonoBehaviour {

	public static ApplicationModel Instance; // Singleton Pattern
	private Player _currentPlayer = null;

	[HideInInspector] public AudioManager myAudioManager;

	public Player CurrentPlayer {
		get{

			if (_currentPlayer == null)
				_currentPlayer = FindObjectOfType<Player>();

			return _currentPlayer;
		}
		set{
			_currentPlayer = value;
		}
	}

	void Start()
	{
		Application.logMessageReceived += OnApplication_LogCallBack;
		Application.logMessageReceivedThreaded += OnApplication_LogCallBack_Threaded;
	}

	void OnApplication_LogCallBack(string condition, string stackTrace, LogType type)
	{
		Debug.Log(stackTrace);
	}

	void OnApplication_LogCallBack_Threaded(string condition, string stackTrace, LogType type)
	{
		Debug.Log(stackTrace);
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
		}
		else
		{
			Destroy(gameObject);
		}

		myAudioManager = GetComponentInChildren<AudioManager>();

		CreatePoolTable(SpellTable, 100);
		CreatePoolTable(ProjectileTable, 100);
		CreatePoolTable(GranadeTable, 20);
//		CreateSpellTable();
//		CreateProjectileTable();
	}

	// Update is called once per frame
	void Update () {
		// Se perder a instancia do jogador, busca o jogador e atualiza o objeto
		/*if (_currentPlayer == null)
			_currentPlayer = FindObjectOfType<Player>();*/

	}


	/* TABELA DE HABILIDADES 1 - 1 */
	public SpellBase[] SpellTable;
	void CreateSpellTable()
	{
		SpellTable = new SpellBase[CONSTANTS.SPELL.COUNT];

		/*GameObject _spell = Resources.Load(CONSTANTS.RESOURCES_PATH.SPELL_FIREBALL) as GameObject;
		SpellBase _spellBase = _spell.GetComponent<SpellBase>();
		_spellBase.gameObject.SetActive(false);

		PoolManager _projectilePoolManager = new PoolManager();
		_spellBase.Pool = _projectilePoolManager;
		_spellBase.Pool.Initialize(20, this.transform);
		_spellBase.Pool.AddObjectToPool(_spellBase, 20);

		SpellTable[0] = _spellBase;*/
	}

	/// <summary>
	/// Tabela de projeteis disponiveis para uso
	/// </summary>
	public Projectile[] ProjectileTable;
	void CreateProjectileTable()
	{
		for(int i = 0; i < ProjectileTable.Length; i++)
		{
			ProjectileTable[i] = Instantiate(ProjectileTable[i]) as Projectile;
			ProjectileTable[i].name += " Table";
			ProjectileTable[i].gameObject.SetActive(false);
			ProjectileTable[i].Pool = new PoolManager();
			ProjectileTable[i].Pool.Initialize(100, this.transform);
			ProjectileTable[i].Pool.AddObjectToPool(ProjectileTable[i], 100);
		}

		/*ProjectileTable = new Projectile[CONSTANTS.ITEM.PROJECTILE_COUNT];

		GameObject _projectileObject = Resources.Load(CONSTANTS.RESOURCES_PATH.PROJECTILE_RIFLE) as GameObject;
		Projectile _projectile = _projectileObject.GetComponent<Projectile>();
		_projectile.gameObject.SetActive(false);

		PoolManager _projectilePoolManager = new PoolManager();
		_projectile.Pool = _projectilePoolManager;
		_projectile.Pool.Initialize(100, this.transform);
		_projectile.Pool.AddObjectToPool(_projectile, 100);

		ProjectileTable[0] = _projectile;*/
	}

	public GranadeBase[] GranadeTable;
	void CreateGranadeTable()
	{

	}

	void CreatePoolTable(PoolObject[] poolTable, int initialPoolAmmout)
	{
		for(int i = 0; i < poolTable.Length; i++)
		{
			poolTable[i] = Instantiate(poolTable[i], Vector3.zero, Quaternion.identity) as PoolObject;
			poolTable[i].name += " Table";
			poolTable[i].gameObject.SetActive(false);
			poolTable[i].Pool = new PoolManager();
			poolTable[i].Pool.Initialize(initialPoolAmmout, poolTable[i].transform);
			poolTable[i].Pool.AddObjectToPool(poolTable[i], initialPoolAmmout);
			poolTable[i].transform.parent = this.transform;
		}		
	}



}
