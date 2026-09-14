using Timberborn.CoreUI;
using Timberborn.InputSystem;
using Timberborn.InputSystemUI;
using Timberborn.MapEditorSimulationSystem;
using Timberborn.SingletonSystem;
using Timberborn.TimeSpeedButtonSystem;
using Timberborn.TooltipSystem;
using Timberborn.UILayoutSystem;
using UnityEngine.UIElements;

namespace Timberborn.MapEditorSimulationSystemUI;

public class MapEditorSimulationPanel : ILoadableSingleton, IInputProcessor
{
	private static readonly string TooltipLocKey = "MapEditor.SimulationControls.Tooltip";

	private static readonly string ResetLocKey = "MapEditor.SimulationControls.Reset";

	private static readonly string ResetMapEditorSimulationKey = "ResetMapEditorSimulation";

	private static readonly string MapEditorDevSimulationSpeedKey = "MapEditorDevSimulationSpeed";

	private static readonly int DevSimulationSpeed = 50;

	private readonly MapEditorSimulation _mapEditorSimulation;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly UILayout _uiLayout;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly TimeSpeedButtonGroup _timeSpeedButtonGroup;

	private readonly BindableButtonFactory _bindableButtonFactory;

	private readonly InputService _inputService;

	private int _speedBeforePause = 1;

	public MapEditorSimulationPanel(MapEditorSimulation mapEditorSimulation, VisualElementLoader visualElementLoader, UILayout uiLayout, ITooltipRegistrar tooltipRegistrar, TimeSpeedButtonGroup timeSpeedButtonGroup, BindableButtonFactory bindableButtonFactory, InputService inputService)
	{
		_mapEditorSimulation = mapEditorSimulation;
		_visualElementLoader = visualElementLoader;
		_uiLayout = uiLayout;
		_tooltipRegistrar = tooltipRegistrar;
		_timeSpeedButtonGroup = timeSpeedButtonGroup;
		_bindableButtonFactory = bindableButtonFactory;
		_inputService = inputService;
	}

	public void Load()
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("MapEditor/MapEditorSimulationPanel");
		_tooltipRegistrar.RegisterLocalizable(visualElement, TooltipLocKey);
		_timeSpeedButtonGroup.Initialize((from button2 in visualElement.Query<Button>()
			where button2.name.StartsWith("Speed")
			select button2).Build(), () => _mapEditorSimulation.SimulationSpeed, SetSpeed);
		Button button = visualElement.Q<Button>("Reset");
		_bindableButtonFactory.CreateAndBind(button, ResetMapEditorSimulationKey, ResetSimulation);
		_tooltipRegistrar.RegisterLocalizable(button, ResetLocKey);
		_inputService.AddInputProcessor(this);
		_uiLayout.AddTopRight(visualElement, 1);
	}

	public bool ProcessInput()
	{
		if (_inputService.IsKeyDown(MapEditorDevSimulationSpeedKey))
		{
			SetSpeed(DevSimulationSpeed);
			return true;
		}
		return false;
	}

	private void SetSpeed(int speed)
	{
		if (speed == 0)
		{
			int simulationSpeed = _mapEditorSimulation.SimulationSpeed;
			if (simulationSpeed == 0)
			{
				_mapEditorSimulation.SetSimulationSpeed(_speedBeforePause);
				return;
			}
			_speedBeforePause = simulationSpeed;
			_mapEditorSimulation.SetSimulationSpeed(0);
		}
		else
		{
			_mapEditorSimulation.SetSimulationSpeed(speed);
		}
	}

	private void ResetSimulation()
	{
		_mapEditorSimulation.ResetSimulation();
	}
}
