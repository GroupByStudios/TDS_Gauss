using System;
using UnityEngine;



/// <summary>
/// Classe responsavel por manter os modificadores de atributos, aplicados a Spells e Runas ou Debufs de Inimigos. UM MODIFICADOR DE ATRIBUTO NUNCA SOBRESCREVE O VALOR DO ATRIBUTO SEMPRE ADICIONA OU SUBTRAI
/// </summary>
public class AttributeModifier
{
	
	[HideInInspector] public int OriginID; // ID do objeto que gerou o modificador
	public ENUMERATORS.Attribute.AttributeBaseTypeEnum Modifier;
	public ENUMERATORS.Attribute.AttributeModifierTypeEnum ModifierType; // Tipo do modificador. Constante ou Por Tempo
	public int AttributeType; // Qual atributo que sera modificado: Hit Point, ManaPoint, Damage, etc..
	public ENUMERATORS.Attribute.AttributeModifierCalcTypeEnum CalcType; // Tipo de calculo a ser aplicado como modificador. Valor ou Percentual
	public ENUMERATORS.Attribute.AttributeModifierApplyToEnum ApplyTo; // Modificador deve ser aplicado ao valor atual ou maximo
	public ENUMERATORS.Attribute.AttributeModifierApplyAsEnum ApplyAs; // modificador deve ser aplicado como temporario ou constante
	public float Value; // Valor constante a ser aplicado - Deve seguir as regras de sinais por exemplo -50 ou +50 para remover ou adicionar. Valor percentual a ser aplicado - Deve seguir as regras de sinais por exemplo -50% ou +50% do atributo
	public float TimeInSeconds; // Tempo em segundos que o modificador sera aplicado no atributo
	[HideInInspector] public float InitialTime; // Tempo de Jogo que o atributo foi aplicado;
	[HideInInspector] public float ExpireTime; // Tempo de jogo que o atributo irá expirar;
	[HideInInspector] public bool Consumed; // Determina se o modificador foi consumido ou nao;
	public bool CanSetToCurrentExceedMax = false;

	public AttributeModifier(){}

	/// <summary>
	/// Construtor
	/// </summary>
	public AttributeModifier(int originID_, 
		ENUMERATORS.Attribute.AttributeModifierTypeEnum modifierType_, 
		ENUMERATORS.Attribute.CharacterAttributeTypeEnum attributeType_, 
		ENUMERATORS.Attribute.AttributeModifierCalcTypeEnum calcType_, 
		ENUMERATORS.Attribute.AttributeModifierApplyToEnum applyTo_,
		float value_,
		float timeInSeconds_,
		float initialTime_
	)
	{
		OriginID = originID_;
		ModifierType = modifierType_;
		AttributeType = (int)attributeType_;
		CalcType = calcType_;
		ApplyTo = applyTo_;
		Value = value_;
		TimeInSeconds = timeInSeconds_;
		InitialTime = initialTime_;
		ExpireTime = InitialTime + TimeInSeconds;
		Consumed = false;
	}

	/// <summary>
	/// Metodo responsavel por gerenciar a adicao de modificadores de atributos ao personagem
	/// </summary>
	/// <param name="attributeModifier_">Attribute modifier.</param>
	public static void AddAttributeModifier(ref AttributeModifier[] attributeModifiers_, AttributeModifier attributeModifier_)
	{
		bool _canBeAdded = true; // Variavel para controle se o atributo pode ser adicionao na tabela, parte do principio que sempre pode.

		// Percorre a tabela de atributos do personagem
		for (int i = 0; i < attributeModifiers_.Length; i++)
		{
			// Se o espaco estiver vazio e pode ser adicionado inclui. Para a execucao do for
			if (attributeModifiers_[i] == null && _canBeAdded){
				attributeModifiers_[i] = attributeModifier_;
				attributeModifiers_[i].InitialTime = Time.time;
				attributeModifiers_[i].ExpireTime = Time.time + attributeModifier_.TimeInSeconds;
				attributeModifiers_[i].Consumed = false;
				break;
			}

			// Verifica se o atributo existe na tabela e inicia os testes
			if (attributeModifiers_[i] != null)
			{
				// Se estiver aplicando o mesmo atributo somente atualiza na tabela
				if (attributeModifiers_[i].AttributeType == attributeModifier_.AttributeType &&
					attributeModifiers_[i].OriginID == attributeModifier_.OriginID &&
					attributeModifiers_[i].ModifierType == attributeModifier_.ModifierType)
				{
					attributeModifiers_[i] = attributeModifier_;
					attributeModifiers_[i].InitialTime = Time.time;
					attributeModifiers_[i].ExpireTime = Time.time + attributeModifier_.TimeInSeconds;
					attributeModifiers_[i].Consumed = false;
					break;					
				}
			}
		}
	}

