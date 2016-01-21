using System;
using UnityEngine;

public class PoolManager
{
	Transform _defaultParent;
	PoolObject[] _pool;
	int _currentIndex;

	public PoolManager()
	{
		
	}

	/// <summary>
	/// Inicializa o Pool
	/// </summary>
	/// <param name="quantity_">Quantity.</param>
	public void Initialize(int quantity_, Transform defaultParent_)
	{
		_pool = new PoolObject[quantity_];
		_currentIndex = 0;
		_defaultParent = defaultParent_;
	}

	/// <summary>
	/// Adiciona objeto ao Pool
	/// </summary>
	/// <param name="object_">Object.</param>
	public void AddObjectToPool(PoolObject templateObject_, int times_)
	{		
		if (_currentIndex > _pool.Length) return; //Nao ha mais espacos para inserir

		for (int i = 0; i < times_; i++){

			_pool[_currentIndex] = GameObject.Instantiate(templateObject_) as PoolObject;
			_pool[_currentIndex].gameObject.SetActive(false);
			_pool[_currentIndex].ObjectAddedToPool();
			_pool[_currentIndex].Pool = templateObject_.Pool; // Recupera a referencia de um unico PoolManager por Template

			_currentIndex++; // Soma mais um para o proximo objeto ser adicionado corretamente
		}

		// Estrutura os filhos dentro do template
		for (int i = 0; i < _pool.Length; i++)
		{
			if (_pool[i] != null)
			{
				_pool[i].transform.parent = _defaultParent;
			}
		}
	}

	/// <summary>
	/// Recupera um objeto livre do pool
	/// </summary>
	/// <returns>The from pool.</returns>
	public PoolObject GetFromPool()
	{
		for (int i = 0; i < _pool.Length; i++)
		{
			if (!_pool[i].gameObject.activeInHierarchy){

				_pool[i].transform.position = Vector3.zero;
				_pool[i].transform.rotation = Quaternion.identity;
				_pool[i].transform.parent = null;
				_pool[i].gameObject.SetActive(true);
				_pool[i].ObjectActivated();

				return _pool[i];
			}
		}

		return null;
	}

	/// <summary>
	/// Retorna objeto para o Pool
	/// </summary>
	public void ReturnToPool(int instanceID_)
	{
		for (int i = 0; i < _pool.Length; i++)
		{
			if (_pool[i].GetInstanceID() == instanceID_)
			{
				_pool[i].transform.position = Vector3.one * 5000;
				_pool[i].transform.rotation = Quaternion.identity;
				_pool[i].transform.parent = _defaultParent;
				_pool[i].gameObject.SetActive(false);
				_pool[i].ObjectDeactivated();
			}
		}
	}
}