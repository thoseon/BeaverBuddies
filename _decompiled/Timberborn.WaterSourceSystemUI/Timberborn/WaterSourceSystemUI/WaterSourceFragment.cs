using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.EntityUndoSystem;
using Timberborn.Localization;
using Timberborn.WaterSourceSystem;
using UnityEngine.UIElements;

namespace Timberborn.WaterSourceSystemUI;

internal class WaterSourceFragment : IEntityPanelFragment
{
	private static readonly string StrengthLocKey = "Water.Strength";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ILoc _loc;

	private readonly WaterSettingFactory _waterSettingFactory;

	private readonly EntityChangeRecorderFactory _entityChangeRecorderFactory;

	private VisualElement _root;

	private WaterSource _waterSource;

	private WaterSourceContamination _waterSourceContamination;

	private readonly List<WaterSetting> _waterSettings = new List<WaterSetting>();

	public WaterSourceFragment(VisualElementLoader visualElementLoader, ILoc loc, WaterSettingFactory waterSettingFactory, EntityChangeRecorderFactory entityChangeRecorderFactory)
	{
		_visualElementLoader = visualElementLoader;
		_loc = loc;
		_waterSettingFactory = waterSettingFactory;
		_entityChangeRecorderFactory = entityChangeRecorderFactory;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Common/EntityPanel/WaterSourceFragment");
		_root.ToggleDisplayStyle(visible: false);
		AddSettings();
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_waterSource = entity.GetComponent<WaterSource>();
		if ((bool)(BaseComponent)(object)_waterSource)
		{
			_waterSourceContamination = entity.GetComponent<WaterSourceContamination>();
			UpdateWaterSettings();
			if (_waterSettings.Any((WaterSetting setting) => setting.Visible))
			{
				_root.ToggleDisplayStyle(visible: true);
			}
		}
		else
		{
			ClearFragment();
		}
	}

	public void ClearFragment()
	{
		_root.ToggleDisplayStyle(visible: false);
		_waterSource = null;
		_waterSourceContamination = null;
	}

	public void UpdateFragment()
	{
		if ((bool)(BaseComponent)(object)_waterSource)
		{
			UpdateWaterSettings();
		}
	}

	private void AddSettings()
	{
		AddSetting(_loc.T(StrengthLocKey), SetWaterSourceStrength, () => _waterSource.SpecifiedStrength, devModeOnly: false);
		AddSetting("Current strength", delegate
		{
		}, () => _waterSource.CurrentStrength, devModeOnly: true);
		AddSetting("Contamination", delegate(float value)
		{
			_waterSourceContamination.SetContamination(value / 100f);
		}, () => _waterSourceContamination.Contamination * 100f, devModeOnly: true);
	}

	private void AddSetting(string label, Action<float> setter, Func<float> getter, bool devModeOnly)
	{
		WaterSetting waterSetting = _waterSettingFactory.Create(label, setter, getter, devModeOnly);
		_waterSettings.Add(waterSetting);
		_root.Add(waterSetting.Root);
	}

	private void SetWaterSourceStrength(float value)
	{
		using (_entityChangeRecorderFactory.CreateChangeRecorder((BaseComponent)(object)_waterSource))
		{
			_waterSource.SetSpecifiedStrength(value);
		}
	}

	private void UpdateWaterSettings()
	{
		foreach (WaterSetting waterSetting in _waterSettings)
		{
			waterSetting.UpdateState();
		}
	}
}
