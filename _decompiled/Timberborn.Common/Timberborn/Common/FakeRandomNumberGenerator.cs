using System;
using UnityEngine;

namespace Timberborn.Common;

internal class FakeRandomNumberGenerator : IFakeRandomNumberGenerator
{
	private readonly int _hashCode;

	public FakeRandomNumberGenerator(int hashCode)
	{
		_hashCode = hashCode;
	}

	public float Range(float inclusiveMin, float inclusiveMax, int byteIndex)
	{
		return Mathf.Lerp(inclusiveMin, inclusiveMax, NormalizedFloat(byteIndex));
	}

	public byte Byte(int byteIndex)
	{
		if ((byteIndex < 0 || byteIndex > 3) ? true : false)
		{
			throw new ArgumentException("byteIndex must be between 0 and 3.");
		}
		return (byte)(_hashCode >> byteIndex * 8);
	}

	private float NormalizedFloat(int byteIndex)
	{
		return (float)(int)Byte(byteIndex) / 255f;
	}
}
