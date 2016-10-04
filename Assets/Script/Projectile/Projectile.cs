using UnityEngine;
using System.Collections;

public class Projectile : PoolObject
{

    public int ID;
    public float Speed;
    public ENUMERATORS.Combat.DamageType DamageType;
    public bool LiveAfterHit;
    [HideInInspector]
    public Character Damager;
    public float Damage;
    public LayerMask CollisionLayer;
    [HideInInspector]
    public Transform myTransform;
    public AudioClip[] ImpactSounds;

    private RaycastHit[] raycastHits;
    private float _moveDistance;

    protected virtual void Awake()
    {
        myTransform = GetComponent<Transform>();
        raycastHits = new RaycastHit[10];
    }

    // Use this for initialization
    protected virtual void Start()
    {

    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        //_moveDistance = Speed * Time.deltaTime;
        _moveDistance = Speed * Time.deltaTime;

        CheckCollision();

        if (this.gameObject.activeInHierarchy)
            MoveProjectile();
    }

    /// <summary>
    /// Metodo responsavel por movimentar o projetil
    /// </summary>
    protected virtual void MoveProjectile()
    {
        //myTransform.position = transform.position + (transform.forward * _moveDistance);
        myTransform.Translate(Vector3.forward * _moveDistance);
    }


    /// <summary>
    /// Metodo responsavel por verificar se IRA colidir caso seja movido
    /// </summary>
    protected virtual void CheckCollision()
    {
        Helper.ClearArrayElements<RaycastHit>(raycastHits);
        Physics.RaycastNonAlloc(transform.position, transform.forward, raycastHits, _moveDistance, CollisionLayer.value);

        //Debug.DrawRay(transform.position, (transform.forward * _moveDistance) * 2, Color.red);

        for (int i = 0; i < raycastHits.Length; i++)
        {
            if (raycastHits[i].collider != null)
            {
                Character _characterDamaged = raycastHits[i].collider.GetComponent<Character>();

                if (_characterDamaged is Player && Damager is Player)
                    continue;

                if (_characterDamaged is BaseEnemy && Damager is BaseEnemy)
                    continue;

                if (_characterDamaged != null)
                {
                    if (Damager is Player)
                    {
                        ((Player)Damager).AggroUp();
						((BaseEnemy)_characterDamaged).ApplyDamage(Damager, DamageType, ENUMERATORS.Player.PlayerClass.ROBOT);
                    }
                    else if (Damager is BaseEnemy)
                    {
						((Player)_characterDamaged).ApplyDamage(Damager, DamageType, ENUMERATORS.Player.PlayerClass.ROBOT);
                    }
                    //PlayImpactSound();
                }

                base.ReturnToPool();
                return;
            }
        }
    }

    protected virtual void PlayImpactSound()
    {
        if (ImpactSounds != null && ImpactSounds.Length > 0)
        {
            ApplicationModel.Instance.myAudioManager.PlayClip(ImpactSounds[Random.Range(0, ImpactSounds.Length - 1)]);
        }
    }

    public override void ObjectActivated()
    {
        base.ObjectActivated();
        SetExpireTime(5);
    }
}
