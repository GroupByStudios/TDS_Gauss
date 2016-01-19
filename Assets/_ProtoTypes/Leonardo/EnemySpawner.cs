using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {

	public Enemy EnemyPrefab;
	public Vector2 SpawnLocationRange;

	public float SpawnCoolDown;

	private float _nextSpawn;

	// Update is called once per frame
	void Update () {

		if(EnemyPrefab != null)
		{
			if (_nextSpawn < Time.time)
			{
				Enemy _newEnemy = Instantiate(EnemyPrefab) as Enemy;

				float xPosition = transform.position.x + SpawnLocationRange.x;
				float zPosition = transform.position.z + SpawnLocationRange.y;

				_newEnemy.transform.position = new Vector3(Random.Range(xPosition * -1, xPosition), 1, Random.Range(zPosition * -1, zPosition));

				_nextSpawn = Time.time + SpawnCoolDown;
			}
		}	
	}


	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(transform.position, new Vector3(SpawnLocationRange.x, 1, SpawnLocationRange.y));
	}
}
