using Timberborn.Common;
using Timberborn.Persistence;

namespace Timberborn.Goods;

public class SerializedGoodValueSerializer : IValueSerializer<SerializedGood>
{
	private readonly IGoodService _goodService;

	public SerializedGoodValueSerializer(IGoodService goodService)
	{
		_goodService = goodService;
	}

	public void Serialize(SerializedGood serializedGood, IValueSaver valueSaver)
	{
		valueSaver.AsString(serializedGood.Id);
	}

	public Obsoletable<SerializedGood> Deserialize(IValueLoader valueLoader)
	{
		string goodId = GetGoodId(valueLoader);
		GoodSpec goodOrNull = _goodService.GetGoodOrNull(goodId);
		if (!(goodOrNull != null))
		{
			return default(Obsoletable<SerializedGood>);
		}
		return new SerializedGood(goodOrNull.Id);
	}

	[BackwardCompatible(2025, 1, 31, Compatibility.Map)]
	private static string GetGoodId(IValueLoader valueLoader)
	{
		if (valueLoader.IsObject())
		{
			return valueLoader.AsObject().Get(new PropertyKey<string>("Id"));
		}
		return valueLoader.AsString();
	}
}
