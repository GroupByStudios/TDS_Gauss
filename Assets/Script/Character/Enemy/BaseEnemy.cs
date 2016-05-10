using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class BaseEnemy : PoolObject {

	const string ANIM_VELOCITY = "Velocity";
	const string ANIM_ATTACK = "Attack";
	const string ANIM_DEAD = "Dead";
	const string ANIM_ATTACK_SPEED_MULTIPLIER = "AttackSpeed_Multiplier";
	const string ANIM_WALK_SPEED_MULTIPLIER = "WalkSpeed_Multiplier";

	public LayerMask PlayerLayer;
	public EnemyState State;
	public float AttackDistance = 1.5f;
	public float WalkSpeedMultiplier = 1f;
	public float AttackSpeedMultiplier = 1f;
	public float AwareDistance = 15;
	public float RandomMove = 0.7f;

	private float _acceleration;
	private float _deAcceleration;
	private float _speed;

	CapsuleCollider _capsule = null;
	Animator _animator = null;
	NavMeshAgent _navmeshAgent = null;

	Player[] PlayersInRange = new Player[4];
	Player[] PlayersInView = new Player[4];
	Player Target = null;

	void Awake()
	{
		_capsule = GetComponent<CapsuleCollider>();
		_animator = GetComponent<Animator>();
		_navmeshAgent = GetComponent<NavMeshAgent>();

		_acceleration = _navmeshAgent.acceleration;
		_deAcceleration = _navmeshAgent.acceleration * 30;
		_speed = _navmeshAgent.speed;
	}

	// Use this for initialization
	void Start () {	
		State = EnemyState.Idle;
	}
	
	// Update is called once per frame
	protected override void Update () {

		this._animator.SetFloat(ANIM_VELOCITY, this._navmeshAgent.velocity.magnitude);
		this._animator.SetFloat(ANIM_ATTACK_SPEED_MULTIPLIER, this.AttackSpeedMultiplier);
		this._animator.SetFloat(ANIM_WALK_SPEED_MULTIPLIER, this.WalkSpeedMultiplier);
		this._navmeshAgent.speed = this._speed * this.WalkSpeedMultiplier;


		if (this.State != EnemyState.Attacking){
			// Procura por jogadores no Range
			CheckForPlayerInRange();

			// Verifica Qual jogador pode ser Visto
			CheckForPlayerInView();

			// Jogadores com maior Aggro, somente se houver mais de 1
			Target = PlayerManager.PlayerWithMoreAggro(this.PlayersInView);

			// tem um alvo ?
			if (Target != null)
			{
				if (this._navmeshAgent.hasPath){

					// Se o remaining distance for infinito quer dizer que existe obstaculos, caso contrario rotaciona para o alvo
					if (!float.IsInfinity(this._navmeshAgent.remainingDistance))
					{
						/* Desenha uma linha reta pra saber se o path eh parecido com a linha reta */
						Vector3 _distance = Target.transform.position - this.transform.position;

						// SE a distance em linha reta entre os 2 pontos for menor que a distance que precisa percorrer nao atualiza a rotacao
						if (!(_distance.magnitude + 0.5f < this._navmeshAgent.remainingDistance))
							this.transform.LookAt(Target.transform.position);
					}

					if (this._navmeshAgent.remainingDistance < this._navmeshAgent.stoppingDistance + this.AttackDistance)
					{
						this._navmeshAgent.acceleration = (this._navmeshAgent.remainingDistance < _navmeshAgent.stoppingDistance + 1.5f) ? this._deAcceleration : this._acceleration;
					}
				}

				// Esta no Range para Ataque?
				if ((Target.transform.position - this.transform.position).magnitude <= AttackDistance)
				{
					this._navmeshAgent.velocity = Vector3.zero;
					this._navmeshAgent.ResetPath();
					this.transform.LookAt(Target.transform.position);

					// Ataca
					this._animator.SetTrigger(ANIM_ATTACK);
					this.State = EnemyState.Attacking;
				}
				else
				{
					this._navmeshAgent.SetDestination(Target.transform.position);
					// Move
				}
			}
			else
			{
				this._navmeshAgent.velocity = Vector3.zero;
				this._navmeshAgent.ResetPath();
			}
		}
	}

	void CheckForPlayerInRange()
	{
		Helper.ClearArrayElements(this.PlayersInRange);
		Player _player = null;

		for (int i = 0; i < PlayerManager.Instance.ActivePlayers.Count; i++)
		{
			_player = PlayerManager.Instance.ActivePlayers[i];
			Vector3 _distance = _player.transform.position - this.transform.position;

			if (_distance.magnitude <= this.AwareDistance)
			{
				this.PlayersInRange[Helper.GetFreePosition(this.PlayersInRange)] = _player;
			}
		}
	}

	void CheckForPlayerInView()
	{
		RaycastHit _hit;
		Ray _rayCast;
		Vector3 rayDirection;
		Player _player = null;

		Helper.ClearArrayElements(this.PlayersInView);

		for (int i = 0; i < this.PlayersInRange.Length; i++)
		{
			_player = this.PlayersInRange[i];

			if (_player != null)
			{
				rayDirection = (_player.transform.position + _capsule.center * 1.5f) - (transform.position + _capsule.center * 1.5f);
				_rayCast = new Ray(transform.position + _capsule.center * 1.5f, rayDirection);

				if (Physics.Raycast(_rayCast.origin, _rayCast.direction, out _hit, 500f))
				{
					if (_hit.transform == _player.transform)
					{
						this.PlayersInView[Helper.GetFreePosition(this.PlayersInView)] = _player;
					}
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
