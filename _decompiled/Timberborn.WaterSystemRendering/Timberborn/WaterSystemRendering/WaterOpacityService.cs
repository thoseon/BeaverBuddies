using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.WaterSystemRendering;

public class WaterOpacityService : ILoadableSingleton
{
	private static readonly int WaterOpacityProperty = Shader.PropertyToID("_WaterOpacity");

	private readonly EventBus _eventBus;

	private readonly List<WaterOpacityToggle> _toggles = new List<WaterOpacityToggle>();

	private bool _waterOpacityOverriden;

	public bool IsWaterTransparent { get; private set; }

	public WaterOpacityService(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public WaterOpacityToggle GetWaterOpacityToggle()
	{
		WaterOpacityToggle waterOpacityToggle = new WaterOpacityToggle();
		_toggles.Add(waterOpacityToggle);
		waterOpacityToggle.StateChanged += delegate
		{
			UpdateOpacity();
		};
		return waterOpacityToggle;
	}

	public void Load()
	{
		ToggleOpacity(setTransparent: false);
	}

	public void ToggleOpacityOverride()
	{
		_waterOpacityOverriden = !_waterOpacityOverriden;
		UpdateOpacity();
	}

	private void UpdateOpacity()
	{
		bool flag = _toggles.FastAny((WaterOpacityToggle toggle) => toggle.Hidden) && !_waterOpacityOverriden;
		if (flag != IsWaterTransparent)
		{
			ToggleOpacity(flag);
		}
	}

	private void ToggleOpacity(bool setTransparent)
	{
		float value = (setTransparent ? 0.4f : 1f);
		Shader.SetGlobalFloat(WaterOpacityProperty, value);
		IsWaterTransparent = setTransparent;
		_eventBus.Post(new WaterOpacityChangedEvent(setTransparent));
	}
}
