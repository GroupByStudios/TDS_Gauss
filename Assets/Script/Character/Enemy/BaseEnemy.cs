using UnityEngine;
using System.Collections;
using System;
using System.Linq;


[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
public class BaseEnemy : Character
{
    // Animation Constants
    protected const string ANIM_VELOCITY = "Velocity";
    protected const string ANIM_ATTACK = "Attack";
    protected const string ANIM_DEAD = "Dead";
    protected const string ANIM_ATTACK_SPEED_MULTIPLIER = "AttackSpeed_Multiplier";
    protected const string ANIM_WALK_SPEED_MULTIPLIER = "WalkSpeed_Multiplier";

    // Animation Timing Variables
    public float AnimTimeAttackEvent = 0.7f;
    public float AnimTimeAttackFinish = 1.25f;
    public float AnimTimeDeathDisable = 1f;
    public float AnimTimeDeathStartDragDown = 5f;


    // Physics Variable;
    internal CapsuleCollider _capsule = null;

    // State Variables;
    public EnemyState State;
    public bool IsDead;

    // Behaviour Variables
    public LayerMask FOVLayer;
    public float AttackAngle = 90f;
    public float AttackDistance = 1.5f;
    public float WalkSpeedMultiplier = 1f;
    public float AttackSpeedMultiplier = 1f;
    public float AwareDistance = 15;
    public float RandomMove = 0.7f;
    public float Damage = 100f;

    // DragDown Variables;
    public bool DragDown = false;
    public float DragDownSpeed = 5f;

    // Player Variables;
    protected Player[] PlayersInRange = new Player[4];
    protected Player[] PlayersInView = new Player[4];
    protected Player Target = null;
    protected EnemyFOV[] PlayersInFOV = new EnemyFOV[4];

    protected Action OnInternalStart;
    protected Action OnInternalBeforeStateMachine;
    protected Action OnInternalCreatingState;
    protected Action OnInternalCreatedState;
    protected Action OnInternalSpawingState;
    protected Action OnInternalIdleState;
    protected Action OnInternalMovingState;
    protected Action OnInternalAttackingState;
    protected Action OnInternalDeadState;
    protected Action OnInternalAfterStateMachine;

    public Action<BaseEnemy> OnBeforeDie;
    public Action OnActivation;
    public Action OnDamageTaken;
    public Action OnDamageDid;

    public Action OnDragDownStarted;
    public Action OnDragDownFinished;
    public Action OnDisableComponents;


    void Awake()
    {

    }

    protected override void Start()
    {
        base.Start();
        _capsule = GetComponent<CapsuleCollider>();
        _animator = GetComponent<Animator>();
        CharacterType = ENUMERATORS.Character.CharacterTypeEnum.Enemy;

        if (OnInternalStart != null)
            OnInternalStart();
        else
            ChangeState(EnemyState.Creating);
    }

    protected override void Update()
    {
        if (OnInternalBeforeStateMachine != null)
            OnInternalBeforeStateMachine();

        if (this.IsDead)
            ChangeState(EnemyState.Dead);

        if ((this.State & EnemyState.Creating) == EnemyState.Creating)
        {
            if (OnInternalCreatingState != null)
                OnInternalCreatingState();
            else
                this.ChangeState(EnemyState.Created);
        }

        if ((this.State & EnemyState.Created) == EnemyState.Created)
        {
            if (OnInternalCreatedState != null)
                OnInternalCreatedState();
            else
                this.ChangeState(EnemyState.Spawning);

            if (this.OnActivation != null)
                this.OnActivation();
        }

        if ((this.State & EnemyState.Spawning) == EnemyState.Spawning)
        {
            if (OnInternalSpawingState != null)
                OnInternalSpawingState();
            else
                this.ChangeState(EnemyState.Idle);
        }

        if ((this.State & EnemyState.Idle) == EnemyState.Idle)
        {
            if (OnInternalMovingState != null)
                OnInternalMovingState();
        }

        if ((this.State & EnemyState.Moving) == EnemyState.Moving)
        {
            if (OnInternalMovingState != null)
                OnInternalMovingState();
        }

        if ((this.State & EnemyState.Attacking) == EnemyState.Attacking)
        {
            if (OnInternalAttackingState != null)
                OnInternalAttackingState();
        }

        if ((this.State & EnemyState.Dead) == EnemyState.Dead)
        {
            if (OnInternalDeadState != null)
                OnInternalDeadState();
            else
            {
                if (this.DragDown)
                    this.HideDeadEnemy();
            }
        }

        /*        switch (this.State)
                {
                    case EnemyState.Creating:

                        if (OnInternalCreatingState != null)
                            OnInternalCreatingState();
                        else
                            this.ChangeState(EnemyState.Created);

                        break;

                    case EnemyState.Created:

                        if (OnInternalCreatedState != null)
                            OnInternalCreatedState();
                        else
                            this.ChangeState(EnemyState.Spawning);

                        if (this.OnActivation != null)
                            this.OnActivation();

                        break;
                    case EnemyState.Spawning:

                        if (OnInternalSpawingState != null)
                            OnInternalSpawingState();
                        else
                            this.ChangeState(EnemyState.Idle);

                        break;
                    case EnemyState.Idle:

                        if (OnInternalIdleState != null)
                            OnInternalIdleState();

                        break;
                    case EnemyState.Moving:

                        if (OnInternalMovingState != null)
                            OnInternalMovingState();

                        break;
                    case EnemyState.Attacking:

                        if (OnInternalAttackingState != null)
                            OnInternalAttackingState();

                        break;

                    case EnemyState.Dead:

                        if (OnInternalDeadState != null)
                            OnInternalDeadState();
                        else
                        {
                            if (this.DragDown)
                                this.HideDeadEnemy();
                        }

                        break;
                }*/

        if (OnInternalAfterStateMachine != null)
            OnInternalAfterStateMachine();
    }

    public void ChangeState(EnemyState state_)
    {
        this.State = state_;
    }

    #region Check Player Methods

    /// <summary>
    /// Verifica se o jogador esta no range de ataque
    /// </summary>
    /// <param name="target_"></param>
    /// <returns></returns>
    protected bool CheckInAttackRange(Player target_)
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

    /// <summary>
    /// Verifica os jogadores no Range
    /// </summary>
    protected void CheckForPlayerInRange()
    {
        int _playerInFovIndex = 0;
        Player _player = null;
        Vector3 _distance;

        // Limpa os jogadores no raio de visao
        for (int i = 0; i < this.PlayersInFOV.Length; i++)
        {
            this.PlayersInFOV[i].ClearEnemyFOV();
        }

        // Calcula a distancia entre o inimigo e os jogadores
        if (PlayerManager.Instance != null)
        {
			for (int i = 0; i < PlayerManager.Instance.ActivePlayers.Count; i++)
            {
                _player = PlayerManager.Instance.ActivePlayers[i];
                _distance = _player.transform.position - this.transform.position;

                if (_distance.magnitude <= this.AwareDistance)
                {
                    this.PlayersInFOV[_playerInFovIndex].Target = _player;
                    this.PlayersInFOV[_playerInFovIndex].Distance = _distance.magnitude;
                    this.PlayersInFOV[_playerInFovIndex].Aggro = _player.Aggro.MaxWithModifiers;
                    _playerInFovIndex++;
                }
            }
        }
    }

    protected EnemyFOV[] GetPlayerWithMoreAggro()
    {
        return this.PlayersInFOV.OrderByDescending(p => p.Aggro).ThenBy(p => p.Distance).ToArray();
    }

    /// <summary>
    /// Verifica se o Jogador está no Raio de Visão
    /// </summary>
    protected void CheckForPlayerInView()
    {
        RaycastHit _hit;
        Ray _rayCast;
        Vector3 _rayDirection;

        for (int i = 0; i < this.PlayersInFOV.Length; i++)
        {
            if (this.PlayersInFOV[i].HasTarget)
            {
                _rayDirection = (this.PlayersInFOV[i].Target.transform.position + _capsule.center) - (transform.position + _capsule.center);
                _rayCast = new Ray(transform.position + _capsule.center, _rayDirection);

                // Adiciona 10% de distancia alem do Aware Distance pra verificar o hit
                if (Physics.Raycast(_rayCast.origin, _rayCast.direction, out _hit, this.AwareDistance * 1.1f, FOVLayer))
                {
                    if (_hit.transform != this.PlayersInFOV[i].Target.transform)
                    {
                        // Nao validou a visao para o jogador
                        this.PlayersInFOV[i].ClearEnemyFOV();
                    }
                }
                else
                {
                    // Nao validou a visao para o jogador
                    this.PlayersInFOV[i].ClearEnemyFOV();
                }
            }
        }
    }

    #endregion

    #region Damage Methods

	public override void ApplyDamage(Character damager_, ENUMERATORS.Combat.DamageType damageType_, ENUMERATORS.Player.PlayerClass classe)
    {
		ApplyDamage(damager_, damageType_, classe, -1);
    }

	public override void ApplyDamage(Character damager_, ENUMERATORS.Combat.DamageType damageType_, ENUMERATORS.Player.PlayerClass classe, float damage_)
    {
        if (!(damager_ is BaseEnemy))
        {
            if ((this.State & EnemyState.Dead) != EnemyState.Dead)
            {
                if (damage_ == -1)
					base.ApplyDamage(damager_, damageType_, classe);
                else
					base.ApplyDamage(damager_, damageType_, classe, damage_);

                // Verifica se a Vida é menor que Zero
                if (HitPoint.CurrentWithModifiers <= 0)
                {
                    if (OnBeforeDie != null)
                        OnBeforeDie(this);

                    Die();
                }
            }
        }
    }

    public override float CalculateDamage()
    {
        return base.CalculateDamage();
    }

    public override void Die()
    {
        ChangeState(EnemyState.Dead);
        this.IsDead = true;
        Invoke("StartDragDown", AnimTimeDeathStartDragDown);
    }

    #endregion

    #region Death / DragDown

    protected void StartDragDown()
    {
        this.DragDown = true;

        if (OnDragDownStarted != null)
            OnDragDownStarted();
    }
    
    private void HideDeadEnemy()
    {  
        // Puxa o inimigo para baixo para sumir da tela e 
        this.transform.Translate(Vector3.down * DragDownSpeed * Time.deltaTime);

        if (this.transform.position.y < -5f)
        {
            OnDragDownFinished();
        }
    }

    #endregion

    #region Gizmos

    void OnGizmosDraw()
    {

    }

    #endregion
}

[Flags]
public enum EnemyState
{
    Creating = 1,
    Created = 2,
    Spawning = 4,
    Idle = 8,
    Moving = 16,
    Attacking = 32,
    Dead = 64
}

public struct EnemyFOV
{
    public bool HasTarget
    {
        get
        {
            return Target != null;
        }
    }

    public float Distance;
    public float Aggro;
    public Player Target;

    public void ClearEnemyFOV()
    {
        this.Distance = float.MaxValue;
        this.Aggro = float.MinValue;
        this.Target = null;
    }
}
