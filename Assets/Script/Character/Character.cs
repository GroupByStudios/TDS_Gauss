using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Classe base de todos os personagens do. Qualquer evento comum entre os personagens deve ser implementado aqui
/// </summary>
//[RequireComponent(typeof(Rigidbody))]
public class Character : MonoBehaviour
{

    // Delegates
	bool disable=false;
	private PlayerInput script;
	private Character scriptCharacter;
	public AudioClip morte;
	public bool audioisplaying = true;
	AudioSource sound;
	public AudioClip[] gemidos;

    // Tipo do personagem
    public ENUMERATORS.Character.CharacterTypeEnum CharacterType;

    // Atributos de GameDesign
    public AttributeBase[] Attributes = InitializeAttributes();
    [HideInInspector]
    //System.NonSerialized]
    public AttributeModifier[] AttributeModifiers;
    public Effects CharacterEffects;

    #region Componentes da Unity

    protected Rigidbody _rigidBody;
    protected Animator _animator;

    #endregion

    #region Unity Methods 

    // Use this for initialization
    protected virtual void Start()
    {
		script = GetComponent<PlayerInput> ();
		scriptCharacter = GetComponent<Character> ();
		sound = GetComponent<AudioSource> ();
        // Obtem as instancias dos componentes Unity;
        _rigidBody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        CharacterEffects = new Effects();

        this.AttributeModifiers = new AttributeModifier[CONSTANTS.ATTRIBUTES.ATTRIBUTE_MODIFIERS_COUNT];
        for (int i = 0; i < this.AttributeModifiers.Length; i++)
        {
            this.AttributeModifiers[i] = null;
        }

    }

    protected virtual void OnEnabled()
    {
        // Registra os eventos
        //OnHitPointChanged += Character_OnHitPointChanged;
    }

    protected virtual void OnDisabled()
    {
        // Remove os eventos
        //OnHitPointChanged -= Character_OnHitPointChanged;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        AttributeModifier.CleanAttributesModifiers(ref this.Attributes);
        AttributeModifier.CheckAttributeModifiers(ref this.AttributeModifiers, this.CharacterEffects);
        AttributeModifier.ApplyAttributesModifiers(ref this.Attributes, ref this.AttributeModifiers, this.CharacterEffects);
    }

    protected virtual void FixedUpdate()
    {

    }

    // Last method called once per frame
    protected virtual void LateUpdate()
    {
    }

    #endregion

    #region Private Methods

    private static AttributeModifier[] InitializeAttributesModifiers()
    {
        AttributeModifier[] _tempModifier = new AttributeModifier[CONSTANTS.ATTRIBUTES.ATTRIBUTE_MODIFIERS_COUNT];
        for (int i = 0; i < _tempModifier.Length; i++)
        {
            _tempModifier[i] = null;
        }

        return _tempModifier;
    }

