using System;

/// <summary>
/// Classe que define um atributo do personagem
/// </summary>
[Serializable]
public class AttributeBase
{

	public ENUMERATORS.Attribute.AttributeBaseTypeEnum AttributeBaseType;

	/// <summary>
	/// Retorna o nome do atributo.
	/// </summary>
	/// <value>Nome do atributo.</value>
	[ReadOnlyInInspectorAttribute] 
	public string Name;

	/// <summary>
	/// Tipo do Atributo
	/// </summary>
	//[ReadOnlyInInspectorAttribute]
	public int AttributeType;

	/// <summary>
	/// Valor maximo para o atributo
	/// </summary>
	public float Max;

	/// <summary>
	/// Valor dos modificadores
	/// </summary>
	public float MaxModifiers;

	/// <summary>
	/// Valor do atributo atual
	/// </summary>
	public float Current;

	/// <summary>
	/// Valor dos modificadores
	/// </summary>
	public float CurrentModifiers;

	/// <summary>
	/// Valor dos Bufs (Modificadores) aplicados + o valor maximo
	/// </summary>
	/// <value>The max buffed.</value>
	public float MaxWithModifiers
	{
		get { return Max + MaxModifiers; }
	}

	public float CurrentWithModifiers
	{
		get {

			/*if (Current + CurrentModifiers >  MaxWithModifiers)
				return MaxWithModifiers;
			else*/
				return Current + CurrentModifiers;
		}
	}

	/// <summary>
	/// Ordem que o atributo deve ser apresentado;
	/// </summary>
	[ReadOnlyInInspectorAttribute]
	public byte DisplayOrder;
}


