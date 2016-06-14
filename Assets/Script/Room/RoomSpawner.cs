using UnityEngine;
using System.Collections;

public class RoomSpawner : MonoBehaviour {

	public RoomManager RoomManager = null;
	public float ShakeAmount = 1.0f;
	public RoomSpawnerState State;
	public float RiseSpeed = 0.5f;
	public int Rounds = 3;
	public float RoundCoolDown = 10f;
	public int EnemiesPerRound = 4;
	public BaseEnemy WeakEnemy = null;


	private Vector3 _originalPosition;
	private Vector3 _endPosition;

	private BaseEnemy[] Enemies = new BaseEnemy[20];
	private float baseEnemyCount = 0;

	bool SpawnStarted = false;

	// Use this for initialization
	void Start () {
		State = RoomSpawnerState.NotActivate;
		_originalPosition = this.transform.position;
		_endPosition = new Vector3(_originalPosition.x, 1, _originalPosition.z);

		Enemies = new BaseEnemy[EnemiesPerRound * Rounds];
	}
	
	// Update is called once per frame
	void Update () {

		switch(State)
		{
		case RoomSpawnerState.NotActivate:
			break;
		case RoomSpawnerState.Activating:

			MoveUpDown(Vector3.up);
			Shake();

			if (transform.position.y >= _endPosition.y){
				transform.position = _endPosition;
				State = RoomSpawnerState.Activated;
			}

			break;

		case RoomSpawnerState.Activated:

			if (!SpawnStarted)
			{
				this.SpawnEnemies();
				SpawnStarted = true;
			}

			break;

		case RoomSpawnerState.Finishing:

			MoveUpDown(Vector3.down);
			Shake();

			if (transform.position.y <= 6f) // Verificar o fim do mapa
			{
				this.State = RoomSpawnerState.Finished;
			}

			break;
		}
	}

	public IEnumerator SpawnEnemiesCoroutine()
	{
		yield return new WaitForSeconds(this.RoundCoolDown);
		SpawnEnemies();
	}

	private void SpawnEnemies()
	{
		if (this.State == RoomSpawnerState.Activated)
		{
			// Chama os Robos
			int enemyIndex = 0;
			Vector3 _position;

			for (int i = 0; i < this.EnemiesPerRound; i++)
			{
				enemyIndex = Helper.GetFreePosition(this.Enemies);
				if (enemyIndex > -1)
				{
					if (WeakEnemy != null){

						this.Enemies[enemyIndex] = Instantiate(WeakEnemy).GetComponent<BaseEnemy>();

						if (RoomManager != null){

							_position.x = Random.Range(RoomManager.MinX, RoomManager.MaxX);
							_position.z = Random.Range(RoomManager.MinZ, RoomManager.MaxZ);
							_position.y = 20f;

							this.Enemies[enemyIndex].transform.position = _position;
							this.Enemies[enemyIndex].transform.Rotate(Vector3.up * Random.Range(0f, 359f));
							this.Enemies[enemyIndex].OnBeforeDie += this.BaseEnemy_OnDie;
							this.baseEnemyCount++;
						}
					}
				}
			}
			StartCoroutine(this.SpawnEnemiesCoroutine());
		}
	}

	private bool IsFinished
	{
		get
		{
			return Helper.GetFreePosition(this.Enemies) == -1 && this.baseEnemyCount == 0;
		}
	}

	private void BaseEnemy_OnDie(BaseEnemy enemy_)
	{		
		baseEnemyCount--;
		enemy_.OnBeforeDie -= this.BaseEnemy_OnDie;

		if (this.IsFinished)
			this.State = RoomSpawnerState.Finishing;
	}

	private void Shake()
	{
		transform.position = _originalPosition + Random.insideUnitSphere * ShakeAmount;
	}

	private void MoveUpDown(Vector3 _position)
	{
		_originalPosition += _position * RiseSpeed * Time.deltaTime;
	}
}

public enum RoomSpawnerState
{
	NotActivate,
	Activating,
	Activated,
	Finishing,
	Finished,
}
