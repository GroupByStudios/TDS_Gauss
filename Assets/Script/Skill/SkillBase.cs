using UnityEngine;
using System.Collections;

public class SkillBase : PoolObject
{
	//public int ID;
	public ENUMERATORS.Spell.SkillID SkillID;
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
	public float ReturnToPoolInSeconds;

	public GameObject SkillObjectPreFab;

	GameObject currentSkillGameObj;

	public virtual void SetupSkill()
	{
		Caster = null;
		Target = null;
		base.ExpireInSeconds = this.ReturnToPoolInSeconds;
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
		if (SkillObjectPreFab != null)
		{
			currentSkillGameObj  = GameObject.Instantiate(SkillObjectPreFab, Vector3.zero, Quaternion.identity) as GameObject;

			if (SkillBehaviour == SkillBehaviourEnum.Aura)
			{
				currentSkillGameObj.transform.SetParent(this.transform, false);
				currentSkillGameObj.transform.position = Vector3.zero + Vector3.up * 0.25f;
			}
			else
			{
				currentSkillGameObj.transform.SetParent(this.transform, false);
			}
				
		}
	}

	public override void ObjectDeactivated ()
	{
		Destroy(currentSkillGameObj);
		base.ObjectDeactivated ();
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
