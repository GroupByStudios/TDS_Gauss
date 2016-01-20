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
	public int Ammo;
	public int AmmoMax;
	public float Damage;
	public float DamageCurrent;
	public int ShootAnimFrames;
	[HideInInspector] public float ShootAnimMultiplier;
	[Range(0.1f, 20f)]
	public float FireRatePerSecond;
	public int ReloadAnimFrames;
	[HideInInspector] public float ReloadAnimMultiplier;
	[Range(0.1f, 20f)]
	public float ReloadTimeInSecond;
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
	}

	// Use this for initialization
	public virtual void  Start () {

		if (FireRatePerSecond == 0f) FireRatePerSecond = 1f;
		if (ReloadTimeInSecond == 0f) ReloadTimeInSecond = 2f;
	}
	
	// Update is called once per frame
	public virtual void Update () {
	
		// TODO aplicar logica em propriedades (EVENT DRIVEN)
		/* Multiplicador da Animacao = Animacoes Desejadas Por Segundo * Quantidade de Frames da Animacao / Quantidade de FPS de Playback (30FPS default Unity) 
		   SE o multiplicador da animacao for menor que 1, marcar como 1 pois o CoolDown resolve e a animacao fica correta
			Cooldown = 1000(1 segundo) Dividido pelas animacoes desejadas por segundo
		*/
		ShootAnimMultiplier = Mathf.Round((FireRatePerSecond * ShootAnimFrames / 30f) * 100f) / 100f; 
		if (ShootAnimMultiplier < 1) 
			ShootAnimMultiplier = 1;
		CoolDown = (1000f / FireRatePerSecond) / 1000f;

		/* Multiplicador da Animacao de Recarga = Quantidade de Frames da Animacao / 30FPS Default Unity / Tempo que a recarga deve durar em segundos
		 * Cooldown de Recarga = Tempo que a recarga deve durar em segundos * 1000
		 */
		ReloadAnimMultiplier = Mathf.Round((ReloadAnimFrames / 30f / ReloadTimeInSecond) * 100f) / 100f;
		ReloadCoolDown = ReloadTimeInSecond;

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

			if (Ammo > 0)
			{
				// Show  Muzzle - TODO

				// Shoot  Projectile
				Projectile _myProjectile = (Projectile)ApplicationModel.Instance.ProjectileTable[ProjectileID].Pool.GetFromPool();
				_myProjectile.transform.position = transform.position;
				_myProjectile.transform.rotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);
				_myProjectile.Damage = Damage;

				// Play  Sound
				PlayShootAudio();

				IsShooting = true;

				Ammo -= 1;
				_nextShootCoolDown = Time.time + CoolDown;
			}
		}
	}

	public virtual void Reload()
	{
		if (!IsReloading && Ammo < AmmoMax){
			Ammo = AmmoMax;

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

	void OnGUI()
	{
		if (WeaponOwner != null && WeaponOwner.PlayerInputController != null)
		{
			float TabSize = 15f;
			float LabelSize = 150f;
			float newLineSize = 15f;
			float newColumnSize = LabelSize + TabSize;

			GUIStyle _guiStyle = new GUIStyle();
			_guiStyle.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
			_guiStyle.fontStyle = FontStyle.Bold;

			Vector2 StarPosition = new Vector2(Screen.width * 0.8f, Screen.height * 0.8f);
			Vector2 CurrentPosition = StarPosition;

			GUI.Label(new Rect(CurrentPosition.x, CurrentPosition.y, LabelSize, LabelSize), WeaponName, _guiStyle);
			CurrentPosition.y += newLineSize;
			GUI.Label(new Rect(CurrentPosition.x, CurrentPosition.y, LabelSize, LabelSize), string.Format("Ammo: {0} / {1}", Ammo, AmmoMax), _guiStyle);
		}
	}
}
