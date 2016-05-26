using UnityEngine;
using System.Collections;


[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : Character {

	NavMeshAgent _navMeshAgent;

	public ENUMERATORS.Enemy.EnemyStateEnum EnemyState;
	public ENUMERATORS.Enemy.EnemyTypeEnum EnemyType;
	public ENUMERATORS.Enemy.EnemyAttackTypeEnum EnemyAttackType;

	public float AggroRadius;
	public int ProjectileTableId; // ID do projetil para ser utilizado
	public float RangedAttackCoolDown;
	public float RangedAttackRadius;
	public float MeleeAttackCoolDown;
	public float MeleeAttackRadius;
	public LayerMask RaycastPlayerLayerMask;
	public bool IsPlayerAggroRadius;
	public bool IsPlayerVisible;
	public bool IsPlayerRangedAttackRadius;
	public bool IsPlayerMeleeAttackRadius;

	private float _nextRangedAttackTime;

	/* Physics Definition */
	LayerMask LayerMaskPlayer;
	Collider[] _playerQuery;
	Collider[] _testSphereColliderResult;

	public Vector2 WalkingCoolDown;
	float _nextWalking;

	protected override void Start ()
	{
		base.Start ();

		// Ajusta os valores inicias do inimigo
		this.EnemyState = ENUMERATORS.Enemy.EnemyStateEnum.SearchingPlayer;
		this.CharacterType = ENUMERATORS.Character.CharacterTypeEnum.Enemy;

		// Ajusta o agente de navegacao
		_navMeshAgent = GetComponent<NavMeshAgent>();
		_navMeshAgent.speed = Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Speed].Max;

		// Cria o array de colliders para a pesquisa do jogador
		_testSphereColliderResult = new Collider[1];
		_playerQuery = new Collider[1];
		LayerMaskPlayer = 1 << LayerMask.NameToLayer("Player");
	}

	protected override void Update ()
	{
		base.Update ();


		switch(EnemyState)
		{
		case ENUMERATORS.Enemy.EnemyStateEnum.SearchingPlayer:

			if (!(EnemyAttackType == ENUMERATORS.Enemy.EnemyAttackTypeEnum.Stationary))
				Patrol(); // Patrula em busca do Jogador
			
			CheckPlayerProximity(); // Verifica se o Jogador esta no raio de proximidade

			if (IsPlayerVisible)
			{
				EnemyState = ENUMERATORS.Enemy.EnemyStateEnum.HasPlayer;
			}

			break;
		case ENUMERATORS.Enemy.EnemyStateEnum.HasPlayer:

			CheckPlayerProximity(); // Verifica se o Jogador esta no raio de proximidade

			if (!(EnemyAttackType == ENUMERATORS.Enemy.EnemyAttackTypeEnum.Stationary))
				MoveToAttack();
			else
				IsPlayerRangedAttackRadius = IsPlayerOnAttackDistance(RangedAttackRadius);
					
			Attack();

			if (!IsPlayerVisible)
			{
				EnemyState = ENUMERATORS.Enemy.EnemyStateEnum.SearchingPlayer;
			}
			break;
		}

		if (!_navMeshAgent.hasPath)
			_navMeshAgent.velocity = Vector3.zero;
	}

	public override void Die ()
	{
		base.Die ();
		Debug.Log("Inimigo Morto:" + gameObject.name);
	}

	void Patrol()
	{
		if (Time.time > _nextWalking)
		{
			if (!_navMeshAgent.hasPath){
				_navMeshAgent.SetDestination(new Vector3(Random.Range(-24, 24), 0, Random.Range(-24, 24)));
			}

			if (_navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete)
			{
				_nextWalking = Time.time + Random.Range(WalkingCoolDown.x, WalkingCoolDown.y);
			}
		}
	}

	void CheckPlayerProximity()
	{
		IsPlayerAggroRadius = false;
		IsPlayerVisible = false;
		_playerQuery[0] = null;

		Vector3 _localForwardPosition = GetForwardPosition;

		if (Physics.OverlapSphereNonAlloc(transform.position, AggroRadius, _playerQuery, LayerMaskPlayer) > 0)
		{
			IsPlayerAggroRadius = true;

			/*if (Physics.Raycast(_localForwardPosition, (ApplicationModel.Instance.CurrentPlayer.transform.position - _localForwardPosition) + Vector3.up, out _raycastHit, 500f, RaycastPlayerLayerMask))
			{
				if (_raycastHit.collider.gameObject.CompareTag(CONSTANTS.TAGS.PLAYER)){
					Debug.DrawLine(_localForwardPosition, _raycastHit.point, Color.green);
					IsPlayerVisible = true;
				}
				else{
					Debug.DrawLine(_localForwardPosition, _raycastHit.point, Color.red);
				}
			}*/
		}
	}

	void MoveToAttack()
	{
		IsPlayerMeleeAttackRadius = false;
		IsPlayerRangedAttackRadius = false;

		if (IsPlayerVisible)
		{
			/*Vector3 _lookAt = new Vector3(ApplicationModel.Instance.CurrentPlayer.transform.position.x, transform.position.y, ApplicationModel.Instance.CurrentPlayer.transform.position.z);
			this.transform.LookAt(_lookAt);

			switch(EnemyAttackType)
			{
			case ENUMERATORS.Enemy.EnemyAttackTypeEnum.Melee:
				IsPlayerMeleeAttackRadius = MovetoAttackDistanceRadius(MeleeAttackRadius);
				break;
			case ENUMERATORS.Enemy.EnemyAttackTypeEnum.Ranged:
				IsPlayerRangedAttackRadius = MovetoAttackDistanceRadius(RangedAttackRadius);
				break;
			}*/
		}
	}

    bool MovetoAttackDistanceRadius(float attackDistanceRadius_)
	{
		// Verifica se o jogador esta no range do attack a distancia
		if (!IsPlayerOnAttackDistance(attackDistanceRadius_))
		{
			//transform.Translate(Vector3.forward * Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Speed].MaxWithModifiers * Time.deltaTime);
			//_navMeshAgent.SetDestination(ApplicationModel.Instance.CurrentPlayer.transform.position);
			return false;
		}
		else
		{
			_rigidBody.velocity = Vector3.zero;
			_navMeshAgent.ResetPath();
			return true;
		}
	}

	void Attack()
	{
		switch(EnemyAttackType)
		{
		case ENUMERATORS.Enemy.EnemyAttackTypeEnum.Melee:
			break;
		case ENUMERATORS.Enemy.EnemyAttackTypeEnum.Ranged:
		case ENUMERATORS.Enemy.EnemyAttackTypeEnum.Stationary:

			if (IsPlayerRangedAttackRadius){
				if (Time.time > _nextRangedAttackTime)
				{
					Projectile _newProjectile = ApplicationModel.Instance.ProjectileTable[ProjectileTableId].Pool.GetFromPool() as Projectile;

					// Se for igual a nulo nao tem mais objetos no pool para recuperar, deve aguardar por um novo objeto ser liberado
					if (_newProjectile != null){
					
						_newProjectile.transform.position = GetForwardPosition;
//						_newProjectile.transform.rotation = new Quaternion(0, transform.transform.rotation.y, 0, 0);
						//_newProjectile.transform.rotation = transform.rotation;

						// Rotaciona o projetil
//						if (EnemyAttackType == ENUMERATORS.Enemy.EnemyAttackTypeEnum.Stationary)
						//{
						// _newProjectile.transform.LookAt(new Vector3(ApplicationModel.Instance.CurrentPlayer.transform.position.x, _newProjectile.transform.position.y, ApplicationModel.Instance.CurrentPlayer.transform.position.z));
						//}

						_newProjectile.Damager = this;
						_newProjectile.DamageType = ENUMERATORS.Combat.DamageType.Melee;

						_nextRangedAttackTime = Time.time + RangedAttackCoolDown;
					}
				}
			}

			break;
		}
	}


	bool IsPlayerOnAttackDistance(float attackDistanceRadius_)
	{
		Vector3 spherePosition = transform.position;

		if (EnemyAttackType == ENUMERATORS.Enemy.EnemyAttackTypeEnum.Melee)
			spherePosition += transform.forward;

		return Physics.OverlapSphereNonAlloc(spherePosition, attackDistanceRadius_, _testSphereColliderResult, LayerMaskPlayer) > 0;
	}

	void OnDrawGizmos()
	{
		// Desenha a Esfera do Aggro
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, AggroRadius);

		// Desenha os gizmos de range
		switch(EnemyAttackType)
		{
		case ENUMERATORS.Enemy.EnemyAttackTypeEnum.Melee:
			Gizmos.color = Color.magenta;
			Gizmos.DrawWireSphere(GetForwardPosition, MeleeAttackRadius);			
			break;
		case ENUMERATORS.Enemy.EnemyAttackTypeEnum.Ranged:
		case ENUMERATORS.Enemy.EnemyAttackTypeEnum.Stationary:
			Gizmos.color = Color.magenta;
			Gizmos.DrawWireSphere(transform.position, RangedAttackRadius);			
			break;
		}
	}
}
