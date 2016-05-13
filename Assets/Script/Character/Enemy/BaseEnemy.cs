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
	public float AttackAngle = 90f;
	public float AttackDistance = 1.5f;
	public float WalkSpeedMultiplier = 1f;
	public float AttackSpeedMultiplier = 1f;
	public float AwareDistance = 15;
	public float RandomMove = 0.7f;
	public float Damage = 100f;
	public float HitPoint = 200f;
	public bool DragDown = false;
	public float DragDownSpeed = 5f;

	private float _acceleration;
	private float _deAcceleration;
	private float _speed;

	private float _attackStateTimes = 0; // GAMBIARRA para corrigir o soluco de animacao que nao dispara evento, alterar para coroutina por tempo ?

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
		State = EnemyState.Creating;
	}
	
	// Update is called once per frame
	protected override void Update () {

		this._animator.SetFloat(ANIM_VELOCITY, this._navmeshAgent.velocity.magnitude);
		this._animator.SetFloat(ANIM_ATTACK_SPEED_MULTIPLIER, this.AttackSpeedMultiplier);
		this._animator.SetFloat(ANIM_WALK_SPEED_MULTIPLIER, this.WalkSpeedMultiplier);
		this._navmeshAgent.speed = this._speed * this.WalkSpeedMultiplier;

		switch(this.State)
		{
		case EnemyState.Creating:
			this.State = EnemyState.Created;
			break;
		case EnemyState.Created:

			this._navmeshAgent.enabled = false;
			this.transform.position = new Vector3(this.transform.position.x, 20f, this.transform.position.z);
			this.State = EnemyState.Spawning;

			break;
		case EnemyState.Spawning:

			/* Verifica se Esta tocando o chao */
			RaycastHit _floor;
			if (Helper.DistanceFromFloor(this.transform.position, out _floor))
			{
				if (_floor.distance > 0.25f)
				{
					this.transform.position = Vector3.Lerp(this.transform.position, _floor.point, 0.25f);
				}
				else
				{
					this._navmeshAgent.enabled = true;
					this.State = EnemyState.Idle;
				}
			}

			break;
		case EnemyState.Idle:
			SeekAndDestroy();
			break;
		case EnemyState.Walking:
			SeekAndDestroy();
			break;
		case EnemyState.Attacking:
			/* GAMBIARRA, alterar os ventos de animacao para coroutines */
			_attackStateTimes++;
			if (_attackStateTimes == 20000)
			{
				this.State = EnemyState.Idle;
				_attackStateTimes = 0;
			}
			/* GAMBIARRA, alterar os ventos de animacao para coroutines */
			break;
		case EnemyState.Dead:

			if (this.DragDown)
				this.HideDeadEnemy();

			break;
		}
	}

	public void HideDeadEnemy()
	{
		// Puxa o inimigo para baixo para sumir da tela e 
		this.transform.Translate(Vector3.down * DragDownSpeed * Time.deltaTime);

		if (this.transform.position.y < -5f)
			Destroy(this.gameObject); // TODO DEVE RETORNAR O INIMIGO PARA O POOL
	}

	public void SeekAndDestroy()
	{
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

				// Acelera e desalecera o agente da navmesh pra evitar que o a gente deslize
				if (this._navmeshAgent.remainingDistance < this._navmeshAgent.stoppingDistance + this.AttackDistance)
				{
					this._navmeshAgent.acceleration = (this._navmeshAgent.remainingDistance < _navmeshAgent.stoppingDistance + 1.5f) ? this._deAcceleration : this._acceleration;
				}
			}

			// Esta no Range para Ataque?
			if (CheckInAttackRange(Target))
			{
				// Remove a velocidade do agente, reseta o path do a gente e rotaciona para o ataque
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
			}
		}
		else
		{
			// se nao tem alvo zera a velocidade e reseta o path
			this._navmeshAgent.velocity = Vector3.zero;
			this._navmeshAgent.ResetPath();
		}
	}

	// Verifica se o jogador esta no range de ataque
	bool CheckInAttackRange(Player target_)
	{
		// Verifica que o target existe
		if (target_ != null)
		{
			// Verifica se esta na distancia de Ataque
			if ((target_.transform.position - this.transform.position).magnitude <= this.AttackDistance)
			{
				// Verifica se esta no angulo de Ataque
				if (Vector3.Angle(this.transform.forward, target_.transform.position - this.transform.position) <= this.AttackAngle)
					return true;
			}
		}

		return false;
	}

	// Verifica os jogadores no Range
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

	// Verifica os jogadores que podem ser vistos
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

	public void ApplyDamage(float damage_)
	{
		if (this.State != EnemyState.Dead)
		{
			this.HitPoint -= damage_;

			if (this.HitPoint <= 0){
				this._navmeshAgent.velocity = Vector3.zero;
				this._navmeshAgent.ResetPath();
				this.State = EnemyState.Dead;
				this._animator.SetTrigger(ANIM_DEAD);
				Invoke("StartDragDown", 5f);

			}
		}
	}

	public void StartDragDown()
	{
		this.DragDown = true;
		this._capsule.enabled = false;
		this._navmeshAgent.enabled = false;
	}

	// Evento de Animacao para executar o Hit
	void Hit_Event()
	{
		// Garante que o Jogador continua no range do ataque 
		if (CheckInAttackRange(this.Target))
		{
			if (this.Target != null)
			{
				this.Target.ApplyDamage(null, this.Damage, ENUMERATORS.Combat.DamageType.Melee);
			}
		}
	}

	// Evento de Animacao do Hit finalizado
	void Hit_Finished()
	{
		if (this.State != EnemyState.Dead)
			this.State = EnemyState.Idle;
	}

	void OnDrawGizmos()
	{
		if (_capsule == null)
			_capsule = GetComponent<CapsuleCollider>();

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position + _capsule.center, AttackDistance);
	}


	#region Eventos 

	public delegate void Die();
	public delegate void Activated();
	public delegate void Attacking();
	public event Die OnDie;
	public event Activated OnActivated;
	public event Attacking OnAttacking;

	#endregion

}

public enum EnemyState
{
	Creating,
	Created,
	Spawning,
	Idle,
	Walking,
	Attacking,
	Dead
}
