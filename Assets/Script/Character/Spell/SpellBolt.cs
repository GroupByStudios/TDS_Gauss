using UnityEngine;
using System;

/// <summary>
/// Habilidade com o comportamento de Bolt
/// </summary>
public class SpellBolt : SpellBase
{
	/// <summary>
	/// Implementa o comportamento de bolt na spell
	/// </summary>
	protected override void Update ()
	{
		base.Update ();

		if (!IsExpired()){

			transform.Translate(Vector3.forward * Speed * Time.deltaTime);

		}
	}

	void OnCollisionEnter(Collision collision_)
	{
		if (collision_ != null)
		{
			Character _characterHitted = collision_.gameObject.GetComponent<Character>();

			if (_characterHitted != null){
				if (_characterHitted != Caster)
				{
					//_characterHitted.ApplyDamage(Caster, ENUMERATORS.Combat.DamageType.Magic);
					ReturnToPool();
				}
			}
			else
			{
				ReturnToPool();
			}
		}
	}

}

