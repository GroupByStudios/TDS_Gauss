using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerSkills : MonoBehaviour {

	[HideInInspector] public SpellBase[] PassiveSkills; // Passivas disponiveis
	[HideInInspector] public SpellBase CurrentPassiveSkill; // Passiva ativa

	[HideInInspector] public SpellBase[] ActionSkills; // Skills Disponíveis

	private Dictionary<DPADController, SpellBase> AssignedSkills; // Skills associadas aos comandos;
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

		AssignedSkills = new Dictionary<DPADController, SpellBase>();
		AssignedSkills.Add(DPADController.UP, null);
		AssignedSkills.Add(DPADController.RIGHT, null);
		AssignedSkills.Add(DPADController.DOWN, null);
		AssignedSkills.Add(DPADController.LEFT, null);

		SkillCoolDownTable = new Dictionary<int, float>();

		switch(myPlayer.PlayerClassID)
		{
		case CONSTANTS.PLAYER.MEDIC:
			break;
		case CONSTANTS.PLAYER.DEFENDER:
			break;
		case CONSTANTS.PLAYER.ENGINEER:
			break;
		case CONSTANTS.PLAYER.ASSAULT:
			break;
		case CONSTANTS.PLAYER.SPECIALIST:
			break;
		}

		// Adiciona as skills ativas na tabela de CoolDown
		for (int i = 0; i < ActionSkills.Length; i++)
		{
			SkillCoolDownTable.Add(ActionSkills[i].ID, 0);
		}

		// Adiciona as skills passivas na tabela de CoolDown
		for (int i = 0; i < PassiveSkills.Length; i++)
		{
			SkillCoolDownTable.Add(PassiveSkills[i].ID, 0);
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

		if (CurrentPassiveSkill != null && CurrentPassiveSkill.ActivationType == SpellActivationEnum.PassiveOnCriticTaken)
		{
			// Executa a skill passiva ativada por receber um critico
			CurrentPassiveSkill.SpellBaseAction();
		}
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

		if (CurrentPassiveSkill != null && CurrentPassiveSkill.ActivationType == SpellActivationEnum.PassiveOnCriticHit)
		{
			// Executa a skill passiva ativada por receber dar um dano critico
			CurrentPassiveSkill.SpellBaseAction();
		}
	}

	/// <summary>
	/// Metodo responsavel por associar uma skill ao comando do DPAD
	/// </summary>
	/// <param name="dpadController_">Dpad controller.</param>
	/// <param name="spellBase_">Spell base.</param>
	public void AssignSkillToDPad(DPADController dpadController_, SpellBase spellBase_)
	{
		AssignedSkills[dpadController_] = spellBase_;
	}

	/// <summary>
	/// Metodo responsavel por retornar uma skill a partir do DPad selecionado
	/// </summary>
	/// <returns>The skill by command.</returns>
	/// <param name="dpadController_">Dpad controller.</param>
	public SpellBase GetSkillByCommand(DPADController dpadController_)
	{
		return AssignedSkills[dpadController_];
	}

}
