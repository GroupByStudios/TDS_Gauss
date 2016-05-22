using UnityEngine;
using System.Collections;

public class WeaponBase : MonoBehaviour {

	[HideInInspector] public Player WeaponOwner;

	public Vector3 DefaultPosition;
	public Vector3 DefaultRotation;

	public int WeaponID;
	public int WeaponTypeID;
	public string WeaponName;
	public bool Automatic;
	public int ProjectileID;

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

	protected float _nextShootCoolDown;
	protected float _nextShootAfterReloadCoolDown;

	public virtual void Awake()
	{
		//DefaultPosition = transform.position;
		//DefaultRotation = transform.rotation;	
		Effects = new Effects();
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

		if (this.Attributes[(int)ENUMERATORS.Attribute.WeaponAttributeTypeEnum.FireRate].Max  == 0f)
			this.Attributes[(int)ENUMERATORS.Attribute.WeaponAttributeTypeEnum.FireRate].Max = 1f;

		if (this.Attributes[(int)ENUMERATORS.Attribute.WeaponAttributeTypeEnum.ReloadSpeed].Max == 0f)
			this.Attributes[(int)ENUMERATORS.Attribute.WeaponAttributeTypeEnum.ReloadSpeed].Max = 2f;
	}
	
	// Update is called once per frame
	public virtual void Update () {
		AttributeModifier.CleanAttributesModifiers(ref this.Attributes); // Limpa os valores calculados
		AttributeModifier.CheckAttributeModifiers(ref this.AttributeModifiers, this.Effects); // Atualiza a tabela de modificadores de atributos
		AttributeModifier.ApplyAttributesModifiers(ref this.Attributes, ref this.AttributeModifiers, this.Effects); // Calcula os modificadores de atributo 

		// TODO aplicar logica em propriedades (EVENT DRIVEN)
		/* Multiplicador da Animacao = Animacoes Desejadas Por Segundo * Quantidade de Frames da Animacao / Quantidade de FPS de Playback (30FPS default Unity) 
		   SE o multiplicador da animacao for menor que 1, marcar como 1 pois o CoolDown resolve e a animacao fica correta
			Cooldown = 1000(1 segundo) Dividido pelas animacoes desejadas por segundo
		*/
		ShootAnimMultiplier = Mathf.Round((this.Attributes[(int)ENUMERATORS.Attribute.WeaponAttributeTypeEnum.FireRate].MaxWithModifiers * ShootAnimFrames / 30f) * 100f) / 100f; 
		if (ShootAnimMultiplier < 1) 
			ShootAnimMultiplier = 1;
		
		CoolDown = (1000f / this.Attributes[(int)ENUMERATORS.Attribute.WeaponAttributeTypeEnum.FireRate].MaxWithModifiers) / 1000f;

		/* Multiplicador da Animacao de Recarga = Quantidade de Frames da Animacao / 30FPS Default Unity / Tempo que a recarga deve durar em segundos
		 * Cooldown de Recarga = Tempo que a recarga deve durar em segundos * 1000
		 */
		ReloadAnimMultiplier = Mathf.Round((ReloadAnimFrames / 30f / this.Attributes[(int)ENUMERATORS.Attribute.WeaponAttributeTypeEnum.ReloadSpeed].MaxWithModifiers) * 100f) / 100f;
		ReloadCoolDown = this.Attributes[(int)ENUMERATORS.Attribute.WeaponAttributeTypeEnum.ReloadSpeed].MaxWithModifiers;

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

			if (this.Attributes[(int)ENUMERATORS.Attribute.WeaponAttributeTypeEnum.Ammo].CurrentWithModifiers > 0)
			{
				// Show  Muzzle - TODO

				// Shoot  Projectile
				Projectile _myProjectile = (Projectile)ApplicationModel.Instance.ProjectileTable[ProjectileID].Pool.GetFromPool();
				_myProjectile.Damager = this.WeaponOwner;
				_myProjectile.transform.position = transform.position;
				_myProjectile.transform.LookAt(WeaponOwner.LookPosition);
				//_myProjectile.transform.rotation =  new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);
				//_myProjectile.Damage = this.Attributes[(int)ENUMERATORS.Attribute.WeaponAttributeTypeEnum.Damage].MaxWithModifiers;

				// Play  Sound
				PlayShootAudio();

				IsShooting = true;

				this.Attributes[(int)ENUMERATORS.Attribute.WeaponAttributeTypeEnum.Ammo].Current--;
				_nextShootCoolDown = Time.time + CoolDown;
			}
		}
	}

	public virtual void Reload()
	{
		if (!IsReloading && this.Attributes[(int)ENUMERATORS.Attribute.WeaponAttributeTypeEnum.Ammo].MaxWithModifiers > 0)
		{
			AttributeBase _attributeAmmo = this.Attributes[(int)ENUMERATORS.Attribute.WeaponAttributeTypeEnum.Ammo];

			if (_attributeAmmo.CurrentModifiers == this.ClipSize)
				return;

			int reloadAmmount = 0;

			if (_attributeAmmo.MaxWithModifiers < this.ClipSize)
				reloadAmmount = (int)_attributeAmmo.MaxWithModifiers;
			else
				reloadAmmount = ClipSize;

			if ((reloadAmmount + _attributeAmmo.Current) > ClipSize)
				reloadAmmount = (int)(ClipSize - _attributeAmmo.Current);

			_attributeAmmo.Max -= reloadAmmount;
			_attributeAmmo.Current += reloadAmmount;

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

}
