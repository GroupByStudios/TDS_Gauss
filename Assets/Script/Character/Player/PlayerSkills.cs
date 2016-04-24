using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerSkills {

	[HideInInspector] public SkillBase[] PassiveSkills; // Passivas disponiveis
	[HideInInspector] public SkillBase CurrentPassiveSkill; // Passiva ativa

	private Dictionary<DPADController, SkillBase> AssignedSkills; // Skills associadas aos comandos;
	private Dictionary<int, float> SkillCoolDownTable;

	private Player myPlayer; // Jogador associado

	/// <summary>
	/// Comandos disponiveis das Skills
	/// </summary>
	public enum DPADController
	{
		UP,
		RIGHT,
		DOWN,
		LEFT
	}

	/// <summary>
	/// Método responsavel por carregar as skills de uma determinada classe
	/// </summary>
	/// <param name="playerClassId_">Player class identifier.</param>
	public void InitializePlayerSkills(Player playerOwner_)
	{
		myPlayer = playerOwner_;

		AssignedSkills = new Dictionary<DPADController, SkillBase>();
		AssignedSkills.Add(DPADController.UP, null);
		AssignedSkills.Add(DPADController.RIGHT, null);
		AssignedSkills.Add(DPADController.DOWN, null);
		AssignedSkills.Add(DPADController.LEFT, null);

		SkillCoolDownTable = new Dictionary<int, float>();

		switch(myPlayer.PlayerClassID)
		{
		case CONSTANTS.PLAYER.MEDIC:

			AssignedSkills[DPADController.LEFT] = ApplicationModel.Instance.GetSpellPool(ENUMERATORS.Spell.SkillID.SKILL_01_HEAL_KIT);

			break;
		case CONSTANTS.PLAYER.DEFENDER:

			AssignedSkills[DPADController.LEFT] = ApplicationModel.Instance.GetSpellPool(ENUMERATORS.Spell.SkillID.SKILL_04_AGRESSIVENESS);

			break;
		case CONSTANTS.PLAYER.ENGINEER:

			AssignedSkills[DPADController.LEFT] = ApplicationModel.Instance.GetSpellPool(ENUMERATORS.Spell.SkillID.SKILL_02_AMMO_KIT);

			break;
		case CONSTANTS.PLAYER.ASSAULT:

			AssignedSkills[DPADController.LEFT] = ApplicationModel.Instance.GetSpellPool(ENUMERATORS.Spell.SkillID.SKILL_05_FRENZY_SHOT);

			break;
		case CONSTANTS.PLAYER.SPECIALIST:

			AssignedSkills[DPADController.LEFT] = ApplicationModel.Instance.GetSpellPool(ENUMERATORS.Spell.SkillID.SKILL_03_CROSSING_SHOT);

			break;
		}

		myPlayer.OnCriticDamageHit += MyPlayer_OnCriticDamageHit;
		myPlayer.OnCriticDamageTaken += MyPlayer_OnCriticDamageTaken;
	}

	/// <summary>
	/// Evento executado quando o personagem toma um hit critico
	/// </summary>
	/// <param name="Attacker">Attacker.</param>
	/// <param name="Receiver">Receiver.</param>
	/// <param name="Damage">Damage.</param>
	void MyPlayer_OnCriticDamageTaken (Character Attacker, Character Receiver, float Damage)
	{
		Debug.Log("Disparou evento de quem recebeu dano critico sem o uso do Update");

		/*if (CurrentPassiveSkill != null && CurrentPassiveSkill.ActivationType == SpellActivationEnum.PassiveOnCriticTaken)
		{
			// Executa a skill passiva ativada por receber um critico
			CurrentPassiveSkill.SpellBaseAction();
		}*/
	}

	/// <summary>
	/// Evento executado quando o personagem acerta um hit critico
	/// </summary>
	/// <param name="Attacker">Attacker.</param>
	/// <param name="Receiver">Receiver.</param>
	/// <param name="Damage">Damage.</param>
	void MyPlayer_OnCriticDamageHit (Character Attacker, Character Receiver, float Damage)
	{
		Debug.Log("Disparou evento de quem deu dano critico sem o uso do Update");

		/*if (CurrentPassiveSkill != null && CurrentPassiveSkill.ActivationType == SpellActivationEnum.PassiveOnCriticHit)
		{
			// Executa a skill passiva ativada por receber dar um dano critico
			CurrentPassiveSkill.SpellBaseAction();
		}*/
	}

	/// <summary>
	/// Metodo responsavel por associar uma skill ao comando do DPAD
	/// </summary>
	/// <param name="dpadController_">Dpad controller.</param>
	/// <param name="spellBase_">Spell base.</param>
	public void AssignSkillToDPad(DPADController dpadController_, SkillBase spellBase_)
	{
		AssignedSkills[dpadController_] = spellBase_;
	}

	/// <summary>
	/// Metodo responsavel por retornar uma skill a partir do DPad selecionado
	/// </summary>
	/// <returns>The skill by command.</returns>
	/// <param name="dpadController_">Dpad controller.</param>
	public SkillBase GetSkillByCommand(DPADController dpadController_)
	{
		return AssignedSkills[dpadController_];
	}

	public void PerformSkill(DPADController dpadController_)
	{
		SkillBase _assignedSkill = AssignedSkills[dpadController_];

		if (_assignedSkill != null)
		{
			if (CheckCoolDown((int)_assignedSkill.SkillID))
			{
				string _message = null;

				// A partir daqui a skill temporaria do pool
				SkillBase _pooledSkill = (SkillBase)_assignedSkill.Pool.GetFromPool();

				if (_pooledSkill != null)
				{
					_pooledSkill.Caster = myPlayer; // TODO CHANGE

					if (_pooledSkill.CastCheck(out _message))
					{										
						_pooledSkill.SpawnSkill();
						this.SetCoolDown((int)_assignedSkill.SkillID, _assignedSkill.CoolDown);
					}
				}
			}
			else
			{
				// Skill em cooldown
			}
		}
	}

	/// <summary>
	/// Verifica se a habilidade esta em coold down
	/// </summary>
	/// <returns><c>true</c>, if cool down was checked, <c>false</c> otherwise.</returns>
	/// <param name="skillID_">Skill I d.</param>
	private bool CheckCoolDown(int skillID_)
	{
		if (!SkillCoolDownTable.ContainsKey(skillID_))
			return true;

		if (ApplicationModel.Instance.GlobalTime > SkillCoolDownTable[skillID_])
		{
			SkillCoolDownTable.Remove(skillID_);
			return true;
		}

		return false;
	}

	/// <summary>
	/// Calcula o tempo de CoolDown
	/// </summary>
	/// <returns><c>true</c>, if cool down was set, <c>false</c> otherwise.</returns>
	/// <param name="skillID_">Skill I d.</param>
	/// <param name="coolDownTime_">Cool down time.</param>
	private void SetCoolDown(int skillID_, float coolDownTime_)
	{
		if (!SkillCoolDownTable.ContainsKey(skillID_))
		{
			SkillCoolDownTable.Add(skillID_, ApplicationModel.Instance.GlobalTime + coolDownTime_);
		}
	}
}
