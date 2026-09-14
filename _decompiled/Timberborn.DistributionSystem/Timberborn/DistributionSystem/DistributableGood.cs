using System;

namespace Timberborn.DistributionSystem;

public readonly struct DistributableGood : IComparable<DistributableGood>
{
	private readonly GoodDistributionSetting _goodDistributionSetting;

	public int Stock { get; }

	public int Capacity { get; }

	public float FillRate
	{
		get
		{
			if (Capacity != 0)
			{
				return (float)Stock / (float)Capacity;
			}
			if (Stock != 0)
			{
				return 1f;
			}
			return 0f;
		}
	}

	public bool CanExport
	{
		get
		{
			if (_goodDistributionSetting.ExportThreshold < 1f)
			{
				return ExportRate > 0f;
			}
			return false;
		}
	}

	public float MaxExportAmount => ExportRate * (float)Capacity;

	public int FreeCapacity => Capacity - Stock;

	public string GoodId => _goodDistributionSetting.GoodId;

	private float ExportRate => FillRate - _goodDistributionSetting.ExportThreshold;

	public DistributableGood(int stock, int capacity, GoodDistributionSetting goodDistributionSetting)
	{
		Stock = stock;
		Capacity = capacity;
		_goodDistributionSetting = goodDistributionSetting;
	}

	public void UpdateLastImportTimestamp(float timestamp)
	{
		_goodDistributionSetting.LastImportTimestamp = timestamp;
	}

	public int CompareTo(DistributableGood other)
	{
		int num = FillRate.CompareTo(other.FillRate);
		if (num != 0)
		{
			return num;
		}
		return _goodDistributionSetting.LastImportTimestamp.CompareTo(other._goodDistributionSetting.LastImportTimestamp);
	}
}
