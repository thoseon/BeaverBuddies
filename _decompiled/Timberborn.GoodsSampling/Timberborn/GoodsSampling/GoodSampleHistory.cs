using System.Collections.Generic;
using Timberborn.Common;
using UnityEngine;

namespace Timberborn.GoodsSampling;

public class GoodSampleHistory
{
	private readonly List<GoodSample> _goodSamples;

	public string GoodId { get; }

	public ReadOnlyList<GoodSample> GoodSamples => _goodSamples.AsReadOnlyList();

	private GoodSampleHistory(string goodId, List<GoodSample> goodSamples)
	{
		GoodId = goodId;
		_goodSamples = goodSamples;
	}

	public static GoodSampleHistory CreateNew(string goodId)
	{
		return new GoodSampleHistory(goodId, new List<GoodSample>());
	}

	public static GoodSampleHistory CreateFromSave(string goodId, List<GoodSample> goodSamples)
	{
		return new GoodSampleHistory(goodId, goodSamples);
	}

	public void Add(GoodSample goodSample)
	{
		_goodSamples.Add(goodSample);
	}

	public IEnumerable<GoodSample> GetGoodSamples(int timeRange, int amount)
	{
		float step = ((float)timeRange - 1f) / ((float)amount - 1f);
		float index = (float)_goodSamples.Count - 1f - (float)(amount - 1) * step;
		int i = 0;
		while (i < amount)
		{
			int index2 = Mathf.RoundToInt(index);
			if (index >= 0f)
			{
				yield return _goodSamples[index2];
			}
			else
			{
				yield return new GoodSample(0, 0, 0, 0, 0, 0);
			}
			i++;
			index += step;
		}
	}
}
