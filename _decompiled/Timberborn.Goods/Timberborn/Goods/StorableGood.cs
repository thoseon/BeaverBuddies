using System;

namespace Timberborn.Goods;

public readonly struct StorableGood : IEquatable<StorableGood>
{
	public string GoodId { get; }

	public bool Takeable { get; }

	public bool Givable { get; }

	public bool IsOnlyTakeable
	{
		get
		{
			if (!Givable)
			{
				return Takeable;
			}
			return false;
		}
	}

	public bool IsOnlyGivable
	{
		get
		{
			if (Givable)
			{
				return !Takeable;
			}
			return false;
		}
	}

	private StorableGood(string goodId, bool takeable, bool givable)
	{
		GoodId = goodId;
		Takeable = takeable;
		Givable = givable;
	}

	public static StorableGood CreateAsTakeable(string goodId)
	{
		return new StorableGood(goodId, takeable: true, givable: false);
	}

	public static StorableGood CreateAsGivable(string goodId)
	{
		return new StorableGood(goodId, takeable: false, givable: true);
	}

	public static StorableGood CreateGiveableAndTakeable(string goodId)
	{
		return new StorableGood(goodId, takeable: true, givable: true);
	}

	public override string ToString()
	{
		return string.Format("{0}: {1}, {2}: {3},", "GoodId", GoodId, "Takeable", Takeable) + string.Format(" {0}: {1}", "Givable", Givable);
	}

	public bool Equals(StorableGood other)
	{
		if (object.Equals(GoodId, other.GoodId) && Takeable == other.Takeable)
		{
			return Givable == other.Givable;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is StorableGood other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (((((GoodId != null) ? GoodId.GetHashCode() : 0) * 397) ^ Takeable.GetHashCode()) * 397) ^ Givable.GetHashCode();
	}

	public static bool operator ==(StorableGood left, StorableGood right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(StorableGood left, StorableGood right)
	{
		return !left.Equals(right);
	}
}
