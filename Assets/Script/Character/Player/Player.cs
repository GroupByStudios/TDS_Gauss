using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerInput))]
public class Player : Character {

	public int PlayerClassID;
	[HideInInspector] public PlayerInput PlayerInputController;

	Camera myCamera;

	bool DebugEnabled;

	public LayerMask MouseRotationLayer;
	Vector3 CameraForward;
	Vector3 LookPosition;
	Vector3 MoveToPosition;

	bool OnIronSight;
	bool IsShooting;

	public WeaponBase[] SupportedWeapons;
	[HideInInspector] public WeaponBase CurrentWeapon;
	public Transform WeaponBone;

	public GranadeBase CurrentGranade;

	void Awake()
	{
		PlayerInputController = GetComponent<PlayerInput>();
		SetInitialAttributes();
		SetupWeapons();
	}


	// Use this for initialization
	protected override void Start () {
		// NAO CODIFICAR NESSA AREA. SOMENTE SE NECESSARIO
		base.Start();
		// Codifique daqui para baixo;

		myCamera = Camera.main;


		base.CharacterType = ENUMERATORS.Character.CharacterTypeEnum.Player;


	}
	
	// Update is called once per frame
	protected override void Update () {
		// NAO CODIFICAR NESSA AREA. SOMENTE SE NECESSARIO
		base.Update();
		// Codifique daqui para baixo;

		// Se tiver um controle anexado processa o update
		if (PlayerInputController != null)
		{
			HandleActions();
			HandleAnimation();
		
			// Habilitar o Modo Debug do Personagem
			if (Input.GetKeyDown(KeyCode.F2)) 
				DebugEnabled = !DebugEnabled;
		}
	}

	// Fixed Update
	protected override void FixedUpdate(){
		// NAO CODIFICAR NESSA AREA. SOMENTE SE NECESSARIO
		base.FixedUpdate();
		// Codifique daqui para baixo;

		if (PlayerInputController != null)
		{
			HandleMovement();
			HandleRotation();
		}
	}

	// Late Update
	protected override void LateUpdate()
	{
		// Codifique daqui para baixo;


		base.LateUpdate();
		// NAO CODIFICAR NESSA AREA. SOMENTE SE NECESSARIO
	}

