using UnityEngine;
using System.Collections;

/***********************************************************************************************************************************/
/**                                                   ATENCAO                                                                     **/
/** TODAS AS SPELLS DEVERAO SER CARREGADAS ATRAVES DE SEUS IDS.                                                                   **/
/** TODAS AS SPELLS DEVERAO TER SEUS PREFABS SALVOS NA PASTA: ASSETS/RESOURCES/PREFAB/SPELL                                       **/
/** TODAS AS SPELLS DEVERAO SER OBTIDAS ATRAVES DO POOL MANAGER DO APPLICATIONMODEL												  **/
/***********************************************************************************************************************************/

/// <summary>
/// Define uma classe base de habilidade
/// </summary>
public class SpellBase : PoolObject {

	public SpellActivationEnum ActivationType;
	public int ID; // ID da Spell para carregar da Tabela de Habilidaes
	public string SpellName;
	public Character Caster; /// Personagem que gerou a habilidade
	public Character Target; // Alvo da Habilidade se houver;
	public bool NeedTarget; /// Magia precisa de um alvo?

	public AttributeModifier[] AttributeModifiers; /// Modificadores que serao aplicados ao personagem, se houverem;

	public float Speed; /// Velocidade da Magia, se necessario

	public float Damage; /// Dano base da habilidade se houver
	public float CoolDown; /// CoolDown da habilidade em segundos
	public float ManaCost; /// Custo de mana para usar a habilidade

	// Use this for initialization
	protected virtual void Start () {
		// Inicializa o Array de Modificadores
		AttributeModifiers = new AttributeModifier[CONSTANTS.ATTRIBUTES.ATTRIBUTE_COUNT];
	}
	
	// Update is called once per frame
	protected override void Update () {
	
	}

	/// <summary>
	/// Metodo responsavel por conter o comportamento da Spell, caso tenha um
	/// </summary>
	public virtual void SpellBaseAction()
	{
		
	}
}

public enum SpellActivationEnum
{
	Action,
	Passive,
	PassiveOnCriticTaken,
	PassiveOnCriticHit
}


