using System;
using Timberborn.Goods;

namespace Timberborn.DistributionSystem;

public class GoodDistributionSetting
{
	private readonly GoodSpec _goodSpec;

	public float ExportThreshold { get; private set; }

	public ImportOption ImportOption { get; private set; }

	public float LastImportTimestamp { get; set; }

	public string GoodId => _goodSpec.Id;

	public event EventHandler SettingChanged;

	private GoodDistributionSetting(GoodSpec goodSpec)
	{
		_goodSpec = goodSpec;
	}

	public static GoodDistributionSetting CreateDefault(GoodSpec goodSpec)
	{
		GoodDistributionSetting goodDistributionSetting = new GoodDistributionSetting(goodSpec);
		goodDistributionSetting.SetDefault();
		return goodDistributionSetting;
	}

	public static GoodDistributionSetting Create(GoodSpec goodSpec, float exportThreshold, ImportOption importOption, float lastImportTimestamp)
	{
		return new GoodDistributionSetting(goodSpec)
		{
			ExportThreshold = exportThreshold,
			ImportOption = importOption,
			LastImportTimestamp = lastImportTimestamp
		};
	}

	public void SetDefault()
	{
		ExportThreshold = 0f;
		ImportOption = ((!_goodSpec.ForceImport) ? ImportOption.Auto : ImportOption.Forced);
		SettingChanged?.Invoke(this, EventArgs.Empty);
	}

	public void SetImportOption(ImportOption importOption)
	{
		ImportOption = importOption;
		SettingChanged?.Invoke(this, EventArgs.Empty);
	}

	public void SetExportThreshold(float exportThreshold)
	{
		ExportThreshold = exportThreshold;
		SettingChanged?.Invoke(this, EventArgs.Empty);
	}
}
