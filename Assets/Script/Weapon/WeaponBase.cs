using UnityEngine;
using System.Collections;

public class WeaponBase : MonoBehaviour {

	[HideInInspector] public Character WeaponOwner;

	public Vector3 DefaultPosition;
	public Vector3 DefaultRotation;

	public int WeaponID;
	public int WeaponTypeID;
	public string WeaponName;
	public bool Automatic;
	public int ProjectileID;
    public bool InfiniteAmmo;

	public AttributeBase[] Attributes = WeaponBase.InitializeAttributes();
	[HideInInspector][System.NonSerialized] public AttributeModifier[] AttributeModifiers = new AttributeModifier[CONSTANTS.ATTRIBUTES.ATTRIBUTE_MODIFIERS_COUNT];
	public Effects Effects;

	public int ShootAnimFrames;
	[HideInInspector] public float ShootAnimMultiplier;
	public int ReloadAnimFrames;
	[HideInInspector] public float ReloadAnimMultiplier;

	public int ClipSize;

	public AudioClip[] ShootAudio;
	public AudioClip[] ReloadAudio;
	public GameObject Muzzle;

	[HideInInspector] public float CoolDown;
	[HideInInspector] public float ReloadCoolDown;
	[HideInInspector] public bool IsShooting;
	[HideInInspector] public bool IsReloading;

	[HideInInspector]
	public bool RenderWeapon = true;
	private Renderer _renderer;

	protected float _nextShootCoolDown;
	protected float _nextShootAfterReloadCoolDown;

	public virtual void Awake()
	{
		//DefaultPosition = transform.position;
		//DefaultRotation = transform.rotation;	
		Effects = new Effects();
		_renderer = GetComponent<Renderer>();
		if (_renderer == null)
			_renderer = GetComponentInChildren<Renderer>();
	}

	private static AttributeBase[] InitializeAttributes()
	{
		AttributeBase[] _tempCharAttribute = new AttributeBase[CONSTANTS.ATTRIBUTES.WEAPON_ATTRIBUTE_COUNT];

		_tempCharAttribute[(int)ENUMERATORS.Attribute.WeaponAttributeTypeEnum.Damage] = 
			new AttributeBase(){ 			
			Name = CONSTANTS.ATTRIBUTES.WEAPON_ATTRIBUTE_NAMES[(int)ENUMERATORS.Attribute.WeaponAttributeTypeEnum.Damage], 
			AttributeType = (int)ENUMERATORS.Attribute.WeaponAttributeTypeEnum.Damage,
			Max = 0,
			MaxModifiers = 0,
			Current = 0,
			CurrentModifiers = 0,
			DisplayOrder = 0,
			AttributeBaseType = ENUMERATORS.Attribute.AttributeBaseTypeEnum.Weapon};

		_tempCharAttribute[(int)ENUMERATORS.Attribute.WeaponAttributeTypeEnum.Ammo] = 
			new AttributeBase(){ 			
			Name = CONSTANTS.ATTRIBUTES.WEAPON_ATTRIBUTE_NAMES[(int)ENUMERATORS.Attribute.WeaponAttributeTypeEnum.Ammo], 
			AttributeType = (int)ENUMERATORS.Attribute.WeaponAttributeTypeEnum.Ammo,
			Max = 0,
			MaxModifiers = 0,
			Current = 0,
			CurrentModifiers = 0,
			DisplayOrder = 0,
			AttributeBaseType = ENUMERATORS.Attribute.AttributeBaseTypeEnum.Weapon};	

		_tempCharAttribute[(int)ENUMERATORS.Attribute.WeaponAttributeTypeEnum.CriticChance] = 		
			new AttributeBase(){ 			
			Name = CONSTANTS.ATTRIBUTES.WEAPON_ATTRIBUTE_NAMES[(int)ENUMERATORS.Attribute.WeaponAttributeTypeEnum.CriticChance], 
			AttributeType = (int)ENUMERATORS.Attribute.WeaponAttributeTypeEnum.CriticChance,
			Max = 0,
			MaxModifiers = 0,
			Current = 0,
			CurrentModifiers = 0,
			DisplayOrder = 0,
			AttributeBaseType = ENUMERATORS.Attribute.AttributeBaseTypeEnum.Weapon};

		_tempCharAttribute[(int)ENUMERATORS.Attribute.WeaponAttributeTypeEnum.CriticMultiplier] = 
			new AttributeBase(){ 			
			Name = CONSTANTS.ATTRIBUTES.WEAPON_ATTRIBUTE_NAMES[(int)ENUMERATORS.Attribute.WeaponAttributeTypeEnum.CriticMultiplier], 
			AttributeType = (int)ENUMERATORS.Attribute.WeaponAttributeTypeEnum.CriticMultiplier,
			Max = 0,
			MaxModifiers = 0,
			Current = 0,
			CurrentModifiers = 0,
			DisplayOrder = 0,
			AttributeBaseType = ENUMERATORS.Attribute.AttributeBaseTypeEnum.Weapon};

		_tempCharAttribute[(int)ENUMERATORS.Attribute.WeaponAttributeTypeEnum.FireRate] = 
			new AttributeBase(){ 			
			Name = CONSTANTS.ATTRIBUTES.WEAPON_ATTRIBUTE_NAMES[(int)ENUMERATORS.Attribute.WeaponAttributeTypeEnum.FireRate], 
			AttributeType = (int)ENUMERATORS.Attribute.WeaponAttributeTypeEnum.FireRate,
			Max = 0,
			MaxModifiers = 0,
			Current = 0,
			CurrentModifiers = 0,
			DisplayOrder = 0,
			AttributeBaseType = ENUMERATORS.Attribute.AttributeBaseTypeEnum.Weapon};
		
		_tempCharAttribute[(int)ENUMERATORS.Attribute.WeaponAttributeTypeEnum.ReloadSpeed] = 
			new AttributeBase(){ 			
			Name = CONSTANTS.ATTRIBUTES.WEAPON_ATTRIBUTE_NAMES[(int)ENUMERATORS.Attribute.WeaponAttributeTypeEnum.ReloadSpeed], 
			AttributeType = (int)ENUMERATORS.Attribute.WeaponAttributeTypeEnum.ReloadSpeed,
			Max = 0,
			MaxModifiers = 0,
			Current = 0,
			CurrentModifiers = 0,
			DisplayOrder = 0,
			AttributeBaseType = ENUMERATORS.Attribute.AttributeBaseTypeEnum.Weapon};

		return _tempCharAttribute;
	}

