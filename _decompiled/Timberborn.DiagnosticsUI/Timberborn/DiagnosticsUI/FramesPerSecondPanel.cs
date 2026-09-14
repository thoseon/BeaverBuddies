using Timberborn.CoreUI;
using Timberborn.Debugging;
using Timberborn.Diagnostics;
using Timberborn.SingletonSystem;
using Timberborn.UILayoutSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.DiagnosticsUI;

public class FramesPerSecondPanel : ILoadableSingleton, IUpdatableSingleton
{
	private readonly UILayout _uiLayout;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly FramesPerSecondCounter _framesPerSecondCounter;

	private readonly DevModeManager _devModeManager;

	private readonly UISettings _uiSettings;

	private Label _fps;

	private int _lastAverageFramesPerSecond;

	private int _lastMinFramesPerSecond;

	public FramesPerSecondPanel(UILayout uiLayout, VisualElementLoader visualElementLoader, FramesPerSecondCounter framesPerSecondCounter, DevModeManager devModeManager, UISettings uiSettings)
	{
		_uiLayout = uiLayout;
		_visualElementLoader = visualElementLoader;
		_framesPerSecondCounter = framesPerSecondCounter;
		_devModeManager = devModeManager;
		_uiSettings = uiSettings;
	}

	public void Load()
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/FramesPerSecondPanel");
		_fps = visualElement.Q<Label>("FPS");
		_uiLayout.AddBottomRight(visualElement, 2);
	}

	public void UpdateSingleton()
	{
		bool flag = _devModeManager.Enabled || _uiSettings.ShowFps;
		if (flag && ValuesUpdated())
		{
			_fps.text = $"FPS: {_lastAverageFramesPerSecond} / {_lastMinFramesPerSecond}";
		}
		_fps.ToggleDisplayStyle(flag);
	}

	private bool ValuesUpdated()
	{
		if (!UpdateAverageFramesPerSecond())
		{
			return UpdateMinFramesPerSecond();
		}
		return true;
	}

	private bool UpdateAverageFramesPerSecond()
	{
		int num = Mathf.RoundToInt(_framesPerSecondCounter.AverageFramesPerSecond);
		if (num != _lastAverageFramesPerSecond)
		{
			_lastAverageFramesPerSecond = num;
			return true;
		}
		return false;
	}

	private bool UpdateMinFramesPerSecond()
	{
		int num = Mathf.RoundToInt(_framesPerSecondCounter.MinFramesPerSecond);
		if (num != _lastMinFramesPerSecond)
		{
			_lastMinFramesPerSecond = num;
			return true;
		}
		return false;
	}
}
