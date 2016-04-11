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
	public CharacterAttribute[] Attributes = InitializeAttributes();
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
		CheckAttributeModifiers(); // Verifica os atributos
		ApplyAttributesModifiers(); // Aplica os modificadores	
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
	private static CharacterAttribute[] InitializeAttributes()
	{
		CharacterAttribute[] _tempCharAttribute = new CharacterAttribute[CONSTANTS.ATTRIBUTES.ATTRIBUTE_COUNT];

		_tempCharAttribute[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Aggro] = 
			new CharacterAttribute(){ 			
			Name = CONSTANTS.ATTRIBUTES.TYPE_NAMES[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Aggro], 
			AttributeType = ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Aggro,
			Max = 0,
			MaxModifiers = 0,
			Current = 0,
			CurrentModifiers = 0,
			DisplayOrder = 0};
		_tempCharAttribute[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Armor] = 
			new CharacterAttribute(){ 			
			Name = CONSTANTS.ATTRIBUTES.TYPE_NAMES[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Armor], 
			AttributeType = ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Armor,
			Max = 0,
			MaxModifiers = 0,
			Current = 0,
			CurrentModifiers = 0,
			DisplayOrder = 0};
		_tempCharAttribute[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.CriticChance] = 		
			new CharacterAttribute(){ 			
			Name = CONSTANTS.ATTRIBUTES.TYPE_NAMES[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.CriticChance], 
			AttributeType = ENUMERATORS.Attribute.CharacterAttributeTypeEnum.CriticChance,
			Max = 0,
			MaxModifiers = 0,
			Current = 0,
			CurrentModifiers = 0,
			DisplayOrder = 1};
		_tempCharAttribute[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.CriticMultiplier] = 		
			new CharacterAttribute(){ 			
			Name = CONSTANTS.ATTRIBUTES.TYPE_NAMES[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.CriticMultiplier], 
			AttributeType = ENUMERATORS.Attribute.CharacterAttributeTypeEnum.CriticMultiplier,
			Max = 0,
			MaxModifiers = 0,
			Current = 0,
			CurrentModifiers = 0,
			DisplayOrder = 2};
		_tempCharAttribute[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Damage] = 		
			new CharacterAttribute(){ 			
			Name = CONSTANTS.ATTRIBUTES.TYPE_NAMES[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Damage], 
			AttributeType = ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Damage,
			Max = 0,
			MaxModifiers = 0,
			Current = 0,
			CurrentModifiers = 0,
			DisplayOrder = 0};
		_tempCharAttribute[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.HitPoint] = 		
			new CharacterAttribute(){ 			
			Name = CONSTANTS.ATTRIBUTES.TYPE_NAMES[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.HitPoint], 
			AttributeType = ENUMERATORS.Attribute.CharacterAttributeTypeEnum.HitPoint,
			Max = 0,
			MaxModifiers = 0,
			Current = 0,
			CurrentModifiers = 0,
			DisplayOrder = 0};
		_tempCharAttribute[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.ShootSpeed] = 
			new CharacterAttribute(){ 			
			Name = CONSTANTS.ATTRIBUTES.TYPE_NAMES[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.ShootSpeed], 
			AttributeType = ENUMERATORS.Attribute.CharacterAttributeTypeEnum.ShootSpeed,
			Max = 0,
			MaxModifiers = 0,
			Current = 0,
			CurrentModifiers = 0,
			DisplayOrder = 0};
		_tempCharAttribute[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Speed] = 
			new CharacterAttribute(){ 			
			Name = CONSTANTS.ATTRIBUTES.TYPE_NAMES[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Speed], 
			AttributeType = ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Speed,
			Max = 0,
			MaxModifiers = 0,
			Current = 0,
			CurrentModifiers = 0,
			DisplayOrder = 0};
		_tempCharAttribute[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Stamina] = 
			new CharacterAttribute(){ 			
			Name = CONSTANTS.ATTRIBUTES.TYPE_NAMES[(int)ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Stamina], 
			AttributeType = ENUMERATORS.Attribute.CharacterAttributeTypeEnum.Stamina,
			Max = 0,
			MaxModifiers = 0,
			Current = 0,
			CurrentModifiers = 0,
			DisplayOrder = 0};

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

	/// <summary>
	/// Metodo responsavel por gerenciar a adicao de modificadores de atributos ao personagem
	/// </summary>
	/// <param name="attributeModifier_">Attribute modifier.</param>
	public void AddAttributeModifier(AttributeModifier attributeModifier_)
	{
		bool _canBeAdded = true; // Variavel para controle se o atributo pode ser adicionao na tabela, parte do principio que sempre pode.

		// Percorre a tabela de atributos do personagem
		for (int i = 0; i < AttributeModifiers.Length; i++)
		{
			// Se o espaco estiver vazio e pode ser adicionado inclui. Para a execucao do for
			if (AttributeModifiers[i] == null && _canBeAdded){
				AttributeModifiers[i] = attributeModifier_;
				AttributeModifiers[i].InitialTime = Time.time;
				AttributeModifiers[i].ExpireTime = Time.time + attributeModifier_.TimeInSeconds;
				AttributeModifiers[i].Consumed = false;
				break;
			}

			// Verifica se o atributo existe na tabela e inicia os testes
			if (AttributeModifiers[i] != null)
			{
				// Se estiver aplicando o mesmo atributo somente atualiza na tabela
				if (AttributeModifiers[i].AttributeType == attributeModifier_.AttributeType &&
					AttributeModifiers[i].OriginID == attributeModifier_.OriginID &&
					AttributeModifiers[i].ModifierType == attributeModifier_.ModifierType)
				{
					AttributeModifiers[i] = attributeModifier_;
					AttributeModifiers[i].InitialTime = Time.time;
					AttributeModifiers[i].ExpireTime = Time.time + attributeModifier_.TimeInSeconds;
					AttributeModifiers[i].Consumed = false;
					break;					
				}
			}
		}
	}

	/// <summary>
	/// Metodo responsavel por gerenciar a adicao de modificadores de atributos ao personagem
	/// </summary>
	/// <param name="attributeModifier_">Lista de Attribute modifier.</param>
	public void AddAttributeModifier(AttributeModifier[] attributeModifier_)
	{
		if (attributeModifier_ != null){
			for(int i = 0; i < attributeModifier_.Length; i++)
			{
				this.AddAttributeModifier(attributeModifier_[i]);
			}
		}
	}

	#endregion

	#region Metodos de Calculos dos Atributos

	/// <summary>
	/// Metodo responsavel por remover os modificadores de atributos que expiraram
	/// </summary>
	void CheckAttributeModifiers()
	{
		bool _needToReorder = false;

		for(int i = 0; i < AttributeModifiers.Length; i++)
		{
			if (AttributeModifiers[i] != null)
			{
				// Modificador de atributo por Tempo e Expirou o Tempo = Remove da Tabela de Modificadores e marca que precisa reorganizar a tabela
				if (AttributeModifiers[i].ModifierType == ENUMERATORS.Attribute.AttributeModifierTypeEnum.Time &&
					AttributeModifiers[i].ExpireTime < Time.time)
				{
					AttributeModifiers[i] = null;
					_needToReorder = true;
					continue;
				}

				// Se for modificador para ser usado uma unica vez e esta marcado como consumido exclui da tabela
				if (AttributeModifiers[i].ModifierType == ENUMERATORS.Attribute.AttributeModifierTypeEnum.OneTimeOnly &&
					AttributeModifiers[i].Consumed)
				{
					AttributeModifiers[i] = null;
					_needToReorder = true;
					continue;					
				}
			}
			else
				break;
		}

		// Reorganiza a tabela se algum modificador foi removido
		if (_needToReorder)
		{
			Helper.ReorderArray<AttributeModifier>(AttributeModifiers);
		}
	}

	/// <summary>
	/// Metodo responsavel por limpar os Modificadores
	/// </summary>
	void CleanAttributesModifiers()
	{
		for(int i = 0; i < Attributes.Length; i++)
		{
			Attributes[i].CurrentModifiers = 0;
			Attributes[i].MaxModifiers = 0;

		}
	}

	/// <summary>
	/// Metodo responsavel por aplicar os modificadores de atributos
	/// </summary>
	void ApplyAttributesModifiers()
	{
		int _attributeTypeIndex;

		for (int i = 0; i < AttributeModifiers.Length; i++)
		{
			if (AttributeModifiers[i] != null)
			{
				_attributeTypeIndex = (int)AttributeModifiers[i].AttributeType;

				switch(AttributeModifiers[i].CalcType)
				{
				case ENUMERATORS.Attribute.AttributeModifierCalcTypeEnum.Percent:

					switch(AttributeModifiers[i].ApplyTo)
					{
					case ENUMERATORS.Attribute.AttributeModifierApplyToEnum.Max:

						switch (AttributeModifiers[i].ApplyAs)
						{
						case ENUMERATORS.Attribute.AttributeModifierApplyAsEnum.Temporary:
							Attributes[_attributeTypeIndex].MaxModifiers += (Attributes[_attributeTypeIndex].Max * AttributeModifiers[i].Value / 100);
							break;
						case ENUMERATORS.Attribute.AttributeModifierApplyAsEnum.Constant:
							Attributes[_attributeTypeIndex].Max += (Attributes[_attributeTypeIndex].Max * AttributeModifiers[i].Value / 100);

						break;
						}
						break;
					case ENUMERATORS.Attribute.AttributeModifierApplyToEnum.Current:

						switch (AttributeModifiers[i].ApplyAs)
						{
						case ENUMERATORS.Attribute.AttributeModifierApplyAsEnum.Temporary:
							Attributes[_attributeTypeIndex].CurrentModifiers += (Attributes[_attributeTypeIndex].Current * AttributeModifiers[i].Value / 100);							
							break;
						case ENUMERATORS.Attribute.AttributeModifierApplyAsEnum.Constant:
							Attributes[_attributeTypeIndex].Current += (Attributes[_attributeTypeIndex].Current * AttributeModifiers[i].Value / 100);		// PERCENTUAL SEMPRE EM CIMA DO MAXIMO
							break;							
						}

						break;
					case ENUMERATORS.Attribute.AttributeModifierApplyToEnum.Both:

							switch (AttributeModifiers[i].ApplyAs)
							{
							case ENUMERATORS.Attribute.AttributeModifierApplyAsEnum.Temporary:
								Attributes[_attributeTypeIndex].MaxModifiers += (Attributes[_attributeTypeIndex].Max * AttributeModifiers[i].Value / 100);
								Attributes[_attributeTypeIndex].CurrentModifiers += (Attributes[_attributeTypeIndex].Current * AttributeModifiers[i].Value / 100);
								break;
							case ENUMERATORS.Attribute.AttributeModifierApplyAsEnum.Constant:
								Attributes[_attributeTypeIndex].Max += (Attributes[_attributeTypeIndex].Max * AttributeModifiers[i].Value / 100);
								Attributes[_attributeTypeIndex].Current += (Attributes[_attributeTypeIndex].Max * AttributeModifiers[i].Value / 100);	// PERCENTUAL SEMPRE EM CIMA DO MAXIMO
								break;
							}
						break;
					}

					break;
				case ENUMERATORS.Attribute.AttributeModifierCalcTypeEnum.Value:

					switch(AttributeModifiers[i].ApplyTo)
					{
					case ENUMERATORS.Attribute.AttributeModifierApplyToEnum.Max:

						switch(AttributeModifiers[i].ApplyAs)
						{
						case ENUMERATORS.Attribute.AttributeModifierApplyAsEnum.Temporary:
							Attributes[_attributeTypeIndex].MaxModifiers += AttributeModifiers[i].Value;
							break;
						case ENUMERATORS.Attribute.AttributeModifierApplyAsEnum.Constant:
							Attributes[_attributeTypeIndex].Max += AttributeModifiers[i].Value;
							break;
						}

						break;
					case ENUMERATORS.Attribute.AttributeModifierApplyToEnum.Current:

						switch(AttributeModifiers[i].ApplyAs)
						{
						case ENUMERATORS.Attribute.AttributeModifierApplyAsEnum.Temporary:
							Attributes[_attributeTypeIndex].CurrentModifiers += AttributeModifiers[i].Value;
							break;
						case ENUMERATORS.Attribute.AttributeModifierApplyAsEnum.Constant:
							Attributes[_attributeTypeIndex].Current += AttributeModifiers[i].Value;
							break;
						}

						break;
					case ENUMERATORS.Attribute.AttributeModifierApplyToEnum.Both:

						switch(AttributeModifiers[i].ApplyAs)
						{
						case ENUMERATORS.Attribute.AttributeModifierApplyAsEnum.Temporary:
							Attributes[_attributeTypeIndex].MaxModifiers += AttributeModifiers[i].Value;
							Attributes[_attributeTypeIndex].CurrentModifiers += AttributeModifiers[i].Value;
							break;
						case ENUMERATORS.Attribute.AttributeModifierApplyAsEnum.Constant:
							Attributes[_attributeTypeIndex].Max += AttributeModifiers[i].Value;
							Attributes[_attributeTypeIndex].Current += AttributeModifiers[i].Value;							
							break;
						}

						break;
					}

					break;
				}

				// Marca o atributo como consumido
				AttributeModifiers[i].Consumed = true;

				if (!AttributeModifiers[i].CanSetToCurrentExceedMax && 
					Attributes[(int)AttributeModifiers[i].AttributeType].Current > Attributes[(int)AttributeModifiers[i].AttributeType].Max)
				{
					Attributes[(int)AttributeModifiers[i].AttributeType].Current = Attributes[(int)AttributeModifiers[i].AttributeType].Max;
				}
			}
		}
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