	// Use this for initialization
	public virtual void  Start () {

		if (this.FireRate.Max  == 0f)
			this.FireRate.Max = 1f;

		if (this.ReloadSpeed.Max == 0f)
			this.ReloadSpeed.Max = 2f;
	}
	
	// Update is called once per frame
	public virtual void Update () {

		if (_renderer != null)
			_renderer.enabled = RenderWeapon;

		AttributeModifier.CleanAttributesModifiers(ref this.Attributes); // Limpa os valores calculados
		AttributeModifier.CheckAttributeModifiers(ref this.AttributeModifiers, this.Effects); // Atualiza a tabela de modificadores de atributos
		AttributeModifier.ApplyAttributesModifiers(ref this.Attributes, ref this.AttributeModifiers, this.Effects); // Calcula os modificadores de atributo 

		// TODO aplicar logica em propriedades (EVENT DRIVEN)
		/* Multiplicador da Animacao = Animacoes Desejadas Por Segundo * Quantidade de Frames da Animacao / Quantidade de FPS de Playback (30FPS default Unity) 
		   SE o multiplicador da animacao for menor que 1, marcar como 1 pois o CoolDown resolve e a animacao fica correta
			Cooldown = 1000(1 segundo) Dividido pelas animacoes desejadas por segundo
		*/
		ShootAnimMultiplier = Mathf.Round((this.FireRate.MaxWithModifiers * ShootAnimFrames / 30f) * 100f) / 100f; 
		if (ShootAnimMultiplier < 1) 
			ShootAnimMultiplier = 1;
		
		CoolDown = (1000f / this.FireRate.MaxWithModifiers) / 1000f;

		/* Multiplicador da Animacao de Recarga = Quantidade de Frames da Animacao / 30FPS Default Unity / Tempo que a recarga deve durar em segundos
		 * Cooldown de Recarga = Tempo que a recarga deve durar em segundos * 1000
		 */
		ReloadAnimMultiplier = Mathf.Round((ReloadAnimFrames / 30f / this.ReloadSpeed.MaxWithModifiers) * 100f) / 100f;
		ReloadCoolDown = this.ReloadSpeed.MaxWithModifiers;

		IsReloading = _nextShootAfterReloadCoolDown > Time.time;
	}

