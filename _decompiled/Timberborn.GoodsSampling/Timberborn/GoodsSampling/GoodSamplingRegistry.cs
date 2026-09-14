using System.Collections.Generic;
using Timberborn.Common;

namespace Timberborn.GoodsSampling;

public class GoodSamplingRegistry
{
	private readonly Dictionary<string, GoodSampleHistory> _goodSampleHistoryMap = new Dictionary<string, GoodSampleHistory>();

	private readonly List<GoodSampleHistory> _goodSampleHistories = new List<GoodSampleHistory>();

	public ReadOnlyList<GoodSampleHistory> GoodSampleHistories => _goodSampleHistories.AsReadOnlyList();

	private GoodSamplingRegistry()
	{
	}

	public static GoodSamplingRegistry CreateNew(IReadOnlyList<string> goodIds)
	{
		GoodSamplingRegistry goodSamplingRegistry = new GoodSamplingRegistry();
		foreach (string goodId in goodIds)
		{
			goodSamplingRegistry.AddMissingGood(goodId);
		}
		return goodSamplingRegistry;
	}

	public static GoodSamplingRegistry CreateFromSave(List<GoodSampleHistory> goodSampleHistories, IReadOnlyList<string> goodIds)
	{
		GoodSamplingRegistry goodSamplingRegistry = new GoodSamplingRegistry();
		foreach (GoodSampleHistory goodSampleHistory in goodSampleHistories)
		{
			goodSamplingRegistry._goodSampleHistoryMap[goodSampleHistory.GoodId] = goodSampleHistory;
			goodSamplingRegistry._goodSampleHistories.Add(goodSampleHistory);
		}
		foreach (string goodId in goodIds)
		{
			if (!goodSamplingRegistry._goodSampleHistoryMap.ContainsKey(goodId))
			{
				goodSamplingRegistry.AddMissingGood(goodId);
			}
		}
		return goodSamplingRegistry;
	}

	public void AddSample(string goodId, GoodSample goodSample)
	{
		_goodSampleHistoryMap[goodId].Add(goodSample);
	}

	public GoodSampleHistory GetGoodSampleHistory(string goodId)
	{
		return _goodSampleHistoryMap[goodId];
	}

	private void AddMissingGood(string goodId)
	{
		GoodSampleHistory goodSampleHistory = GoodSampleHistory.CreateNew(goodId);
		_goodSampleHistoryMap[goodId] = goodSampleHistory;
		_goodSampleHistories.Add(goodSampleHistory);
	}
}
