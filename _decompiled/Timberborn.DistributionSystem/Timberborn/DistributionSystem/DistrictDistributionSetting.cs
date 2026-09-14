using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.Goods;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;

namespace Timberborn.DistributionSystem;

public class DistrictDistributionSetting : BaseComponent, IPersistentEntity, IInitializableEntity
{
	private static readonly ComponentKey DistrictDistributionSettingKey = new ComponentKey("DistrictDistributionSetting");

	private static readonly ListKey<GoodDistributionSetting> GoodDistributionSettingsKey = new ListKey<GoodDistributionSetting>("GoodDistributionSettings");

	private readonly GoodDistributionSettingSerializer _goodDistributionSettingSerializer;

	private readonly IGoodService _goodService;

	private readonly List<GoodDistributionSetting> _goodDistributionSettings = new List<GoodDistributionSetting>();

	public ReadOnlyList<GoodDistributionSetting> GoodDistributionSettings => _goodDistributionSettings.AsReadOnlyList();

	public event EventHandler<GoodDistributionSetting> SettingChanged;

	public DistrictDistributionSetting(GoodDistributionSettingSerializer goodDistributionSettingSerializer, IGoodService goodService)
	{
		_goodDistributionSettingSerializer = goodDistributionSettingSerializer;
		_goodService = goodService;
	}

	public void InitializeEntity()
	{
		AddMissingGoodSettings();
	}

	public void Save(IEntitySaver entitySaver)
	{
		entitySaver.GetComponent(DistrictDistributionSettingKey).Set(GoodDistributionSettingsKey, _goodDistributionSettings, _goodDistributionSettingSerializer);
	}

	public void Load(IEntityLoader entityLoader)
	{
		foreach (GoodDistributionSetting item in entityLoader.GetComponent(DistrictDistributionSettingKey).Get(GoodDistributionSettingsKey, _goodDistributionSettingSerializer))
		{
			AddGoodDistributionSetting(item);
		}
	}

	public GoodDistributionSetting GetGoodDistributionSetting(string goodId)
	{
		for (int i = 0; i < _goodDistributionSettings.Count; i++)
		{
			if (_goodDistributionSettings[i].GoodId == goodId)
			{
				return _goodDistributionSettings[i];
			}
		}
		throw new ArgumentException("No good distribution setting for good " + goodId);
	}

	public IEnumerable<GoodDistributionSetting> GetGoodDistributionSettingsForGroup(string groupId)
	{
		foreach (GoodDistributionSetting goodDistributionSetting in _goodDistributionSettings)
		{
			if (_goodService.GetGood(goodDistributionSetting.GoodId).GoodGroupId == groupId)
			{
				yield return goodDistributionSetting;
			}
		}
	}

	public void ResetToDefault()
	{
		foreach (GoodDistributionSetting goodDistributionSetting in _goodDistributionSettings)
		{
			goodDistributionSetting.SetDefault();
		}
	}

	public void SetDistrictExportThreshold(int threshold)
	{
		foreach (GoodDistributionSetting goodDistributionSetting in _goodDistributionSettings)
		{
			goodDistributionSetting.SetExportThreshold(threshold);
		}
	}

	public void SetDistrictImportOption(ImportOption importOption)
	{
		foreach (GoodDistributionSetting goodDistributionSetting in _goodDistributionSettings)
		{
			goodDistributionSetting.SetImportOption(importOption);
		}
	}

	private void AddMissingGoodSettings()
	{
		foreach (string good2 in _goodService.Goods)
		{
			if (IsSettingMissing(good2))
			{
				GoodSpec good = _goodService.GetGood(good2);
				AddGoodDistributionSetting(GoodDistributionSetting.CreateDefault(good));
			}
		}
	}

	private void AddGoodDistributionSetting(GoodDistributionSetting goodDistributionSetting)
	{
		_goodDistributionSettings.Add(goodDistributionSetting);
		goodDistributionSetting.SettingChanged += OnSettingChanged;
	}

	private void OnSettingChanged(object sender, EventArgs e)
	{
		SettingChanged?.Invoke(this, (GoodDistributionSetting)sender);
	}

	private bool IsSettingMissing(string goodId)
	{
		foreach (GoodDistributionSetting goodDistributionSetting in _goodDistributionSettings)
		{
			if (goodDistributionSetting.GoodId == goodId)
			{
				return false;
			}
		}
		return true;
	}
}
