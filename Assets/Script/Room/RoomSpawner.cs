using UnityEngine;
using System.Collections;

public class RoomSpawner : MonoBehaviour {

	public RoomManager RoomManager = null;
	public float ShakeAmount = 1.0f;
	public RoomSpawnerState State;
	public float RiseSpeed = 0.5f;
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
	}
	
	// Update is called once per frame
	void Update () {

		if (!SpawnStarted){
			switch(State)
			{
			case RoomSpawnerState.NotActivate:
				break;
			case RoomSpawnerState.Activating:

				Rise();
				Shake();

				if (transform.position.y >= _endPosition.y){
					transform.position = _endPosition;
					State = RoomSpawnerState.Activated;
				}

				break;

			case RoomSpawnerState.Activated:
				this.SpawnEnemies();
				SpawnStarted = true;
				break;
			}
		}
	}

	public IEnumerator SpawnEnemiesCoroutine()
	{
		yield return new WaitForSeconds(20f);
		SpawnEnemies();
	}

	private void SpawnEnemies()
	{
		if (this.State == RoomSpawnerState.Activated)
		{
			// Chama os Robos
			int SpawnPerTime = 6;
			int enemyIndex = 0;
			Vector3 _position;


			for (int i = 0; i < SpawnPerTime; i++)
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
						}
					}
				}
			}
			StartCoroutine(this.SpawnEnemiesCoroutine());
		}
	}


	private void BaseEnemy_OnDie()
	{
		baseEnemyCount--;
	}

	private void BaseEnemy_OnActivated()
	{
		baseEnemyCount++;
	}

	private void Shake()
	{
		transform.position = _originalPosition + Random.insideUnitSphere * ShakeAmount;
	}

	private void Rise()
	{
		_originalPosition += Vector3.up * RiseSpeed * Time.deltaTime;
	}
}

public enum RoomSpawnerState
{
	NotActivate,
	Activating,
	Activated,
	Destroyed
}
