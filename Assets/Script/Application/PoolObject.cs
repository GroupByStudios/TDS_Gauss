using UnityEngine;
using System;

public class PoolObject : MonoBehaviour
{
	[HideInInspector] public PoolManager Pool;
	[HideInInspector] public float ExpireInSeconds;
	[HideInInspector] public float ExpireTime;

	public void SetPoolManagerReference(PoolManager poolManager_)
	{
		Pool = poolManager_;
	}

	public void SetExpireTime(float expireInSeconds_)
	{
		ExpireInSeconds = expireInSeconds_;

		// Seta o tempo para expirar o objeto
		ExpireTime = Time.time + ExpireInSeconds;
	}

	protected virtual void Update()
	{
		IsExpired();
	}

	public virtual bool IsExpired()
	{
		// Verifica se foi setado um tempo de vida para o objeto quando estiver fora do pool
		if (ExpireInSeconds > 0)
		{
			// Verifica se o tempo de vida do objeto expirou
			if (Time.time > ExpireTime)
			{
				// Se expirou retorna para o pool
				ReturnToPool();
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Metodo invocado quando o objeto é removido do pool
	/// </summary>
	public virtual void ObjectActivated()
	{
		SetExpireTime(ExpireInSeconds);
	}

	/// <summary>
	/// Metodo invocado quando obojeto volta para o pool
	/// </summary>
	public virtual void ObjectDeactivated()
	{
	}

	/// <summary>
	/// Metodo invocado quando um objeto e incluso no pool
	/// </summary>
	public virtual void ObjectAddedToPool()
	{
	}



	/// <summary>
	/// Metodo responsavel por definir o retorno do objeto para o Pool
	/// </summary>
	public virtual void ReturnToPool()
	{
		Pool.ReturnToPool(GetInstanceID());
	}
}