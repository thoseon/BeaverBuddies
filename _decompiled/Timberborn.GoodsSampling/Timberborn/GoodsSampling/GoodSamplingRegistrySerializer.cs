using Timberborn.Goods;
using Timberborn.Persistence;

namespace Timberborn.GoodsSampling;

public class GoodSamplingRegistrySerializer : IValueSerializer<GoodSamplingRegistry>
{
	private static readonly ListKey<GoodSampleHistory> GoodSampleHistoryKey = new ListKey<GoodSampleHistory>("GoodSampleHistory");

	private readonly GoodSampleHistorySerializer _goodSampleHistorySerializer;

	private readonly IGoodService _goodService;

	public GoodSamplingRegistrySerializer(GoodSampleHistorySerializer goodSampleHistorySerializer, IGoodService goodService)
	{
		_goodSampleHistorySerializer = goodSampleHistorySerializer;
		_goodService = goodService;
	}

	public void Serialize(GoodSamplingRegistry value, IValueSaver valueSaver)
	{
		valueSaver.AsObject().Set(GoodSampleHistoryKey, value.GoodSampleHistories, _goodSampleHistorySerializer);
	}

	public Obsoletable<GoodSamplingRegistry> Deserialize(IValueLoader valueLoader)
	{
		return new Obsoletable<GoodSamplingRegistry>(GoodSamplingRegistry.CreateFromSave(valueLoader.AsObject().Get(GoodSampleHistoryKey, _goodSampleHistorySerializer), _goodService.Goods));
	}
}
