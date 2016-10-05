using UnityEngine;
using System.Collections;

public class SkillEffect : SkillBase {

	public override void SetupSkill ()
	{
		// TODO: Mudar a maneira como a skill é carregada
		//this.ID = GetInstanceID(); // TODO: Mudar a busca do ID;

		/*this.SkillName = "Tiro Certo";
		this.SkillDescription = "Habilidade para permitir que um único tiro atravesse vários inimigos.";
		this.SkillType = SkillTypeEnum.Buf;
		this.ActivationType = SkillActivationEnum.Activation;
		this.SkillBehaviour = SkillBehaviourEnum.Aura;
		this.CoolDown = 0.5f;
		this.EnergyCost = 50f;
		this.NeedTarget = false;

		this.Activated = false;
		this.ActivationTime = 0f; // Instantanea

		this.ExpireInSeconds = 5; // EXPIRACAO DA SKILL

		// Aplica o efeito CROSSING_SHOT NO PERSONAGEM
		this.AttributeModifiers = new AttributeModifier[1];
		this.AttributeModifiers[0] = new AttributeModifier();
		this.AttributeModifiers[0].OriginID = (int)this.SkillID;
		this.AttributeModifiers[0].Modifier = ENUMERATORS.Attribute.AttributeBaseTypeEnum.Character;
		this.AttributeModifiers[0].ModifierType = ENUMERATORS.Attribute.AttributeModifierTypeEnum.Time;
		this.AttributeModifiers[0].TimeInSeconds = 5; // EXPIRACAO DO EFEITO

		this.AttributeModifiers[0].HasSkillEffect = true;
		this.AttributeModifiers[0].SkillEffects = SkillEffects.CROSSING_SHOOT;*/

		base.SetupSkill ();
	}

	public override void SpawnSkill ()
	{
		this.Activated = true;
		base.SpawnSkill ();
		this.transform.SetParent(Caster.transform, false);
		this.transform.localPosition = Vector3.zero;

		if (Caster != null)
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
						
						_modifiersToSet = Caster.AttributeModifiers;

						break;
					case ENUMERATORS.Attribute.AttributeBaseTypeEnum.Weapon:

						if (Caster is Player)
							_modifiersToSet = ((Player)Caster).CurrentWeapon.AttributeModifiers;
						
						break;
					default:break;
					}

					if (_modifiersToSet != null)
					{
						AttributeModifier.AddAttributeModifier(ref _modifiersToSet, this.AttributeModifiers[i]);
					}
				}
			}
		}
	}

	// TODO SOBREESCREVER O METODO DE CAST PARA VERIFICAR SE O JOGADOR JA ESTA COM A HABILIDADE ATIVIDADE, SE SIM REMOVER A ANTERIOR.

}
