using System;

namespace Timberborn.DecalSystem;

public readonly struct Decal : IEquatable<Decal>
{
	public string Id { get; }

	public string Category { get; }

	public bool IsEmpty => string.IsNullOrEmpty(Id);

	public Decal(string id, string category)
	{
		Id = id;
		Category = category;
	}

	public bool Equals(Decal other)
	{
		if (Id == other.Id)
		{
			return Category == other.Category;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is Decal other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Id, Category);
	}
}
