using System;
using UnityEngine;

public static class Vector3Helper
{
	public static bool V3Equal(this Vector3 a, Vector3 b){
		return Vector3.SqrMagnitude(a - b) < 0.0001;
	}
}

