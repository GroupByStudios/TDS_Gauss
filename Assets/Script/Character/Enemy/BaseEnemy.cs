using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
public class BaseEnemy : PoolObject {

	const string ANIM_WALKING = "Walking";
	const string ANIM_ATTACK = "Attack";
	const string ANIM_DEAD = "Dead";

	public LayerMask PlayerLayer;
	public EnemyState State;
	public float AttackDistance = 0;
	public float RandomMove = 0.7f;

	CapsuleCollider _capsule = null;
	Animator _animator = null;

	void Awake()
	{
		_capsule = GetComponent<CapsuleCollider>();
		_animator = GetComponent<Animator>();		
	}

	// Use this for initialization
	void Start () {	
		State = EnemyState.Idle;
	}
	
	// Update is called once per frame
	protected override void Update () {

		switch(this.State)
		{
		case EnemyState.Idle:
			CheckPlayerProximity();
			CanSeePlayer();
			break;
		case EnemyState.Walking:
			CheckPlayerProximity();
			CanSeePlayer();
			break;
		case EnemyState.Attacking:
			break;
		case EnemyState.Dead:
			break;
		}
	}

	void MoveRandom()
	{
		
	}

	void CanSeePlayer()
	{
		RaycastHit _hit;
		Ray _rayCast;
		Vector3 rayDirection;
		Player _player = null;

		for (int i = 0; i < PlayerManager.Instance.ActivePlayers.Count; i++)
		{
			_player = PlayerManager.Instance.ActivePlayers[i];
			rayDirection = (_player.transform.position + _capsule.center) - (transform.position + _capsule.center);
			_rayCast = new Ray(transform.position + _capsule.center, rayDirection);

			//Debug.DrawRay(_rayCast.origin, _rayCast.direction);

			if (Physics.Raycast(_rayCast.origin, _rayCast.direction, out _hit, 500f))
			{
				if (_hit.transform == _player.transform)
				{
					//Debug.Log("Esta vendo");
				}
			}
		}
	}

	void CheckPlayerProximity()
	{
		Player closestPlayer = null;
		Player processingPlayer = null;
		Vector3 distanceVector;

		for (int i = 0; i < PlayerManager.Instance.ActivePlayers.Count; i++)
		{
			processingPlayer = PlayerManager.Instance.ActivePlayers[i];
			distanceVector = processingPlayer.transform.position - transform.position;

			if (distanceVector.magnitude <= AttackDistance)
			{
				_animator.SetTrigger(ANIM_ATTACK);
				closestPlayer = processingPlayer;
				this.State = EnemyState.Attacking;
				break;
			}
		}
	}


	void Hit_Event()
	{
		//Debug.Log("Evento de Hit executado");
	}

	void Hit_Finished()
	{
		this.State = EnemyState.Idle;		
		//Debug.Log("Finalizou o Hit");
	}

	void OnDrawGizmos()
	{
		if (_capsule == null)
			_capsule = GetComponent<CapsuleCollider>();

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position + _capsule.center, AttackDistance);
	}

}

public enum EnemyState
{
	Idle,
	Walking,
	Attacking,
	Dead
}