    /// <summary>
    /// Metodo responsavel por inicializar a arvore de atributos
    /// </summary>
    private static AttributeBase[] InitializeAttributes()
    {
        AttributeBase[] _tempCharAttribute = new AttributeBase[CONSTANTS.ATTRIBUTES.CHARACTER_ATTRIBUTE_COUNT];

        _tempCharAttribute[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Aggro] =
            new AttributeBase()
            {
                Name = CONSTANTS.ATTRIBUTES.CHARACTER_TYPE_NAMES[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Aggro],
                AttributeType = (int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Aggro,
                Max = 0,
                MaxModifiers = 0,
                Current = 0,
                CurrentModifiers = 0,
                DisplayOrder = 0,
                AttributeBaseType = ENUMERATORS.Attribute.AttributeBaseTypeEnum.Character
            };

        _tempCharAttribute[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Armor] =
            new AttributeBase()
            {
                Name = CONSTANTS.ATTRIBUTES.CHARACTER_TYPE_NAMES[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Armor],
                AttributeType = (int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Armor,
                Max = 0,
                MaxModifiers = 0,
                Current = 0,
                CurrentModifiers = 0,
                DisplayOrder = 0,
                AttributeBaseType = ENUMERATORS.Attribute.AttributeBaseTypeEnum.Character
            };

        _tempCharAttribute[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.HitPoint] =
            new AttributeBase()
            {
                Name = CONSTANTS.ATTRIBUTES.CHARACTER_TYPE_NAMES[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.HitPoint],
                AttributeType = (int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.HitPoint,
                Max = 0,
                MaxModifiers = 0,
                Current = 0,
                CurrentModifiers = 0,
                DisplayOrder = 0,
                AttributeBaseType = ENUMERATORS.Attribute.AttributeBaseTypeEnum.Character
            };

        _tempCharAttribute[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.EnergyPoint] =
            new AttributeBase()
            {
                Name = CONSTANTS.ATTRIBUTES.CHARACTER_TYPE_NAMES[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.EnergyPoint],
                AttributeType = (int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.EnergyPoint,
                Max = 0,
                MaxModifiers = 0,
                Current = 0,
                CurrentModifiers = 0,
                DisplayOrder = 0,
                AttributeBaseType = ENUMERATORS.Attribute.AttributeBaseTypeEnum.Character
            };

        _tempCharAttribute[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Speed] =
            new AttributeBase()
            {
                Name = CONSTANTS.ATTRIBUTES.CHARACTER_TYPE_NAMES[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Speed],
                AttributeType = (int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Speed,
                Max = 0,
                MaxModifiers = 0,
                Current = 0,
                CurrentModifiers = 0,
                DisplayOrder = 0,
                AttributeBaseType = ENUMERATORS.Attribute.AttributeBaseTypeEnum.Character
            };

        _tempCharAttribute[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Stamina] =
            new AttributeBase()
            {
                Name = CONSTANTS.ATTRIBUTES.CHARACTER_TYPE_NAMES[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Stamina],
                AttributeType = (int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Stamina,
                Max = 0,
                MaxModifiers = 0,
                Current = 0,
                CurrentModifiers = 0,
                DisplayOrder = 0,
                AttributeBaseType = ENUMERATORS.Attribute.AttributeBaseTypeEnum.Character
            };

        return _tempCharAttribute;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Metodo responsavel por gerenciar a morte do personagem, deve ser sobrescrito nas classes que herdam de Character
    /// </summary>
    public virtual void Die()
    {
        //Destroy(this.gameObject);
    }

    /// <summary>
    /// Metodo responsavel por calcular o dano recebido do personagem
    /// </summary>
    /// <param name="damager_">Oponente que deferiu o dano</param>
    /// <param name="damageType_">Tipo do Dano</param>
	public virtual void ApplyDamage(Character damager_, ENUMERATORS.Combat.DamageType damageType_, ENUMERATORS.Player.PlayerClass classe)
    {
		ApplyDamage(damager_, damageType_, classe, damager_.CalculateDamage());
    }

	public virtual void ApplyDamage(Character damager_, ENUMERATORS.Combat.DamageType damageType_, ENUMERATORS.Player.PlayerClass classe, float damage_)
    {
        float damage = damage_ - this.Armor.MaxWithModifiers;

        if (damage < 0)
            damage = 1;

        this.HitPoint.Current -= damage;
		if (classe == ENUMERATORS.Player.PlayerClass.SPECIALIST && audioisplaying && !disable) {
			if (this.HitPoint.CurrentWithModifiers <= 0) {
				sound.clip = gemidos [6];
				sound.Play (); 
				script.enabled = !script.enabled;
				scriptCharacter.enabled = !scriptCharacter.enabled;
				CharacterType = ENUMERATORS.Character.CharacterTypeEnum.Enemy; 
				disable = true;
			} 
			else {
				sound.clip = gemidos [0];
				sound.Play (); 
				audioisplaying = false;
				StartCoroutine ("EsperarSomTerminar");
			}
		}
		else if (classe == ENUMERATORS.Player.PlayerClass.ENGINEER && audioisplaying && !disable) {
			if (this.HitPoint.CurrentWithModifiers <= 0) {
				sound.clip = gemidos [6];
				sound.Play (); 
				script.enabled = !script.enabled;
				scriptCharacter.enabled = !scriptCharacter.enabled;
				CharacterType = ENUMERATORS.Character.CharacterTypeEnum.Enemy; 
				disable = true;
			} else {
				sound.clip = gemidos [1];
				sound.Play (); 
				audioisplaying = false;
				StartCoroutine ("EsperarSomTerminar");
			}
		}
		else if (classe == ENUMERATORS.Player.PlayerClass.DEFENDER && audioisplaying && !disable) {
			if (this.HitPoint.CurrentWithModifiers <= 0) {
				sound.clip = gemidos [6];
				sound.Play (); 
				script.enabled = !script.enabled;
				scriptCharacter.enabled = !scriptCharacter.enabled;
				CharacterType = ENUMERATORS.Character.CharacterTypeEnum.Enemy; 
				disable = true;
			} else {	
				sound.clip = gemidos [2];
				sound.Play (); 
				audioisplaying = false;
				StartCoroutine ("EsperarSomTerminar");
			}
		}
		else if (classe == ENUMERATORS.Player.PlayerClass.MEDIC && audioisplaying && !disable) {
			if (this.HitPoint.CurrentWithModifiers <= 0) {
				sound.clip = gemidos [6];
				sound.Play (); 
				script.enabled = !script.enabled;
				scriptCharacter.enabled = !scriptCharacter.enabled;
				CharacterType = ENUMERATORS.Character.CharacterTypeEnum.Enemy; 
				disable = true;
			} else {
				sound.clip = gemidos [3];
				sound.Play (); 
				audioisplaying = false;
				StartCoroutine ("EsperarSomTerminar");
			}
		}
		else if (classe == ENUMERATORS.Player.PlayerClass.ASSAULT && audioisplaying && !disable) {
			if (this.HitPoint.CurrentWithModifiers <= 0) {
				sound.clip = gemidos [6];
				sound.Play (); 
				script.enabled = !script.enabled;
				scriptCharacter.enabled = !scriptCharacter.enabled;
				CharacterType = ENUMERATORS.Character.CharacterTypeEnum.Enemy; 
				disable = true;
			} else {
				sound.clip = gemidos [4];
				sound.Play (); 
				audioisplaying = false;
				StartCoroutine ("EsperarSomTerminar");
			}
		}
		else if (classe == ENUMERATORS.Player.PlayerClass.ROBOT && audioisplaying) {
			sound.clip = gemidos[5];
			sound.Play (); 
			audioisplaying = false;
			StartCoroutine ("EsperarSomTerminar");
		}
			



        //Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.HitPoint].Current -= damager_.CalculateDamage();
        /*if (OnHitPointChanged != null)
            OnHitPointChanged(this);*/
    }

	IEnumerator EsperarSomTerminar(){
		yield return new WaitForSeconds (sound.clip.length);
		audioisplaying = true;
	}


    public virtual float CalculateDamage()
    {
        return 0;
    }

    #endregion

    #region Propriedades Programadas 

    /// <summary>
    /// Retorna o ponto de frente do personagem na posicao Global
    /// </summary>
    public Vector3 GetForwardPosition
    {
        get { return this.transform.position + this.transform.forward; }
    }

    /// <summary>
    /// Retorno o ponto da direita do personagem na posicao Global
    /// </summary>
    public Vector3 GetRightPosition
    {
        get { return this.transform.position + this.transform.right; }
    }

    public AttributeBase Aggro
    {
        get
        {
            return Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Aggro];
        }
    }

    public AttributeBase Armor
    {
        get
        {
            return Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Armor];
        }
    }

    public AttributeBase HitPoint
    {
        get
        {
            return Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.HitPoint];
        }
    }

    public AttributeBase EnergyPoint
    {
        get
        {
            return Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.EnergyPoint];
        }
    }

    public AttributeBase Stamina
    {
        get
        {
            return Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Stamina];
        }
    }

    public AttributeBase Speed
    {
        get
        {
            return Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Speed];
        }
    }

    #endregion
}