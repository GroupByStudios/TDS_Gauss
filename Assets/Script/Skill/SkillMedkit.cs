using UnityEngine;
using System.Collections;

public class SkillMedkit : SkillBase {

	public override void SetupSkill ()
	{
		// TODO: Mudar a maneira como a skill é carregada
		this.ID = GetInstanceID(); // TODO: Mudar a busca do ID;

		this.SkillName = "Medkit";
		this.SkillDescription = "Kit médico para cura imediata, após colocado no chão tempo de ativacao {0}";
		this.SkillType = SkillTypeEnum.Heal;
		this.ActivationType = SkillActivationEnum.Activation;
		this.SkillBehaviour = SkillBehaviourEnum.Object;
		this.CoolDown = 2.5f;
		this.EnergyCost = 50f;
		this.NeedTarget = false;

		this.Activated = false;
		this.ActivationTime = 1f;

		// Medkit aplica 25% de cura de HP 
		this.AttributeModifiers = new AttributeModifier[1];
		this.AttributeModifiers[0] = new AttributeModifier();
		this.AttributeModifiers[0].OriginID = this.ID;
		this.AttributeModifiers[0].ApplyTo = ENUMERATORS.Attribute.AttributeModifierApplyToEnum.Current;
		this.AttributeModifiers[0].AttributeType = ENUMERATORS.Attribute.CharacterAttributeTypeEnum.HitPoint;
		this.AttributeModifiers[0].CalcType = ENUMERATORS.Attribute.AttributeModifierCalcTypeEnum.Percent;
		this.AttributeModifiers[0].ModifierType = ENUMERATORS.Attribute.AttributeModifierTypeEnum.OneTimeOnly;
		this.AttributeModifiers[0].Value = 25;

		base.SetupSkill ();
	}

	public override void OnCollisionEnter (Collision collidedWith_)
	{
		base.OnCollisionEnter (collidedWith_);
	}

	public override void OnTriggerEnter (Collider trigerWith_)
	{
		if (this.Activated)
		{
			base.OnTriggerEnter (trigerWith_);

			// Process medkit
			Character _character = trigerWith_.gameObject.GetComponent<Player>() as Character;

			if (_character != null)
			{
				_character.AddAttributeModifier(this.Pool.DefaultParent.gameObject.GetComponent<SkillMedkit>().AttributeModifiers);
				base.SetupSkill();
				this.ReturnToPool();
			}
		}
	}

	public override void SpawnSkill ()
	{
		base.SpawnSkill ();
		this.Activated = false;
		this.transform.position = Caster.transform.position;
	}

}
