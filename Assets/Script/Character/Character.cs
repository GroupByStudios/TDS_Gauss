using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Classe base de todos os personagens do. Qualquer evento comum entre os personagens deve ser implementado aqui
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Character : MonoBehaviour {

	// Delegates

	public delegate void CriticDamage(Character Attacker, Character Receiver, float Damage);
	public delegate void HitPointChanged(Character character);
	public event CriticDamage OnCriticDamageHit;
	public event CriticDamage OnCriticDamageTaken;
	public event HitPointChanged OnHitPointChanged;

	// Tipo do personagem
	public ENUMERATORS.Character.CharacterTypeEnum CharacterType;

	// Atributos de GameDesign
	public AttributeBase[] Attributes = InitializeAttributes();
	public AttributeModifier[] AttributeModifiers = InitializeAttributesModifiers();

	#region Componentes da Unity

	protected Rigidbody _rigidBody;
	protected Animator _animator;

	#endregion


	#region Eventos

	/// <summary>
	/// Evento para gerenciar o hit do Personagem
	/// </summary>
	/// <param name="character">Character.</param>
	void Character_OnHitPointChanged (Character character)
	{
		if (Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.HitPoint].CurrentWithModifiers <= 0){
			Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.HitPoint].Current = 0;
			Die();
		}
	}

	/// <summary>
	/// Expor metodo para execucao do evento de Dano Critico Recebido
	/// </summary>
	/// <param name="Attacker">Attacker.</param>
	/// <param name="Receiver">Receiver.</param>
	/// <param name="Damage">Damage.</param>
	public void Call_OnCriticDamageHit(Character Attacker, Character Receiver, float Damage)
	{
		if (OnCriticDamageHit != null)
			OnCriticDamageHit(Attacker, Receiver, Damage);
	}

	/// <summary>
	/// Expor metodo para execucao do evento de Dano Critico Executado
	/// </summary>
	/// <param name="Attacker">Attacker.</param>
	/// <param name="Receiver">Receiver.</param>
	/// <param name="Damage">Damage.</param>
	public void Call_OnCriticDamageTaken(Character Attacker, Character Receiver, float Damage)
	{
		if (OnCriticDamageTaken != null)
			OnCriticDamageTaken(Attacker, Receiver, Damage);
	}

	#endregion

	#region Unity Methods 

	// Use this for initialization
	protected virtual void Start () {

		// Obtem as instancias dos componentes Unity;
		_rigidBody = GetComponent<Rigidbody>();
		_animator = GetComponent<Animator>();
	}

	protected virtual void OnEnabled()
	{
		// Registra os eventos
		OnHitPointChanged += Character_OnHitPointChanged;
	}

	protected virtual void OnDisabled()
	{
		// Remove os eventos
		OnHitPointChanged -= Character_OnHitPointChanged;
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		AttributeModifier.CleanAttributesModifiers(ref this.Attributes);
		AttributeModifier.CheckAttributeModifiers(ref this.AttributeModifiers);
		AttributeModifier.ApplyAttributesModifiers(ref this.Attributes, ref this.AttributeModifiers);
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
		for(int i = 0 ; i < _tempModifier.Length; i++)
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
			new AttributeBase(){ 			
			Name = CONSTANTS.ATTRIBUTES.CHARACTER_TYPE_NAMES[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Aggro], 
			AttributeType = (int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Aggro,
			Max = 0,
			MaxModifiers = 0,
			Current = 0,
			CurrentModifiers = 0,
			DisplayOrder = 0,
			AttributeBaseType = ENUMERATORS.Attribute.AttributeBaseTypeEnum.Character};
		
		_tempCharAttribute[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Armor] = 
			new AttributeBase(){ 			
			Name = CONSTANTS.ATTRIBUTES.CHARACTER_TYPE_NAMES[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Armor], 
			AttributeType = (int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Armor,
			Max = 0,
			MaxModifiers = 0,
			Current = 0,
			CurrentModifiers = 0,
			DisplayOrder = 0,
			AttributeBaseType = ENUMERATORS.Attribute.AttributeBaseTypeEnum.Character};	
		
		_tempCharAttribute[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.HitPoint] = 		
			new AttributeBase(){ 			
			Name = CONSTANTS.ATTRIBUTES.CHARACTER_TYPE_NAMES[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.HitPoint], 
			AttributeType = (int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.HitPoint,
			Max = 0,
			MaxModifiers = 0,
			Current = 0,
			CurrentModifiers = 0,
			DisplayOrder = 0,
			AttributeBaseType = ENUMERATORS.Attribute.AttributeBaseTypeEnum.Character};

		_tempCharAttribute[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.EnergyPoint] = 		
			new AttributeBase(){ 			
			Name = CONSTANTS.ATTRIBUTES.CHARACTER_TYPE_NAMES[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.EnergyPoint], 
			AttributeType = (int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.EnergyPoint,
			Max = 0,
			MaxModifiers = 0,
			Current = 0,
			CurrentModifiers = 0,
			DisplayOrder = 0,
			AttributeBaseType = ENUMERATORS.Attribute.AttributeBaseTypeEnum.Character};
		
		_tempCharAttribute[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Speed] = 
			new AttributeBase(){ 			
			Name = CONSTANTS.ATTRIBUTES.CHARACTER_TYPE_NAMES[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Speed], 
			AttributeType = (int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Speed,
			Max = 0,
			MaxModifiers = 0,
			Current = 0,
			CurrentModifiers = 0,
			DisplayOrder = 0,
			AttributeBaseType = ENUMERATORS.Attribute.AttributeBaseTypeEnum.Character};
		
		_tempCharAttribute[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Stamina] = 
			new AttributeBase(){ 			
			Name = CONSTANTS.ATTRIBUTES.CHARACTER_TYPE_NAMES[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Stamina], 
			AttributeType = (int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Stamina,
			Max = 0,
			MaxModifiers = 0,
			Current = 0,
			CurrentModifiers = 0,
			DisplayOrder = 0,
			AttributeBaseType = ENUMERATORS.Attribute.AttributeBaseTypeEnum.Character};

		return _tempCharAttribute;
	}

	#endregion

	#region Public Methods

	/// <summary>
	/// Metodo responsavel por gerenciar a morte do personagem, deve ser sobrescrito nas classes que herdam de Character
	/// </summary>
	public virtual void Die()
	{
		Destroy(this.gameObject);
	}

	/// <summary>
	/// Metodo responsavel por calcular o dano recebido do personagem
	/// </summary>
	/// <param name="damager_">Oponente que deferiu o dano</param>
	/// <param name="damageType_">Tipo do Dano</param>
	public void ApplyDamage(Character damager_, float damage_, ENUMERATORS.Combat.DamageType damageType_)
	{		
		Attributes[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.HitPoint].Current -= damage_;

		if (OnHitPointChanged != null)
			OnHitPointChanged(this);
	}


	#endregion

	#region Propriedades Programadas 

	/// <summary>
	/// Retorna o ponto de frente do personagem na posicao Global
	/// </summary>
	public Vector3 GetForwardPosition
	{
		get{ return this.transform.forward + this.transform.position; }
	}

	/// <summary>
	/// Retorno o ponto da direita do personagem na posicao Global
	/// </summary>
	public Vector3 GetRightPosition
	{
		get{ return this.transform.right + this.transform.position; }
	}


	#endregion
}