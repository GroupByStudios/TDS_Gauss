using UnityEngine;
using System.Collections;

public class SkillPickup: SkillBase {

	public override void SetupSkill ()
	{
		// TODO: Mudar a maneira como a skill é carregada
		//this.ID = GetInstanceID(); // TODO: Mudar a busca do ID;

		this.SkillName = "Ammo Kit";
		this.SkillDescription = "Kit de municao";
		this.SkillType = SkillTypeEnum.Buf;
		this.ActivationType = SkillActivationEnum.Activation;
		this.SkillBehaviour = SkillBehaviourEnum.Object;
		this.CoolDown = 0.5f;
		this.EnergyCost = 50f;
		this.NeedTarget = false;

		this.Activated = false;
		this.ActivationTime = 1f;

		// Medkit aplica 25% de cura de HP 
		this.AttributeModifiers = new AttributeModifier[1];
		this.AttributeModifiers[0] = new AttributeModifier();
		this.AttributeModifiers[0].OriginID = (int)this.SkillID;
		this.AttributeModifiers[0].Modifier = ENUMERATORS.Attribute.AttributeBaseTypeEnum.Weapon;
		this.AttributeModifiers[0].ApplyTo = ENUMERATORS.Attribute.AttributeModifierApplyToEnum.Max;
		this.AttributeModifiers[0].AttributeType = (int)ENUMERATORS.Attribute.WeaponAttributeTypeEnum.Ammo;
		this.AttributeModifiers[0].CalcType = ENUMERATORS.Attribute.AttributeModifierCalcTypeEnum.Value;
		this.AttributeModifiers[0].ModifierType = ENUMERATORS.Attribute.AttributeModifierTypeEnum.OneTimeOnly;
		this.AttributeModifiers[0].ApplyAs = ENUMERATORS.Attribute.AttributeModifierApplyAsEnum.Constant;
		this.AttributeModifiers[0].CanSetToCurrentExceedMax = false;
		this.AttributeModifiers[0].Value = 24;


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
			Player _player = trigerWith_.gameObject.GetComponent<Player>();

			if (_player != null)
			{
				if (this.AttributeModifiers != null)
				{
					AttributeModifier[] _modifiersToSet;

					for (int i = 0; i < this.AttributeModifiers.Length; i++)
					{
						_modifiersToSet = null;
						switch(this.AttributeModifiers[i].Modifier)
						{
						case ENUMERATORS.Attribute.AttributeBaseTypeEnum.Character:
							_modifiersToSet = _player.AttributeModifiers;
							break;
						case ENUMERATORS.Attribute.AttributeBaseTypeEnum.Weapon:
							_modifiersToSet = _player.CurrentWeapon.AttributeModifiers;
							break;
						default:break;
						}

						if (_modifiersToSet != null)
						{
							AttributeModifier.AddAttributeModifier(ref _modifiersToSet, this.AttributeModifiers[i]);
						}
					}
				}

				base.SetupSkill();
				this.ReturnToPool();
			}
		} 
	}

	public override void SpawnSkill ()
	{
		this.Activated = false;
		this.ActivateWithTime();
		base.SpawnSkill ();
		this.transform.position = Caster.transform.position;
	}

}
