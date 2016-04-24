using System;

/// <summary>
/// Classe responsavel por manter todos os enumeradores do Jogo
/// </summary>
public class ENUMERATORS{

	/// <summary>
	/// Enumeradores especificos para inimigos
	/// </summary>
	public class Enemy{

		/// <summary>
		/// Enumerador para definir o tipo do inimigo
		/// </summary>
		public enum EnemyTypeEnum
		{
			Normal,
			MiniBoss,
			Boss
		}

		/// <summary>
		/// Enumerador para definir o tipo do ataque dos inimigos
		/// </summary>
		public enum EnemyAttackTypeEnum
		{
			Melee,
			Ranged,
			Stationary
		}

		/// <summary>
		/// Enumerador para definir a Maquina de estado do inimigo
		/// </summary>
		public enum EnemyStateEnum
		{
			SearchingPlayer,
			HasPlayer,
		}

	}

	/// <summary>
	/// Enumeradores especificos para habilidades
	/// </summary>
	public class Spell{
		
		/// <summary>
		/// Define os tipos de habilidades
		/// </summary>
		public enum SpellTypeEnum
		{
			Passive,
			Active
		}

		public enum SkillID
		{
			SKILL_01_HEAL_KIT 		= 1,
			SKILL_02_AMMO_KIT 		= 2,
			SKILL_03_CROSSING_SHOT 	= 3,
			SKILL_04_AGRESSIVENESS	= 4,
			SKILL_05_FRENZY_SHOT    = 5
		}

	}

	/// <summary>
	/// Enumeradores especificos para atributos
	/// </summary>
	public class Attribute{

		/// <summary>
		/// Enumerador para definir a tabela dos atributos disponiveis no jogo
		/// </summary>
		public enum CharacterAttributeTypeEnum
		{
			HitPoint = 0,
			EnergyPoint = 1,
			Stamina = 2,			
			Speed = 3,
			Armor = 4,
			Aggro = 5
		}

		public enum WeaponAttributeTypeEnum
		{
			Damage = 0,
			Ammo = 1,
			FireRate = 2,
			ReloadSpeed = 3,
			CriticChance = 4,
			CriticMultiplier = 5
		}

		/// <summary>
		/// Enumerador para determinar se o modificador deve ser aplicado no valor maximo ou atual de um atributo
		/// </summary>
		public enum AttributeModifierApplyToEnum
		{
			None, // Nenhum dos tres, por exemplo aura de efeito
			Max, // Modificador deve ser aplicado ao valor maximo do atributo
			Current, // Modificador deve ser aplicado ao valor atual do atributo
			Both // Modificador sera aplicado no valor maximo e atual do atributo
		}

		public enum AttributeBaseTypeEnum
		{
			Character,
			Weapon
		}

		/// <summary>
		/// O modificador deve ser aplicado como temporatio ou aplicado aos valores atuais
		/// </summary>
		public enum AttributeModifierApplyAsEnum
		{
			Constant,
			Temporary
		}

		/// <summary>
		/// Enumerador para definir o tipo do modificador de atributo.
		/// </summary>
		public enum AttributeModifierTypeEnum
		{
			Constant,	// Constante = Sempre ativo até ser removido do jogador
			Time,		// Tempo = Ativo por X tempo
			OneTimeOnly // Somente uma vez = Se ativo é aplicado e entao removido
		}

		/// <summary>
		/// Enumerador pra definir a origem do Modificador. Spell, Runa, etc
		/// </summary>
		public enum AttributeModifierOriginEnum
		{
			Spell,
			Rune,
			Potion
		}

		/// <summary>
		/// Enumerador para definir o metodo de calculo do modificaor. Valor ou Percentual
		/// </summary>
		public enum AttributeModifierCalcTypeEnum
		{
			Value,
			Percent
		}

	}

	/// <summary>
	/// Enumeradores especificos para todos os personagens
	/// </summary>
	public class Character {

		/// <summary>
		/// Enumerador dos tipos de Personagens
		/// </summary>
		public enum CharacterTypeEnum
		{
			NPC,
			Player,
			Enemy,
		}

		/// <summary>
		/// Enumeradores para definir o estado do personagem
		/// </summary>
		public enum CharacterState
		{
			Dead,
			Alive
		}
	}

	/// <summary>
	/// Enumeradores especificos de combat
	/// </summary>
	public class Combat {

		/// <summary>
		/// Enumerador para definir o tipo de dano a ser aplicado
		/// </summary>
		public enum DamageType
		{
			Melee,
			Magic
		}
		
	}

}
