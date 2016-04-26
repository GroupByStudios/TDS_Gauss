using System;
using System.Collections;
using System.Collections.Generic;

using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using UnityEngine;

/// <summary>
/// Classe responsavel por manter funcoes auxiliares ao desenvolvimento
/// </summary>
public static class Helper
{
	/// <summary>
	/// Metodo responsavel por Reorganizar o Array
	/// </summary>
	/// <param name="arrayObject_">Array object.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static void ReorderArray<T>(T[] arrayObject_)
	{
		for(int i = 0; i < arrayObject_.Length; i++)
		{
			if (arrayObject_[i] == null && i < arrayObject_.Length - 1) // Se o espaco estiver vazio percorre a lista a partir do proximo item somente se nao for o ultimo
			{
				for (int j = i + 1; j < arrayObject_.Length; j++) // Percorre os proximos itens
				{
					arrayObject_[j-1] = arrayObject_[j]; // Move o objeto 1 index pra cima
					arrayObject_[j] = default(T); // Anula a referencia ao objeto, nao o objeto?
				}
			}
		}
	}

	/// <summary>
	/// Metodo responsavel por realizar uma copia do objeto
	/// </summary>
	/// <returns>The clone.</returns>
	/// <param name="obj">Object.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static T DeepClone<T>(T obj)
	{
		using (var ms = new MemoryStream())
		{
			var formatter = new BinaryFormatter();
			formatter.Serialize(ms, obj);
			ms.Position = 0;

			return (T) formatter.Deserialize(ms);
		}
	}


}


