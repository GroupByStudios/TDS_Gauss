using UnityEngine;
using System.Linq;
using System.Collections;

[RequireComponent(typeof(PlayerInput))]
public class Player : Character
{

    public ENUMERATORS.Player.PlayerClass PlayerClass;
    [HideInInspector]
    public PlayerInput PlayerInputController;

    public PlayerSkills PlayerSkillSet;

    Camera myCamera;

    bool DebugEnabled;

    public LayerMask MouseRotationLayer;
    Vector3 CameraForward;
    [HideInInspector]
    public Vector3 LookPosition;
    Vector3 MoveToPosition;

    bool OnIronSight;

    public WeaponBase[] SupportedWeapons;
    [HideInInspector]
    public WeaponBase CurrentWeapon;
    public Transform WeaponBone;

    public GranadeBase CurrentGranade;

    // Laser Sight
    public Vector3 LaserOriginOffset;
    public float LaserWidth;
    LineRenderer _laserLineRenderer;
    Vector3 _laserOrigin;
    [HideInInspector]
    public Vector3 LaserEnd;
    GameObject _laserPoint;
    Light _laserPointLight;

    void Awake()
    {
        CurrentGranade = null;
        PlayerInputController = GetComponent<PlayerInput>();
        SetInitialAttributes();
        SetupWeapons();
    }


