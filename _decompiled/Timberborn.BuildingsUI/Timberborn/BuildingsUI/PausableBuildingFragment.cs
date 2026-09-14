using Timberborn.BaseComponentSystem;
using Timberborn.Buildings;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.InputSystemUI;
using Timberborn.TooltipSystem;
using UnityEngine.UIElements;

namespace Timberborn.BuildingsUI;

public class PausableBuildingFragment : IEntityPanelFragment
{
	private static readonly string ToggleBuildingPauseKey = "ToggleBuildingPause";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly BindableToggleFactory _bindableToggleFactory;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private PausableBuilding _pausableBuilding;

	private VisualElement _root;

	private BindableToggle _pauseToggle;

	public PausableBuildingFragment(VisualElementLoader visualElementLoader, BindableToggleFactory bindableToggleFactory, ITooltipRegistrar tooltipRegistrar)
	{
		_visualElementLoader = visualElementLoader;
		_bindableToggleFactory = bindableToggleFactory;
		_tooltipRegistrar = tooltipRegistrar;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/PausableBuildingFragment");
		Toggle toggle = _root.Q<Toggle>("Toggle");
		_pauseToggle = _bindableToggleFactory.Create(toggle, ToggleBuildingPauseKey, ToggleActivationState, () => !_pausableBuilding.Paused);
		_root.ToggleDisplayStyle(visible: false);
		_tooltipRegistrar.RegisterWithKeyBinding(toggle, ToggleBuildingPauseKey);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_pausableBuilding = entity.GetComponent<PausableBuilding>();
		if ((bool)_pausableBuilding)
		{
			_pauseToggle.Bind();
		}
	}

	public void ClearFragment()
	{
		if ((bool)_pausableBuilding)
		{
			ToggleHighlight(state: false);
		}
		_pausableBuilding = null;
		_root.ToggleDisplayStyle(visible: false);
		_pauseToggle.Unbind();
	}

	public void UpdateFragment()
	{
		if ((bool)_pausableBuilding && _pausableBuilding.IsPausable())
		{
			_root.ToggleDisplayStyle(visible: true);
			_pauseToggle.Enable();
		}
		else
		{
			_root.ToggleDisplayStyle(visible: false);
			_pauseToggle.Disable();
		}
	}

	public void ToggleHighlight(bool state)
	{
		_root.EnableInClassList("highlight", state);
	}

	private void ToggleActivationState(bool resume)
	{
		if ((bool)_pausableBuilding && _pausableBuilding.IsPausable())
		{
			if (resume)
			{
				_pausableBuilding.Resume();
			}
			else
			{
				_pausableBuilding.Pause();
			}
		}
	}
}
