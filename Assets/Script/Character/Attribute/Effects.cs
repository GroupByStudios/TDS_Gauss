using System;

/// <summary>
/// Classe responsavel por manter todos os efeitos
/// </summary>
public class Effects
{
	public SkillEffects SkillEffects {get;set;}
}

[System.Flags]
public enum SkillEffects
{
	CROSSING_SHOOT = 0x01,
	FRENZY_SHOOT = 0x02
}