    // Use this for initialization
    protected override void Start()
    {
        // NAO CODIFICAR NESSA AREA. SOMENTE SE NECESSARIO
        base.Start();
        // Codifique daqui para baixo;

        myCamera = Camera.main;
        base.CharacterType = ENUMERATORS.Character.CharacterTypeEnum.Player;

        // Initialize o controle de skills
        PlayerSkillSet = new PlayerSkills();
        PlayerSkillSet.InitializePlayerSkills(this);

        CurrentGranade = null;

        _laserLineRenderer = GetComponent<LineRenderer>();

        if (_laserLineRenderer != null)
        {
            _laserLineRenderer.enabled = true;
            _laserPoint = new GameObject(string.Concat(this.name, "LaserSpotLight"), typeof(Light));
            _laserPoint.transform.SetParent(this.gameObject.transform);
            _laserPointLight = _laserPoint.GetComponent<Light>();
            _laserPointLight.type = LightType.Spot;
            _laserPointLight.enabled = true;
            _laserPointLight.shadows = LightShadows.None;
            _laserPointLight.color = _laserLineRenderer.material.GetColor("_EmissionColor");
            _laserPointLight.spotAngle = 20f;
            _laserPointLight.intensity = 4.5f;
            _laserPointLight.range = 1f;
            _laserPointLight.bounceIntensity = 0;

        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        // NAO CODIFICAR NESSA AREA. SOMENTE SE NECESSARIO
        base.Update();
        // Codifique daqui para baixo;

        // Se tiver um controle anexado processa o update
        if (PlayerInputController != null)
        {
            AggroDown();
            HandleActions();
            HandleAnimation();
            DrawLaserSight();

            // Habilitar o Modo Debug do Personagem
            if (Input.GetKeyDown(KeyCode.F2))
                DebugEnabled = !DebugEnabled;
        }
    }

    // Fixed Update
    protected override void FixedUpdate()
    {
        // NAO CODIFICAR NESSA AREA. SOMENTE SE NECESSARIO
        base.FixedUpdate();
        // Codifique daqui para baixo;

        if (PlayerInputController != null)
        {
            HandleMovement();
            HandleRotation();
            CalculateLaserSight();
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
        Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.EnergyPoint].Max = 1000f;
        Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.EnergyPoint].Current = 1000f;
        Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Stamina].Max = 800f;
        Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Stamina].Current = 800f;
        Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Speed].Max = 7f;
        Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Armor].Max = 100f;
        Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Aggro].Max = 0f;
    }

    void SetupWeapons()
    {
        if (SupportedWeapons != null)
        {
            for (int i = 0; i < SupportedWeapons.Length; i++)
            {
                SupportedWeapons[i] = Instantiate(SupportedWeapons[i]) as WeaponBase;
                SupportedWeapons[i].gameObject.SetActive(false);
                SupportedWeapons[i].transform.SetParent(this.transform, false);

                if (CurrentWeapon == null)
                    ChangeWeapon(SupportedWeapons[i]);
            }
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

    void CalculateLaserSight()
    {
        // Desenha o Laser
        if (this.CurrentWeapon != null && this._laserLineRenderer != null)
        {
            _laserOrigin = this.CurrentWeapon.transform.position + this.LaserOriginOffset;
            LaserEnd = _laserOrigin + (LookPosition * 500f);
            LaserEnd.y = _laserOrigin.y;

            // TODO CORRIGIR HEAP vs STACK por PERFORMANCE
            RaycastHit[] _hits = new RaycastHit[50];
            Physics.RaycastNonAlloc(_laserOrigin, LaserEnd, _hits);
            _hits = _hits.OrderBy(h => h.distance).ToArray();

            // Tenta atualizar a posicao do Laser com a colisao
            for (int i = 0; i < _hits.Length; i++)
            {
                /* Se nao for o jogador e arma do jogador, atualiza o final do laser */
                if (_hits[i].transform != null &&
                    _hits[i].transform != this.transform &&
                    _hits[i].transform != this.CurrentWeapon.transform)
                {
                    LaserEnd = new Vector3(_hits[i].point.x, _laserOrigin.y, _hits[i].point.z);
                    break;
                }
            }
        }
    }

    void DrawLaserSight()
    {
        if (this.CurrentWeapon != null && this._laserLineRenderer != null)
        {
            this._laserLineRenderer.SetPosition(0, _laserOrigin);
            this._laserLineRenderer.SetPosition(1, LaserEnd);
            this._laserLineRenderer.SetWidth(LaserWidth, LaserWidth);

            if (this._laserPointLight != null)
            {
                Vector3 _direction = (LaserEnd - _laserOrigin);
                this._laserPointLight.transform.position = LaserEnd;
                this._laserPointLight.transform.LookAt(LaserEnd + _direction);

                // Converte de Space global para Local
                Vector3 _localPosition = this._laserPointLight.transform.InverseTransformPoint(this._laserPointLight.transform.position);
                // Diminui o vetor Foward
                _localPosition = new Vector3(_localPosition.x, _localPosition.y, _localPosition.z - 0.5f);

                // Volta de Local para Global
                this._laserPoint.transform.position = this._laserPoint.transform.TransformPoint(_localPosition);
            }
        }
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
            MoveToPosition = PlayerInputController.Move_Y * Vector3.forward + PlayerInputController.Move_X * Vector3.right;
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

        /* Verifica se o Jogador pode se movimentar, levar em consideracao a posicao no ViewPort (Escala de 0,0 a 1,0 nos eixos X e Y) */
        Vector3 _viewPortPosition = Camera.main.WorldToViewportPoint(this.transform.position);
        if ((MoveToPosition.x > 0 && _viewPortPosition.x >= 0.9) ||
            (MoveToPosition.x < 0 && _viewPortPosition.x <= 0.1))
        {
            MoveToPosition.x = 0;
        }

        if ((MoveToPosition.z > 0 && _viewPortPosition.y >= 0.8) || //XZ Plano para movimentacao
            (MoveToPosition.z < 0 && _viewPortPosition.y <= 0.1))   //XZ Plano para movimentacao
        {
            MoveToPosition.z = 0;
        }

        if (MoveToPosition.magnitude > 0)
        {
            MoveToPosition += transform.position;
            transform.LookAt(MoveToPosition);
            _rigidBody.MovePosition(MoveToPosition);
        }

        //LookPosition = transform.position + transform.forward * 10f;

        _rigidBody.velocity = new Vector3(0, _rigidBody.velocity.y, 0); // Zera a velocidade para evitar movimentacoes desnecessarias do personagem
    }

    /// <summary>
    /// Metodo responsavel por gerenciar a rotacao do personagem
    /// </summary>
    void HandleRotation()
    {
        if (PlayerInputController.IsKeyboardAndMouse)
        {
            Ray ray = Camera.main.ScreenPointToRay(PlayerInputController.MouseInput);
            RaycastHit Hit;

            if (Physics.Raycast(ray, out Hit, 100f, MouseRotationLayer))
            {
                //                LookPosition = Hit.point;
                LookPosition = (Hit.point - this.transform.position);
                LookPosition.y = transform.position.y;
            }

            //if (LookPosition.magnitude < 0.5f)
            LookPosition = LookPosition * 100f;

            LookPosition = Vector3.ClampMagnitude(LookPosition, 500f);
            //Debug.DrawRay(this.transform.position, LookPosition);

            transform.LookAt(this.transform.position + LookPosition, Vector3.up);
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
        //IsShooting = false;

        if (PlayerInputController.ThrowGranadeActionIsPressed)
        {

            if (CurrentGranade == null)
            {
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

            // Handle Skills
            if (PlayerInputController.SkillSlot1_WasPressed)
            {
                PlayerSkillSet.PerformSkill(PlayerSkills.DPADController.LEFT);
            }
            else if (PlayerInputController.SkillSlot2_WasPressed)
            {
                PlayerSkillSet.PerformSkill(PlayerSkills.DPADController.RIGHT);
            }
        }

        if (CurrentWeapon != null)
        {
            if (PlayerInputController.ShootActionIsPressed)
                CurrentWeapon.TriggerPressed();

            if (PlayerInputController.ShootActionWasPressed)
                CurrentWeapon.TriggerWasPressed();

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

    /// <summary>
    /// Metodo principal responsavel por calcular o dano que o jogador dara
    /// </summary>
    /// <returns>The damage.</returns>
    /// <param name="enemy_">Enemy.</param>
    public override float CalculateDamage()
    {
        // IMPLEMENTAR CALCULO DE COMBATE
        if (this.CurrentWeapon != null)
            return this.CurrentWeapon.Damage.MaxWithModifiers;
        else
            return 0;
    }

    #endregion

    #region Aggro Methods

    public void AggroUp()
    {
        this.Aggro.Max += 3;

        if (this.Aggro.Max > 100)
            this.Aggro.Max = 100;

    }

    private void AggroDown()
    {
        if (this.Aggro.Max > 0)
        {
            this.Aggro.Max -= (3 * Time.deltaTime);

            if (this.Aggro.Max < 0)
                this.Aggro.Max = 0;
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
        foreach (AttributeBase _charAttribute in Attributes)
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
        foreach (AttributeModifier _attrModifier in AttributeModifiers)
        {
            if (_attrModifier != null)
            {
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
    }
}