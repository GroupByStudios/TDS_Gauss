using UnityEngine;
using System.Collections;

public class Projectile : PoolObject {

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

	protected virtual void Awake()
	{
		myTransform = GetComponent<Transform>();
		raycastHits = new RaycastHit[10];
	}

	// Use this for initialization
	protected virtual void Start () {

	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update();

		CheckCollision();
		MoveProjectile();
	}

	/// <summary>
	/// Metodo responsavel por movimentar o projetil
	/// </summary>
	protected virtual void MoveProjectile() {
		myTransform.position = transform.position + (transform.forward * Speed * Time.deltaTime);
	}


	/// <summary>
	/// Metodo responsavel por verificar se IRA colidir caso seja movido
	/// </summary>
	protected virtual void CheckCollision()
	{
		int _hits;
		float _moveDistance = Speed * Time.deltaTime;

		_hits = Physics.RaycastNonAlloc(transform.position, transform.forward, raycastHits, _moveDistance, CollisionLayer);

		if (_hits > 0){
			for(int i = 0; i < _hits; i++)
			{
				if (raycastHits[i].collider != null)
				{
					Character _characterDamaged = raycastHits[i].collider.GetComponent<Character>();
					if (_characterDamaged != null){
						_characterDamaged.ApplyDamage(Damager, DamageType);
						PlayImpactSound();
					}

					base.ReturnToPool();
					return;

				}
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

	public override void ObjectActivated ()
	{
		base.ObjectActivated ();
		SetExpireTime(5);
	}
}
