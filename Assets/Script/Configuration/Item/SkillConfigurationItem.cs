using UnityEngine;
using System.Collections;

public class SkillConfigurationItem : ScriptableObject
{
    //public SkillActivationEnum ActivationType { get; set; }
    public int ID { get; set; }
    public string SpellName { get; set; }
    public bool NeedTarget { get; set; }
    public AttributeModifier[] AttributeModifiers { get; set; }
    public float Speed { get; set; }
    public float Damage { get; set; }
    public float CoolDown { get; set; }
    public float ManaCost { get; set; }
    public string Prefab { get; set; }
    public string Class { get; set; }
}

