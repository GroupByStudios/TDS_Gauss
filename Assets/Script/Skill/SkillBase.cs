using UnityEngine;
using System.Collections;

public class SkillBase : PoolObject
{
	public int ID;
	public SkillActivationEnum ActivationType;
	public SkillTypeEnum SkillType;
	public SkillBehaviourEnum SkillBehaviour;
	public string SkillName;
	public string SkillDescription;

	public Character Caster;
	public Character Target;
	public AttributeModifier[] AttributeModifiers;

	public bool NeedTarget;

	public float EnergyCost;
	public float CoolDown;

	public bool Activated;
	public float ActivationTime;

	public virtual void SetupSkill()
	{
		Caster = null;
		Target = null;
	}

	public void Activate()
	{
		this.Activated = true;
	}

	public void ActivateWithTime()
	{
		Invoke("Activate", ActivationTime);
	}

	public virtual void SpawnSkill()
	{
	}

	public virtual void OnCollisionEnter(Collision collidedWith_)
	{
	}

	public virtual void OnTriggerEnter(Collider trigerWith_)
	{
	}

	public virtual bool CastCheck(out string message)
	{
		if (Caster == null)
		{
			message = "A habilidade não possuí um Caster";
			Debug.Log(message);
			return false;
		}

		if (NeedTarget && Target == null)
		{
			message = "A skill precisa de um alvo";
			Debug.Log(message);
			return false;
		}

		message = string.Empty;

		return true;
	}
}

public enum SkillActivationEnum
{
	Passive,
	Activation
}

public enum SkillTypeEnum
{
	Heal,
	Damage,
	Buf,
	Debuf
}

public enum SkillBehaviourEnum
{
	None,
	Object,
	Bolt,
 	Aura,
}