	/// <summary>
	/// Metodo responsavel por gerenciar a adicao de modificadores de atributos ao personagem
	/// </summary>
	/// <param name="attributeModifier_">Lista de Attribute modifier.</param>
	public static void AddAttributeModifier(ref AttributeModifier[] attributeModifiers_, AttributeModifier[] attributeModifierList_)
	{
		if (attributeModifierList_ != null){
			for(int i = 0; i < attributeModifierList_.Length; i++)
			{
				AttributeModifier.AddAttributeModifier(ref attributeModifiers_, attributeModifierList_[i]);
			}
		}
	}


	public static void CleanAttributesModifiers(ref AttributeBase[] attributes_)
	{
		for(int i = 0; i < attributes_.Length; i++)
		{
			attributes_[i].CurrentModifiers = 0;
			attributes_[i].MaxModifiers = 0;
		}		
	}

	public static void CheckAttributeModifiers(ref AttributeModifier[] attributeModifiers_)
	{
		bool _needToReorder = false;

		for(int i = 0; i < attributeModifiers_.Length; i++)
		{
			if (attributeModifiers_[i] != null)
			{
				// Modificador de atributo por Tempo e Expirou o Tempo = Remove da Tabela de Modificadores e marca que precisa reorganizar a tabela
				if (attributeModifiers_[i].ModifierType == ENUMERATORS.Attribute.AttributeModifierTypeEnum.Time &&
					attributeModifiers_[i].ExpireTime < Time.time)
				{
					attributeModifiers_[i] = null;
					_needToReorder = true;
					continue;
				}

				// Se for modificador para ser usado uma unica vez e esta marcado como consumido exclui da tabela
				if (attributeModifiers_[i].ModifierType == ENUMERATORS.Attribute.AttributeModifierTypeEnum.OneTimeOnly &&
					attributeModifiers_[i].Consumed)
				{
					attributeModifiers_[i] = null;
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
			Helper.ReorderArray<AttributeModifier>(attributeModifiers_);
		}	
	}

	public static void ApplyAttributesModifiers(ref AttributeBase[] attributes, ref AttributeModifier[] attributeModifiers_)
	{
		int _attributeTypeIndex;

		for (int i = 0; i < attributeModifiers_.Length; i++)
		{
			if (attributeModifiers_[i] != null)
			{
				_attributeTypeIndex = (int)attributeModifiers_[i].AttributeType;

				switch(attributeModifiers_[i].CalcType)
				{
				case ENUMERATORS.Attribute.AttributeModifierCalcTypeEnum.Percent:

					switch(attributeModifiers_[i].ApplyTo)
					{
					case ENUMERATORS.Attribute.AttributeModifierApplyToEnum.Max:

						switch (attributeModifiers_[i].ApplyAs)
						{
						case ENUMERATORS.Attribute.AttributeModifierApplyAsEnum.Temporary:
							attributes[_attributeTypeIndex].MaxModifiers += (attributes[_attributeTypeIndex].Max * attributeModifiers_[i].Value / 100);
							break;
						case ENUMERATORS.Attribute.AttributeModifierApplyAsEnum.Constant:
							attributes[_attributeTypeIndex].Max += (attributes[_attributeTypeIndex].Max * attributeModifiers_[i].Value / 100);

							break;
						}
						break;
					case ENUMERATORS.Attribute.AttributeModifierApplyToEnum.Current:

						switch (attributeModifiers_[i].ApplyAs)
						{
						case ENUMERATORS.Attribute.AttributeModifierApplyAsEnum.Temporary:
							attributes[_attributeTypeIndex].CurrentModifiers += (attributes[_attributeTypeIndex].Current * attributeModifiers_[i].Value / 100);							
							break;
						case ENUMERATORS.Attribute.AttributeModifierApplyAsEnum.Constant:
							attributes[_attributeTypeIndex].Current += (attributes[_attributeTypeIndex].Current * attributeModifiers_[i].Value / 100);		// PERCENTUAL SEMPRE EM CIMA DO MAXIMO
							break;							
						}

						break;
					case ENUMERATORS.Attribute.AttributeModifierApplyToEnum.Both:

						switch (attributeModifiers_[i].ApplyAs)
						{
						case ENUMERATORS.Attribute.AttributeModifierApplyAsEnum.Temporary:
							attributes[_attributeTypeIndex].MaxModifiers += (attributes[_attributeTypeIndex].Max * attributeModifiers_[i].Value / 100);
							attributes[_attributeTypeIndex].CurrentModifiers += (attributes[_attributeTypeIndex].Current * attributeModifiers_[i].Value / 100);
							break;
						case ENUMERATORS.Attribute.AttributeModifierApplyAsEnum.Constant:
							attributes[_attributeTypeIndex].Max += (attributes[_attributeTypeIndex].Max * attributeModifiers_[i].Value / 100);
							attributes[_attributeTypeIndex].Current += (attributes[_attributeTypeIndex].Max * attributeModifiers_[i].Value / 100);	// PERCENTUAL SEMPRE EM CIMA DO MAXIMO
							break;
						}
						break;
					}

					break;
				case ENUMERATORS.Attribute.AttributeModifierCalcTypeEnum.Value:

					switch(attributeModifiers_[i].ApplyTo)
					{
					case ENUMERATORS.Attribute.AttributeModifierApplyToEnum.Max:

						switch(attributeModifiers_[i].ApplyAs)
						{
						case ENUMERATORS.Attribute.AttributeModifierApplyAsEnum.Temporary:
							attributes[_attributeTypeIndex].MaxModifiers += attributeModifiers_[i].Value;
							break;
						case ENUMERATORS.Attribute.AttributeModifierApplyAsEnum.Constant:
							attributes[_attributeTypeIndex].Max += attributeModifiers_[i].Value;
							break;
						}

						break;
					case ENUMERATORS.Attribute.AttributeModifierApplyToEnum.Current:

						switch(attributeModifiers_[i].ApplyAs)
						{
						case ENUMERATORS.Attribute.AttributeModifierApplyAsEnum.Temporary:
							attributes[_attributeTypeIndex].CurrentModifiers += attributeModifiers_[i].Value;
							break;
						case ENUMERATORS.Attribute.AttributeModifierApplyAsEnum.Constant:
							attributes[_attributeTypeIndex].Current += attributeModifiers_[i].Value;
							break;
						}

						break;
					case ENUMERATORS.Attribute.AttributeModifierApplyToEnum.Both:

						switch(attributeModifiers_[i].ApplyAs)
						{
						case ENUMERATORS.Attribute.AttributeModifierApplyAsEnum.Temporary:
							attributes[_attributeTypeIndex].MaxModifiers += attributeModifiers_[i].Value;
							attributes[_attributeTypeIndex].CurrentModifiers += attributeModifiers_[i].Value;
							break;
						case ENUMERATORS.Attribute.AttributeModifierApplyAsEnum.Constant:
							attributes[_attributeTypeIndex].Max += attributeModifiers_[i].Value;
							attributes[_attributeTypeIndex].Current += attributeModifiers_[i].Value;							
							break;
						}

						break;
					}

					break;
				}

				// Marca o atributo como consumido
				attributeModifiers_[i].Consumed = true;

				// Aplica os valores maximos caso o modificador de atributo esteja configurado para nao exceder
				if (!attributeModifiers_[i].CanSetToCurrentExceedMax && 
					attributes[(int)attributeModifiers_[i].AttributeType].Current > attributes[(int)attributeModifiers_[i].AttributeType].Max)
				{
					attributes[(int)attributeModifiers_[i].AttributeType].Current = attributes[(int)attributeModifiers_[i].AttributeType].Max;
				}
			}
		}		
	}
}

