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
			Stamina = 1,			
			Speed = 2,
			Armor = 3,
			Damage = 4,
			ShootSpeed = 5,
			CriticChance = 6,
			CriticMultiplier = 7,
			Aggro = 8
		}

		/// <summary>
		/// Enumerador para determinar se o modificador deve ser aplicado no valor maximo ou atual de um atributo
		/// </summary>
		public enum AttributeModifierApplyToEnum
		{
			Max, // Modificador deve ser aplicado ao valor maximo do atributo
			Current, // Modificador deve ser aplicado ao valor atual do atributo
			Both // Modificador sera aplicado no valor maximo e atual do atributo
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
