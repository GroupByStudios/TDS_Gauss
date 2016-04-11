using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class GodShrine : MonoBehaviour {

	public bool IsActivated; // Altar esta habilitado
	public float CoolDown; // CoolDown do Altar em Segundos
	public ENUMERATORS.Attribute.AttributeModifierTypeEnum ModifierType; // Tipo do modificador. Constante ou Por Tempo
	public ENUMERATORS.Attribute.CharacterAttributeTypeEnum AttributeType; // Qual atributo que sera modificado: Hit Point, ManaPoint, Damage, etc..
	public ENUMERATORS.Attribute.AttributeModifierCalcTypeEnum CalcType; // Tipo de calculo a ser aplicado como modificador. Valor ou Percentual
	public ENUMERATORS.Attribute.AttributeModifierApplyToEnum ApplyTo; // Modificador deve ser aplicado ao valor atual ou maximo
	public float Value; // Valor constante a ser aplicado - Deve seguir as regras de sinais por exemplo -50 ou +50 para remover ou adicionar. Valor percentual a ser aplicado - Deve seguir as regras de sinais por exemplo -50% ou +50% do atributo
	public float TimeInSeconds; // Tempo em segundos que o modificador sera aplicado no atributo

	AttributeModifier Modifier;
	BoxCollider _activationCollider;
	float _nextCoolDown;

	void Start()
	{
		_activationCollider	= GetComponent<BoxCollider>();
		_activationCollider.isTrigger = true;

		Modifier = new AttributeModifier();
		Modifier.OriginID = this.gameObject.GetInstanceID();
		Modifier.ModifierType = ModifierType;
		Modifier.AttributeType = (int)AttributeType;
		Modifier.CalcType = CalcType;
		Modifier.ApplyTo = ApplyTo;
		Modifier.Value = Value;
		Modifier.TimeInSeconds = TimeInSeconds;
	}

	void Update()
	{
		if (!IsActivated && _nextCoolDown < Time.time)
			_activationCollider.enabled = true;
	}

	public AttributeModifier Activate()
	{
		_activationCollider.enabled = false;
		_nextCoolDown = Time.time + CoolDown;
		return Modifier;
	}




}
