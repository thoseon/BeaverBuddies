using System;
using UnityEngine;

namespace Timberborn.PrefabOptimization;

public readonly struct VerticalShapeInfo : IEquatable<VerticalShapeInfo>
{
	public int TotalPrefabCount { get; }

	public GameObject StartPrefab { get; }

	public GameObject RepeatingPrefab { get; }

	public string Name { get; }

	public VerticalShapeInfo(int totalPrefabCount, GameObject startPrefab, GameObject repeatingPrefab, string name)
	{
		TotalPrefabCount = totalPrefabCount;
		StartPrefab = startPrefab;
		RepeatingPrefab = repeatingPrefab;
		Name = name;
	}

	public bool Equals(VerticalShapeInfo other)
	{
		if (TotalPrefabCount == other.TotalPrefabCount && object.Equals(StartPrefab, other.StartPrefab) && object.Equals(RepeatingPrefab, other.RepeatingPrefab))
		{
			return Name == other.Name;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is VerticalShapeInfo other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(TotalPrefabCount, StartPrefab, RepeatingPrefab, Name);
	}
}
