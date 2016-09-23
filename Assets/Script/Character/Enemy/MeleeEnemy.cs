using UnityEngine;
using System.Linq;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class MeleeEnemy : BaseEnemy
{
	public AudioClip[] audio;
	private const string ANIM_ON_AIR = "OnAir";
    private const string ANIM_ON_LANDED = "OnLanded";

    public float AnimTimeFinishLanding = 1.5f;
    public float MeleeDamage;

    /* Queue? */
    bool _finishLandedQueued = false;
    bool _attackEventQueued = false;
    bool _attackFinishQueued = false;
    public bool IsRagDoll = false;

    Vector3 _lastFloorPosition;
    RaycastHit _floor;

    private float _acceleration;
    private float _deAcceleration;
    private float _speed;

    NavMeshAgent _navmeshAgent = null;
    Renderer _renderer = null;
    Material _emissiveMat = null;

    void Awake()
    {
        _navmeshAgent = GetComponent<NavMeshAgent>();

        _navmeshAgent.enabled = false;
        _acceleration = _navmeshAgent.acceleration;
        _deAcceleration = _navmeshAgent.acceleration * 30;
        _speed = _navmeshAgent.speed;

        /* Wrap the events */
        base.OnInternalStart += this.InternalStart;
        base.OnInternalBeforeStateMachine += this.InternalBeforeStateMachine;
        base.OnInternalCreatingState += this.InternalCreatingState;
        base.OnInternalCreatedState += this.InternalCreatedState;
        base.OnInternalSpawingState += this.InternalSpawingState;
        base.OnInternalIdleState += this.InternalIdleState;
        base.OnInternalMovingState += this.InternalMovingState;
        base.OnInternalAttackingState += this.InternalAttackingState;
        base.OnInternalDeadState += this.InternalDeadState;

        /* Misc Events */
        base.OnBeforeDie += BeforeDie;
        base.OnDragDownStarted += DragDownStarted;
        base.OnDragDownFinished += DragDownFinished;
    }

    #region StateMachine Override

    private void InternalStart()
    {
        State = EnemyState.Creating;

        _renderer = GetComponentInChildren<Renderer>();
        if (_renderer != null && _renderer.materials.Length > 1)
        {
            _emissiveMat = _renderer.materials[1];
        }
    }

    private void InternalBeforeStateMachine()
    {
        this._animator.SetFloat(ANIM_VELOCITY, this._navmeshAgent.velocity.magnitude);
        this._animator.SetFloat(ANIM_ATTACK_SPEED_MULTIPLIER, this.AttackSpeedMultiplier);
        this._animator.SetFloat(ANIM_WALK_SPEED_MULTIPLIER, this.WalkSpeedMultiplier);
        this._animator.SetBool(ANIM_ON_AIR, false);
        this._navmeshAgent.speed = this._speed * this.WalkSpeedMultiplier;

        if (this.IsDead)
            this.State = EnemyState.Dead;
		
    }

    private void InternalCreatingState()
    {
        this.State = EnemyState.Created;
    }

    private void InternalCreatedState()
    {
        this._navmeshAgent.enabled = false;
        this.transform.position = new Vector3(this.transform.position.x, 20f, this.transform.position.z);
        this.State = EnemyState.Spawning;
    }

    private void InternalSpawingState()
    {
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
					GetComponent<AudioSource> ().PlayOneShot (audio[2]);
                }
            }
            else
            {
                this._animator.SetBool(ANIM_ON_LANDED, true);
                this.transform.position = new Vector3(this.transform.position.x, _lastFloorPosition.y + 0.01f, this.transform.position.z);
                this._navmeshAgent.enabled = true;
                Invoke("FinishLanding", AnimTimeFinishLanding);
                _finishLandedQueued = true;
				GetComponent<AudioSource> ().PlayOneShot (audio[2]);
            }
        }
    }

    private void InternalIdleState()
    {
        SeekAndDestroy();
    }

    private void InternalMovingState()
    {
        SeekAndDestroy();
    }

    private void InternalAttackingState()
    {
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
    }

    private void InternalDeadState()
    {
        if (_emissiveMat != null)
        {
            _emissiveMat.SetColor("_EmissionColor", Color.Lerp(_emissiveMat.GetColor("_EmissionColor"), Color.black, 0.2f));
        }
    }

    #endregion

    #region Events 

    private void BeforeDie(BaseEnemy baseEnemy_)
    {
        this._capsule.enabled = false;
        this._navmeshAgent.velocity = Vector3.zero;

        if (this._navmeshAgent.hasPath)
            this._navmeshAgent.ResetPath();

        if (!IsRagDoll)
        {
            this._animator.SetTrigger(ANIM_DEAD);
			GetComponent<AudioSource> ().PlayOneShot (audio[0]);
        }
        else
        {
            this._animator.enabled = false;
			GetComponent<AudioSource> ().PlayOneShot (audio[0]);
        }

        this._navmeshAgent.enabled = false;
    }

    Collider[] _colliders;
    private void DragDownStarted()
    {
        /* Desabilita todos os Colliders */
        _colliders = GetComponentsInChildren<Collider>(true);
        for (int i = 0; i < _colliders.Length; i++)
        {
            _colliders[i].enabled = false;
        }
    }
    private void DragDownFinished()
    {
        Destroy(this.gameObject);// Implementar o conceito de Pool
    }

    #endregion

    public void FinishLanding()
    {
        if (this.State != EnemyState.Dead)
            this.State = EnemyState.Idle;
    }

    public void SeekAndDestroy()
    {
        // Procura por jogadores no Range
        CheckForPlayerInRange();

        // Verifica Qual jogador pode ser Visto
        CheckForPlayerInView();

        // Ordena o Array pelos Jogadores com maior Aggro e Menor Distancia
        this.PlayersInFOV = GetPlayerWithMoreAggro();

        Target = this.PlayersInFOV[0].Target;

        // tem um alvo ?
        if (Target != null)
        {
            if (this._navmeshAgent.hasPath)
            {
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
					GetComponent<AudioSource> ().PlayOneShot (audio[1]);
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
            if (this._navmeshAgent.enabled && this._navmeshAgent.hasPath)
            {
                this._navmeshAgent.velocity = Vector3.zero;
                this._navmeshAgent.ResetPath();
            }
        }
    }

    // Evento de Animacao para executar o Hit
    void Attack_Event()
    {
        // Garante que o Jogador continua no range do ataque 
        if (CheckInAttackRange(this.Target))
        {
            if (this.Target != null)
            {
                this.Target.ApplyDamage(this, ENUMERATORS.Combat.DamageType.Melee);
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

    public override float CalculateDamage()
    {
        return MeleeDamage;
    }
}