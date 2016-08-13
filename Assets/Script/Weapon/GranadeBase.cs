using UnityEngine;
using System.Collections;

public class GranadeBase : PoolObject
{
    public LayerMask AffectedLayer;
    public float ExplodeInSecond = 3f;
    public float AccumulativeThrowForcePerSecond = 30f;
    public float ExplosionForce = 500f;
    public float ExplosionRadius = 5f;
    public float ExplosionDamage = 100f;
    public bool DebugDrawGizmo = false;

    public float TorqueVerticalMinForce = 3000;
    public float TorqueVerticalMaxForce = 5000;

    public float ExplosionYOffSet = 0f;
    public bool ForceExplosion = false;

    public GameObject ExplosionParticlePrefab;

    private Rigidbody myRigidBody;
    private Renderer myRenderer;

    private float _currentThrowForce;
    private bool _throwed;
    private float _timeToExplode;
    private bool _armed;
    private Collider[] _colliders = new Collider[100];
    private Rigidbody _affectedRigidBody;
    private BaseEnemy _affectedCharacter;

    public Color ArmedColorOne;
    public Color ArmedColorTwo;
    public float ArmedColorSwitch = 0.25f;
    private float CurrentArmedColorSwitch = 0f;
    private byte ColorMode = 1;
    private Material EmissiveShader;

    [HideInInspector]
    Character ThrownByCharacter;

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (enabled)
        {
            ChangeColor();
            Explode(ForceExplosion);
        }
    }

    public override void ObjectAddedToPool()
    {
        if (myRenderer == null) myRenderer = GetComponent<Renderer>();
        if (myRigidBody == null) myRigidBody = GetComponent<Rigidbody>();

        if (myRenderer != null && myRenderer.materials.Length == 2)
        {
            EmissiveShader = myRenderer.materials[1];
        }
    }

    public virtual void CookGranade()
    {
        myRigidBody.isKinematic = true;
        _currentThrowForce += AccumulativeThrowForcePerSecond * Time.deltaTime;
    }

    public virtual void ThrowGranade(Vector3 WorldDirection_)
    {
        if (!_throwed)
        {

            myRigidBody.isKinematic = false;

            _armed = true;
            _timeToExplode = Time.time + ExplodeInSecond;

            myRenderer.enabled = true;
            _throwed = true;

            _currentThrowForce = Mathf.Clamp(_currentThrowForce, 3, 12);

            this.transform.LookAt(WorldDirection_);
            myRigidBody.AddForce(Vector3.up * Mathf.Clamp(_currentThrowForce, 0, 3), ForceMode.Impulse);
            myRigidBody.AddForce(WorldDirection_ * _currentThrowForce, ForceMode.Impulse);
            myRigidBody.AddTorque(this.transform.right * Random.Range(TorqueVerticalMinForce, TorqueVerticalMaxForce));
        }
    }

    public void ChangeColor()
    {
        CurrentArmedColorSwitch += Time.deltaTime;

        if (EmissiveShader != null && CurrentArmedColorSwitch > ArmedColorSwitch)
        {
            if (ColorMode % 2 == 0)
            {
                EmissiveShader.SetColor("_TintColor", ArmedColorOne);
            }
            else
            {
                EmissiveShader.SetColor("_TintColor", ArmedColorTwo);
            }

            ColorMode++;
            if (ColorMode > 2)
                ColorMode = 1;

            CurrentArmedColorSwitch = 0;
        }
    }

    /// <summary>
    /// Metodo responsavel por verificar se é necessário explodir e executar a explosao
    /// </summary>
    public virtual void Explode(bool forceExplosion)
    {
        if (_armed && _timeToExplode < Time.time || forceExplosion)
        {
            // Toca o som de explosao

            // Apresenta particulas de explosao
            if (ExplosionParticlePrefab != null)
            {
                GameObject explosionParticle = GameObject.Instantiate(ExplosionParticlePrefab);
                explosionParticle.transform.position = transform.position;
                GameObject.Destroy(explosionParticle, 5f);
            }

            ApplicationModel.Instance.ShakeCamera();

            // Verifica as colisoes
            int _hits = Physics.OverlapSphereNonAlloc(transform.position, ExplosionRadius, _colliders, AffectedLayer);

            if (_hits > 0)
            {
                for (int i = 0; i < _hits; i++)
                {
                    // Recupera os componentes
                    _affectedCharacter = _colliders[i].GetComponent<BaseEnemy>();
                    _affectedRigidBody = _colliders[i].attachedRigidbody;

                    if (_affectedCharacter != null)
                    {
                        // Aplica dano se for personagem
                        _affectedCharacter.ApplyDamage(ThrownByCharacter, ENUMERATORS.Combat.DamageType.Melee, ExplosionDamage);
                    }

                    if (_affectedRigidBody != null)
                    {
                        // Aplica a forca de explosao nos rigidbodys
                        _affectedRigidBody.AddExplosionForce(ExplosionForce, transform.position + Vector3.up * ExplosionYOffSet, ExplosionRadius);
                    }
                }
            }

            ReturnToPool();
        }
    }

    public override void ObjectActivated()
    {
        base.ObjectActivated();

        myRenderer.enabled = false;
        myRigidBody.velocity = Vector3.zero;
    }

    public override void ObjectDeactivated()
    {
        _armed = false;
        _throwed = false;
        _timeToExplode = 0f;
        _currentThrowForce = 0f;

        base.ObjectDeactivated();
    }

    protected virtual void OnDrawGizmos()
    {
        if (DebugDrawGizmo)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * ExplosionYOffSet, ExplosionRadius);
        }
    }
}
