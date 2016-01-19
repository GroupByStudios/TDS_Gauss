using System;
using UnityEngine;



/// <summary>
/// Classe responsavel por manter os modificadores de atributos, aplicados a Spells e Runas ou Debufs de Inimigos. UM MODIFICADOR DE ATRIBUTO NUNCA SOBRESCREVE O VALOR DO ATRIBUTO SEMPRE ADICIONA OU SUBTRAI
/// </summary>
public class AttributeModifier
{
	
	[HideInInspector] public int OriginID; // ID do objeto que gerou o modificador
	public ENUMERATORS.Attribute.AttributeModifierTypeEnum ModifierType; // Tipo do modificador. Constante ou Por Tempo
	public ENUMERATORS.Attribute.CharacterAttributeTypeEnum AttributeType; // Qual atributo que sera modificado: Hit Point, ManaPoint, Damage, etc..
	public ENUMERATORS.Attribute.AttributeModifierCalcTypeEnum CalcType; // Tipo de calculo a ser aplicado como modificador. Valor ou Percentual
	public ENUMERATORS.Attribute.AttributeModifierApplyToEnum ApplyTo; // Modificador deve ser aplicado ao valor atual ou maximo
	public float Value; // Valor constante a ser aplicado - Deve seguir as regras de sinais por exemplo -50 ou +50 para remover ou adicionar. Valor percentual a ser aplicado - Deve seguir as regras de sinais por exemplo -50% ou +50% do atributo
	public float TimeInSeconds; // Tempo em segundos que o modificador sera aplicado no atributo
	[HideInInspector] public float InitialTime; // Tempo de Jogo que o atributo foi aplicado;
	[HideInInspector] public float ExpireTime; // Tempo de jogo que o atributo irá expirar;
	[HideInInspector] public bool Consumed; // Determina se o modificador foi consumido ou nao;

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
		AttributeType = attributeType_;
		CalcType = calcType_;
		ApplyTo = applyTo_;
		Value = value_;
		TimeInSeconds = timeInSeconds_;
		InitialTime = initialTime_;
		ExpireTime = InitialTime + TimeInSeconds;
		Consumed = false;
	}
}