	/// <summary>
	/// Metodo responsavel por atribuir os atributos iniciais
	/// </summary>
	void SetInitialAttributes()
	{
		Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.HitPoint].Max = 1000f;
		Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.HitPoint].Current = 1000f;
		Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Stamina].Max = 800f;
		Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Stamina].Current = 800f;
		Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Speed].Max = 7f;
		Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Armor].Max = 100f;
		Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Damage].Max = 50f;
		Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.ShootSpeed].Max = 1f;
		Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.CriticChance].Max = 0.15f;
		Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.CriticMultiplier].Max = 1.5f;
		Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Aggro].Max = 0f;
	}

	void SetupWeapons()
	{
		for(int i = 0;  i < SupportedWeapons.Length; i++)
		{
			SupportedWeapons[i] = Instantiate(SupportedWeapons[i]) as WeaponBase;
			SupportedWeapons[i].gameObject.SetActive(false);
			SupportedWeapons[i].transform.SetParent(this.transform, false);

			if (CurrentWeapon == null)
				ChangeWeapon(SupportedWeapons[i]);
		}
	}

	#region Private Methods

	/// <summary>
	/// Set a arma 
	/// </summary>
	void ChangeWeapon(WeaponBase setupWeapon_)
	{
		if (WeaponBone != null && setupWeapon_ != null && CurrentWeapon != setupWeapon_)
		{
			if (CurrentWeapon != null)
			{
				CurrentWeapon.transform.parent = null;
				CurrentWeapon.gameObject.SetActive(false);
				CurrentWeapon.transform.parent = this.transform;
			}

			CurrentWeapon = setupWeapon_;
			CurrentWeapon.gameObject.SetActive(true);
			CurrentWeapon.WeaponOwner = this;
			CurrentWeapon.transform.SetParent(WeaponBone, false);
		}
	}

	void OnEnable()
	{
		if (CurrentWeapon != null)
			ChangeWeapon(CurrentWeapon);
	}

	/// <summary>
	/// Metodo responsavel por gerenciar a movimentacao do personagem
	/// </summary>
	void HandleMovement()
	{
		if (myCamera != null)
		{
			CameraForward = Vector3.Scale(myCamera.transform.up, new Vector3(1, 0, 1)).normalized;
			MoveToPosition = PlayerInputController.Move_Y * CameraForward + PlayerInputController.Move_X * myCamera.transform.right;
		}
		else
		{
			MoveToPosition =  PlayerInputController.Move_Y * Vector3.forward + PlayerInputController.Move_X * Vector3.right;
		}

		if (MoveToPosition.magnitude > 1)
			MoveToPosition.Normalize();

		PlayerInputController.ConvertMoveInput(MoveToPosition);

		// Se estiver mirando diminui a velocidade em %
		if (OnIronSight) MoveToPosition *= 0.7f;

		//Altera a velocidade de movimentação do personagem
		MoveToPosition *= Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Speed].MaxWithModifiers;
		Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Speed].Current = MoveToPosition.magnitude;

		// Aplica as movimentacoes do frame
		MoveToPosition *= Time.fixedDeltaTime;

		if (MoveToPosition.magnitude > 0){
			
			MoveToPosition += transform.position;
			transform.LookAt(MoveToPosition);
			_rigidBody.MovePosition(MoveToPosition);
		}

		_rigidBody.velocity = new Vector3(0, _rigidBody.velocity.y, 0); // Zera a velocidade para evitar movimentacoes desnecessarias do personagem
	}

	/// <summary>
	/// Metodo responsavel por gerenciar a rotacao do personagem
	/// </summary>
	void HandleRotation()
	{
		if (PlayerInputController.InputDeviceJoystick == null)
		{
			Ray ray = Camera.main.ScreenPointToRay(PlayerInputController.MouseInput);
			RaycastHit Hit;

			ray.origin = new Vector3(ray.origin.x, 10f, ray.origin.z);
			if (Physics.Raycast(ray, out Hit, 100f, MouseRotationLayer))
			{
				LookPosition = Hit.point;
			}

			LookPosition.y = transform.position.y;

			transform.LookAt(LookPosition, Vector3.up);
		}
		else
		{
			if (PlayerInputController.MouseInput.magnitude != 0)
			{
				LookPosition = transform.position + (new Vector3(PlayerInputController.MouseInput.x, 0, PlayerInputController.MouseInput.y) * 100f);
				LookPosition.y = transform.position.y;
				transform.LookAt(LookPosition, Vector3.up);
			}
		}

		
	}

	/// <summary>
	/// Metodo responsavel por gerenciar as acoes do personagem
	/// </summary>
	void HandleActions()
	{
		OnIronSight = false;
		IsShooting = false;

		if (PlayerInputController.ThrowGranadeActionIsPressed){
		
			if (CurrentGranade == null){
				CurrentGranade = ApplicationModel.Instance.GranadeTable[0].Pool.GetFromPool() as GranadeBase;
			}

			if (CurrentGranade != null)
			{
				CurrentGranade.CookGranade();
			}
		}
		else
		{
			if (PlayerInputController.ThrowGranadeActionWasRelease && CurrentGranade != null)
			{
				CurrentGranade.transform.position = transform.position + Vector3.up * 2f;				
				CurrentGranade.ThrowGranade(transform.forward);
				CurrentGranade = null;
			}

			OnIronSight = PlayerInputController.AimActionIsPressed;
			IsShooting = PlayerInputController.ShootActionIsPressed;
		}

		if (CurrentWeapon != null)
		{
			if (IsShooting)
				CurrentWeapon.Shoot();

			if (PlayerInputController.ReloadActionWasPressed)
			{
				CurrentWeapon.Reload();
			}

			// Se estiver carregando nao pode olhar na mira
			OnIronSight = OnIronSight && !CurrentWeapon.IsReloading;
		}
	}

	/// <summary>
	/// Metodo responsavel por gerenciar as animacoes
	/// </summary>
	void HandleAnimation()
	{
		if (_animator != null)
		{
			_animator.SetFloat(CONSTANTS.ANIMATION.FORWARD, PlayerInputController.Move_Y_Converted, 0.1f, Time.deltaTime);
			_animator.SetFloat(CONSTANTS.ANIMATION.SIDEWAYS, PlayerInputController.Move_X_Converted, 0.1f, Time.deltaTime);
			_animator.SetBool(CONSTANTS.ANIMATION.ISIRONSIGHT, OnIronSight);


			if (CurrentWeapon != null)
			{
				_animator.SetBool(CONSTANTS.ANIMATION.ISRELOADING, CurrentWeapon.IsReloading);

				if (CurrentWeapon.IsShooting) 
				{
					_animator.SetTrigger(CONSTANTS.ANIMATION.ISSHOOTING);
					CurrentWeapon.IsShooting = false; // TODO LIMPAR A VARIAVEL SE ESTA ATIRANDO EM LUGAR MAIS APROPRIADO
				}

				_animator.SetInteger(CONSTANTS.ANIMATION.WEAPONTYPE, CurrentWeapon.WeaponTypeID);
				_animator.SetFloat(CONSTANTS.ANIMATION.SHOOTINGMULTIPLIER, CurrentWeapon.ShootAnimMultiplier);
				_animator.SetFloat(CONSTANTS.ANIMATION.RELOADINGMULTIPLIER, CurrentWeapon.ReloadAnimMultiplier);
			}
		}
	}

	#endregion

	void OnGUI()
	{
		if (DebugEnabled)
			DrawDebugMode();		
	}

	void DrawDebugMode()
	{
		float TabSize = 15f;
		float LabelSize = 150f;
		float newLineSize = 15f;
		float newColumnSize = LabelSize + TabSize;

		GUIStyle _guiStyle = new GUIStyle();
		_guiStyle.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
		_guiStyle.fontStyle = FontStyle.Bold;

		Vector2 StarPosition = new Vector2(0, 0);
		Vector2 CurrentPosition = StarPosition;

		GUI.Label(new Rect(CurrentPosition.x, CurrentPosition.y, LabelSize, LabelSize), "Player DEBUG MODE", _guiStyle);
		CurrentPosition.y += newLineSize * 2;

		// Atualiza a posicao inicial para criar a tabela
		StarPosition = CurrentPosition;

		GUI.Label(new Rect(CurrentPosition.x, CurrentPosition.y, LabelSize, LabelSize), "Attributes", _guiStyle);
		CurrentPosition.y += newLineSize;

		float attributeLineSize = 0.65f;
		foreach(CharacterAttribute _charAttribute in Attributes)
		{
			_guiStyle.fontSize = 12;
			GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), _charAttribute.AttributeType.ToString(), _guiStyle);
			CurrentPosition.y += newLineSize * attributeLineSize;			
			GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("Name: {0}", _charAttribute.Name), _guiStyle);
			CurrentPosition.y += newLineSize * attributeLineSize;	
			GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("Current: {0}", _charAttribute.Current), _guiStyle);
			CurrentPosition.y += newLineSize * attributeLineSize;	
			GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("Current Modifier: {0}", _charAttribute.CurrentModifiers), _guiStyle);
			CurrentPosition.y += newLineSize * attributeLineSize;	
			GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("Current W Modifier: {0}", _charAttribute.CurrentWithModifiers), _guiStyle);
			CurrentPosition.y += newLineSize * attributeLineSize;	
			GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("Max: {0}", _charAttribute.Max), _guiStyle);
			CurrentPosition.y += newLineSize * attributeLineSize;	
			GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("Max Modifiers: {0}", _charAttribute.MaxModifiers), _guiStyle);
			CurrentPosition.y += newLineSize * attributeLineSize;	
			GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("Max W Modifiers: {0}", _charAttribute.MaxWithModifiers), _guiStyle);
			CurrentPosition.y += newLineSize;	
		}

		StarPosition.x += newColumnSize;
		CurrentPosition = StarPosition;

		GUI.Label(new Rect(CurrentPosition.x, CurrentPosition.y, LabelSize, LabelSize), "Attributes Modifiers", _guiStyle);
		CurrentPosition.y += newLineSize;
		attributeLineSize = 0.65f;
		foreach(AttributeModifier _attrModifier in AttributeModifiers)
		{
			if (_attrModifier != null){
				_guiStyle.fontSize = 12;
				GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("OriginID: {0}", _attrModifier.OriginID), _guiStyle);
				CurrentPosition.y += newLineSize * attributeLineSize;
				GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("ModifierType: {0}", _attrModifier.ModifierType), _guiStyle);
				CurrentPosition.y += newLineSize * attributeLineSize;
				GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("AttributeType: {0}", _attrModifier.AttributeType), _guiStyle);
				CurrentPosition.y += newLineSize * attributeLineSize;
				GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("CalcType: {0}", _attrModifier.CalcType), _guiStyle);
				CurrentPosition.y += newLineSize * attributeLineSize;
				GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("ApplyTo: {0}", _attrModifier.ApplyTo), _guiStyle);
				CurrentPosition.y += newLineSize * attributeLineSize;
				GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("Value: {0}", _attrModifier.Value), _guiStyle);
				CurrentPosition.y += newLineSize * attributeLineSize;
				GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("TimeInSeconds: {0}", _attrModifier.TimeInSeconds), _guiStyle);
				CurrentPosition.y += newLineSize * attributeLineSize;
				GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("InitialTime: {0}", _attrModifier.InitialTime), _guiStyle);
				CurrentPosition.y += newLineSize * attributeLineSize;
				GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("ExpireTime: {0}", _attrModifier.ExpireTime), _guiStyle);
				CurrentPosition.y += newLineSize * attributeLineSize;
				GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("Consumed: {0}", _attrModifier.Consumed), _guiStyle);
				CurrentPosition.y += newLineSize * attributeLineSize;
			}
		}

		StarPosition.x += newColumnSize;
		CurrentPosition = StarPosition;

		GUI.Label(new Rect(CurrentPosition.x, CurrentPosition.y, LabelSize, LabelSize), "Character Spell", _guiStyle);
		CurrentPosition.y += newLineSize;
		attributeLineSize = 0.65f;
		for(int i = 0; i < CharacterSpellTable.Length; i++)			
		{
			CharacterSpell _charSpell = CharacterSpellTable[i];

			if(_charSpell != null)
			{
				_guiStyle.fontSize = 12;
				GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("Spell Next Cast: {0}", _charSpell.CoolDownTime), _guiStyle);
				CurrentPosition.y += newLineSize * attributeLineSize;

				if (_charSpell.Spell != null){
					GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("Spell ID: {0}", _charSpell.Spell.ID), _guiStyle);
					CurrentPosition.y += newLineSize * attributeLineSize;
					GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("NeedTarget: {0}", _charSpell.Spell.NeedTarget), _guiStyle);
					CurrentPosition.y += newLineSize * attributeLineSize;
					GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("AttributeModifiers: NOT IMPLEMENTED"), _guiStyle);
					CurrentPosition.y += newLineSize * attributeLineSize;
					GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("Damage: {0}", _charSpell.Spell.Damage), _guiStyle);
					CurrentPosition.y += newLineSize * attributeLineSize;
					GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("CoolDown: {0}", _charSpell.Spell.CoolDown), _guiStyle);
					CurrentPosition.y += newLineSize * attributeLineSize;
					GUI.Label(new Rect(CurrentPosition.x + TabSize, CurrentPosition.y, LabelSize, LabelSize), string.Format("ManaCost: {0}", _charSpell.Spell.ManaCost), _guiStyle);
					CurrentPosition.y += newLineSize * attributeLineSize;
				}
			}

		}
	}
}