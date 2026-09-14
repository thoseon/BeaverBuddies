using Timberborn.Goods;
using Timberborn.Persistence;

namespace Timberborn.DistributionSystem;

public class GoodDistributionSettingSerializer : IValueSerializer<GoodDistributionSetting>
{
	private static readonly PropertyKey<string> GoodIdKey = new PropertyKey<string>("GoodId");

	private static readonly PropertyKey<float> ExportThresholdKey = new PropertyKey<float>("ExportThreshold");

	private static readonly PropertyKey<ImportOption> ImportOptionKey = new PropertyKey<ImportOption>("ImportOption");

	private static readonly PropertyKey<float> LastImportTimestampKey = new PropertyKey<float>("LastImportTimestamp");

	private readonly IGoodService _goodService;

	public GoodDistributionSettingSerializer(IGoodService goodService)
	{
		_goodService = goodService;
	}

	public void Serialize(GoodDistributionSetting value, IValueSaver valueSaver)
	{
		IObjectSaver objectSaver = valueSaver.AsObject();
		objectSaver.Set(GoodIdKey, value.GoodId);
		objectSaver.Set(ExportThresholdKey, value.ExportThreshold);
		objectSaver.Set(ImportOptionKey, value.ImportOption);
		objectSaver.Set(LastImportTimestampKey, value.LastImportTimestamp);
	}

	public Obsoletable<GoodDistributionSetting> Deserialize(IValueLoader valueLoader)
	{
		IObjectLoader objectLoader = valueLoader.AsObject();
		string id = objectLoader.Get(GoodIdKey);
		GoodSpec goodOrNull = _goodService.GetGoodOrNull(id);
		if ((object)goodOrNull != null)
		{
			float exportThreshold = objectLoader.Get(ExportThresholdKey);
			ImportOption importOption = objectLoader.Get(ImportOptionKey);
			float lastImportTimestamp = objectLoader.Get(LastImportTimestampKey);
			return GoodDistributionSetting.Create(goodOrNull, exportThreshold, importOption, lastImportTimestamp);
		}
		return default(Obsoletable<GoodDistributionSetting>);
	}
}