	public virtual void TriggerPressed()
	{
		if (Automatic)
			Shoot();
	}

	public virtual void TriggerWasPressed()
	{
		if (!Automatic)
			Shoot();
	}

	protected virtual void Shoot()
	{
		if (_nextShootCoolDown < Time.time && !IsReloading)
		{
			if (Automatic && IsShooting) return; // Se a arma for automatica somente atira se ela nao estiver atirando

			if (Ammo.CurrentWithModifiers > 0)
			{
				// Show  Muzzle - TODO

				// Shoot  Projectile
				Projectile _myProjectile = (Projectile)ApplicationModel.Instance.ProjectileTable[ProjectileID].Pool.GetFromPool();
				_myProjectile.Damager = this.WeaponOwner;
				_myProjectile.transform.position = transform.position;
                
                if (WeaponOwner is Player)
				    _myProjectile.transform.LookAt((WeaponOwner as Player).LaserEnd);
                else
                    _myProjectile.transform.localRotation = WeaponOwner.transform.rotation;

                //_myProjectile.transform.rotation =  new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);
                //_myProjectile.Damage = this.Attributes[(int)ENUMERATORS.Attribute.WeaponAttributeTypeEnum.Damage].MaxWithModifiers;

                // Play  Sound
                PlayShootAudio();

				IsShooting = true;

                Ammo.Current--;
				_nextShootCoolDown = Time.time + CoolDown;
			}
		}
	}

	public virtual void Reload()
	{
		if (!IsReloading && Ammo.MaxWithModifiers > 0)
		{
			if (Ammo.CurrentModifiers == this.ClipSize)
				return;

			int reloadAmmount = 0;

			if (Ammo.MaxWithModifiers < this.ClipSize)
				reloadAmmount = (int)Ammo.MaxWithModifiers;
			else
				reloadAmmount = ClipSize;

			if ((reloadAmmount + Ammo.Current) > ClipSize)
				reloadAmmount = (int)(ClipSize - Ammo.Current);

            // Somente diminui o maximo de munição se não for infinito
            if(!InfiniteAmmo)
                Ammo.Max -= reloadAmmount;

            Ammo.Current += reloadAmmount;

			IsReloading = true;

			PlayReloadAudio();

			_nextShootAfterReloadCoolDown = Time.time + ReloadCoolDown;
		}
	}

	public virtual void PlayReloadAudio()
	{
		if (ShootAudio != null && ReloadAudio.Length > 0)
			ApplicationModel.Instance.myAudioManager.PlayClip(ReloadAudio[Random.Range(0, ReloadAudio.Length)]);				
	}

	public virtual void PlayShootAudio()
	{
		if (ShootAudio != null && ShootAudio.Length > 0)
			ApplicationModel.Instance.myAudioManager.PlayClip(ShootAudio[Random.Range(0, ShootAudio.Length)]);				
	}

	public AttributeBase Damage
	{
		get
		{
			return this.Attributes[(int)ENUMERATORS.Attribute.WeaponAttributeTypeEnum.Damage];
		}
	}

	public AttributeBase Ammo
	{
		get
		{
			return this.Attributes[(int)ENUMERATORS.Attribute.WeaponAttributeTypeEnum.Ammo];
		}
	}

	public AttributeBase FireRate
	{
		get
		{
			return this.Attributes[(int)ENUMERATORS.Attribute.WeaponAttributeTypeEnum.FireRate];
		}
	}

	public AttributeBase ReloadSpeed
	{
		get
		{
			return this.Attributes[(int)ENUMERATORS.Attribute.WeaponAttributeTypeEnum.ReloadSpeed];
		}
	}

	public AttributeBase CriticChance
	{
		get
		{
			return this.Attributes[(int)ENUMERATORS.Attribute.WeaponAttributeTypeEnum.ReloadSpeed];
		}
	}

	public AttributeBase CriticMultiplier
	{
		get
		{
			return this.Attributes[(int)ENUMERATORS.Attribute.WeaponAttributeTypeEnum.ReloadSpeed];
		}
	}

    public bool IsEmpty
    {
        get
        {
            return Ammo.Current <= 0;
        }
    }

}
