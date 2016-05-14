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
	const string ANIM_ON_AIR = "OnAir";
	const string ANIM_ON_LANDED = "OnLanded";

	public bool IsDead;

	public LayerMask FOVLayer;
	public EnemyState State;
	public float AttackAngle = 90f;
	public float AttackDistance = 1.5f;
	public float WalkSpeedMultiplier = 1f;
	public float AttackSpeedMultiplier = 1f;
	public float AwareDistance = 15;
	public float RandomMove = 0.7f;
	public float Damage = 100f;
	public float HitPoint = 200f;

	/* Queue? */
	bool _finishLandedQueued = false;
	bool _attackEventQueued = false;
	bool _attackFinishQueued = false;

	/* Tempo de Animacoes */
	public float AnimTimeAttackEvent = 0.7f;
	public float AnimTimeAttackFinish = 1.25f;
	public float AnimTimeDeathDisable = 1f;
	public float AnimTimeDeathStartDragDown = 5f;
	public float AnimTimeFinishLanding = 1.5f;

	Vector3 _lastFloorPosition;
	RaycastHit _floor;
	public bool DragDown = false;
	public float DragDownSpeed = 5f;

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

		_navmeshAgent.enabled = false;
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
		this._animator.SetBool(ANIM_ON_AIR, false);
		this._navmeshAgent.speed = this._speed * this.WalkSpeedMultiplier;

		if (this.IsDead)
			this.State = EnemyState.Dead;

		switch(this.State)
		{
		case EnemyState.Creating:
			this.State = EnemyState.Created;
			break;
		case EnemyState.Created:

			this._navmeshAgent.enabled = false;
			this.transform.position = new Vector3(this.transform.position.x, 20f, this.transform.position.z);
			this.State = EnemyState.Spawning;

			if (this.OnActivated != null)
				this.OnActivated(this);

			break;
		case EnemyState.Spawning:

			if (!_finishLandedQueued)
			{
				/* Verifica se Esta tocando o chao */
				if (Helper.DistanceFromFloor(this.transform.position, out _floor))
				{
					_lastFloorPosition = _floor.transform.position;
					if (_floor.distance >= 0.01)
					{
						this._animator.SetBool(ANIM_ON_AIR, true);
						//this.transform.position = Vector3.Lerp(this.transform.position, new Vector3(this.transform.position.x, _floor.transform.position.y, this.transform.position.z), 0.1f);
						this.transform.Translate(Vector3.down * 25 * Time.deltaTime);
					}
					else
					{
						this._animator.SetBool(ANIM_ON_LANDED, true);
						this.transform.position = new Vector3(this.transform.position.x, _lastFloorPosition.y + 0.01f, this.transform.position.z);
						this._navmeshAgent.enabled = true;
						Invoke("FinishLanding", AnimTimeFinishLanding);
						_finishLandedQueued = true;
					}
				}
				else
				{
					this._animator.SetBool(ANIM_ON_LANDED, true);
					this.transform.position = new Vector3(this.transform.position.x, _lastFloorPosition.y + 0.01f, this.transform.position.z);
					this._navmeshAgent.enabled = true;
					Invoke("FinishLanding", AnimTimeFinishLanding);
					_finishLandedQueued = true;
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

			if (!this._attackEventQueued && !this._attackFinishQueued)
			{
				Invoke("Attack_Event", AnimTimeAttackEvent);
				this._attackEventQueued = true;
			}

			if (!_attackFinishQueued)
			{
				Invoke("Attack_Finished", AnimTimeAttackFinish);
				this._attackFinishQueued = true;
			}

			break;
		case EnemyState.Dead:

			if (this.DragDown)
				this.HideDeadEnemy();

			break;
		}
	}
		
	public void ChangeState(EnemyState state_)
	{
		this.State = state_;
	}

	public void FinishLanding()
	{
		if (this.State != EnemyState.Dead)
			this.State = EnemyState.Idle;
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


			if (this.State != EnemyState.Dead)
			{
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

				if (Physics.Raycast(_rayCast.origin, _rayCast.direction, out _hit, 500f, FOVLayer))
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
		if (this.State >= EnemyState.Idle &&
			this.State <= EnemyState.Attacking)
		{
			this.HitPoint -= damage_;

			if (this.HitPoint <= 0){
				this._navmeshAgent.velocity = Vector3.zero;
				this._navmeshAgent.ResetPath();
				this.State = EnemyState.Dead;
				this._animator.SetTrigger(ANIM_DEAD);
				this.IsDead = true;
				Invoke("OnDeath_DisabledComponents", AnimTimeDeathDisable); 
				Invoke("StartDragDown", AnimTimeDeathStartDragDown); 
				if (this.OnDie != null)
					this.OnDie(this);
			}
		}
	}

	public void OnDeath_DisabledComponents()
	{
		this._capsule.enabled = false;
	}

	public void StartDragDown()
	{
		this.DragDown = true;
		this._navmeshAgent.enabled = false;
	}

	// Evento de Animacao para executar o Hit
	void Attack_Event()
	{
		// Garante que o Jogador continua no range do ataque 
		if (CheckInAttackRange(this.Target))
		{
			if (this.Target != null)
			{
				this.Target.ApplyDamage(null, this.Damage, ENUMERATORS.Combat.DamageType.Melee);
			}
		}

		this._attackEventQueued = false;
	}

	// Evento de Animacao do Hit finalizado
	void Attack_Finished()
	{
		if (this.State != EnemyState.Dead)
			this.State = EnemyState.Idle;

		this._attackFinishQueued = false;
	}

	void OnDrawGizmos()
	{
		if (_capsule == null)
			_capsule = GetComponent<CapsuleCollider>();

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position + _capsule.center, AttackDistance);
	}


	#region Eventos 

	public delegate void Die(BaseEnemy enemy_);
	public delegate void Activated(BaseEnemy enemy_);
	public event Die OnDie;
	public event Activated OnActivated;

	#endregion

}

public enum EnemyState
{
	Creating	= 0,
	Created		= 2,
	Spawning	= 4,
	Idle		= 8,
	Walking		= 16,
	Attacking	= 32,
	Dead		= 64
}
