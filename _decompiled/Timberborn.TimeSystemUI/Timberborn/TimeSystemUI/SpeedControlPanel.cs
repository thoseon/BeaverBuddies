using Timberborn.CoreUI;
using Timberborn.Debugging;
using Timberborn.InputSystem;
using Timberborn.SingletonSystem;
using Timberborn.TickSystem;
using Timberborn.TimeSpeedButtonSystem;
using Timberborn.TimeSystem;
using Timberborn.TooltipSystem;
using Timberborn.UILayoutSystem;
using UnityEngine.UIElements;

namespace Timberborn.TimeSystemUI;

public class SpeedControlPanel : ILoadableSingleton, IDevModule, IInputProcessor
{
	private static readonly float SlowDevGameSpeed = 0.25f;

	private static readonly float FastDevGameSpeed = 30f;

	private static readonly float SuperFastDevGameSpeed = 99f;

	private static readonly string TickOnceKey = "TickOnce";

	private static readonly string SlowDevGameSpeedKey = "SlowDevGameSpeed";

	private static readonly string FastDevGameSpeedKey = "FastDevGameSpeed";

	private static readonly string SuperFastDevGameSpeedKey = "SuperFastDevGameSpeed";

	private readonly UILayout _uiLayout;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly SpeedManager _speedManager;

	private readonly Ticker _ticker;

	private readonly InputService _inputService;

	private readonly TimeSpeedButtonGroup _timeSpeedButtonGroup;

	private readonly EventBus _eventBus;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private float _speedBeforePause = 1f;

	private bool _pauseNextTick;

	private VisualElement _root;

	public SpeedControlPanel(UILayout uiLayout, VisualElementLoader visualElementLoader, SpeedManager speedManager, Ticker ticker, InputService inputService, TimeSpeedButtonGroup timeSpeedButtonGroup, EventBus eventBus, ITooltipRegistrar tooltipRegistrar)
	{
		_uiLayout = uiLayout;
		_visualElementLoader = visualElementLoader;
		_speedManager = speedManager;
		_ticker = ticker;
		_inputService = inputService;
		_timeSpeedButtonGroup = timeSpeedButtonGroup;
		_eventBus = eventBus;
		_tooltipRegistrar = tooltipRegistrar;
	}

	public DevModuleDefinition GetDefinition()
	{
		return new DevModuleDefinition.Builder().AddMethod(DevMethod.CreateBindable("Speed: x0.25", SlowDevGameSpeedKey, delegate
		{
			SetSpeed(SlowDevGameSpeed);
		})).AddMethod(DevMethod.CreateBindable("Speed: x30", FastDevGameSpeedKey, delegate
		{
			SetSpeed(FastDevGameSpeed);
		})).AddMethod(DevMethod.CreateBindable("Speed: x99", SuperFastDevGameSpeedKey, delegate
		{
			SetSpeed(SuperFastDevGameSpeed);
		}))
			.Build();
	}

	public void Load()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/SpeedControlPanel");
		_eventBus.Register(this);
		_tooltipRegistrar.RegisterWithKeyBinding(_root.Q<Button>("Speed0"), "Speed0");
		_tooltipRegistrar.RegisterWithKeyBinding(_root.Q<Button>("Speed1"), "Speed1");
		_tooltipRegistrar.RegisterWithKeyBinding(_root.Q<Button>("Speed3"), "Speed2");
		_tooltipRegistrar.RegisterWithKeyBinding(_root.Q<Button>("Speed7"), "Speed3");
	}

	public bool ProcessInput()
	{
		if (_inputService.IsKeyDown(TickOnceKey))
		{
			PauseOrTickOnce();
		}
		else if (_inputService.IsKeyDown(SlowDevGameSpeedKey))
		{
			SetSpeed(SlowDevGameSpeed);
		}
		else if (_inputService.IsKeyDown(FastDevGameSpeedKey))
		{
			SetSpeed(FastDevGameSpeed);
		}
		else if (_inputService.IsKeyDown(SuperFastDevGameSpeedKey))
		{
			SetSpeed(SuperFastDevGameSpeed);
		}
		return false;
	}

	[OnEvent]
	public void OnShowPrimaryUI(ShowPrimaryUIEvent showPrimaryUIEvent)
	{
		_timeSpeedButtonGroup.Initialize(_root.Query<Button>().Build(), () => _speedManager.CurrentSpeed, delegate(int speed)
		{
			SetSpeed(speed);
		});
		_inputService.AddInputProcessor(this);
		_uiLayout.AddTopRight(_root, 2);
	}

	private void PauseOrTickOnce()
	{
		if (_speedManager.CurrentSpeed == 0f)
		{
			_ticker.TickOnce();
		}
		else
		{
			SetSpeed(0f);
		}
	}

	private void SetSpeed(float timeSpeed)
	{
		if (timeSpeed == 0f)
		{
			float currentSpeed = _speedManager.CurrentSpeed;
			if (currentSpeed == 0f)
			{
				_speedManager.ChangeSpeed(_speedBeforePause);
				return;
			}
			_speedBeforePause = currentSpeed;
			_speedManager.ChangeSpeed(0f);
		}
		else
		{
			_speedManager.ChangeSpeed(timeSpeed);
		}
	}
}
