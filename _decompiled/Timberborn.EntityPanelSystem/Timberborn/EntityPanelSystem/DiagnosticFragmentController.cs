using System.Collections.Generic;
using Timberborn.CoreUI;
using Timberborn.Debugging;
using Timberborn.EntitySystem;
using Timberborn.SingletonSystem;
using UnityEngine.UIElements;

namespace Timberborn.EntityPanelSystem;

internal class DiagnosticFragmentController
{
	private readonly DevModeManager _devModeManager;

	private readonly EventBus _eventBus;

	private readonly List<IEntityPanelFragment> _diagnosticFragments = new List<IEntityPanelFragment>();

	private VisualElement _root;

	private EntityComponent _shownEntity;

	public DiagnosticFragmentController(DevModeManager devModeManager, EventBus eventBus)
	{
		_devModeManager = devModeManager;
		_eventBus = eventBus;
	}

	public void Initialize(IEnumerable<IEntityPanelFragment> fragments, VisualElement parent)
	{
		_root = parent.Q<VisualElement>("DiagnosticFragments");
		_root.ToggleDisplayStyle(_devModeManager.Enabled);
		foreach (IEntityPanelFragment fragment in fragments)
		{
			_root.Add(fragment.InitializeFragment());
			_diagnosticFragments.Add(fragment);
		}
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnDevModeToggled(DevModeToggledEvent devModeToggledEvent)
	{
		if (devModeToggledEvent.Enabled)
		{
			if ((bool)_shownEntity)
			{
				ShowFragments(_shownEntity);
			}
		}
		else
		{
			ClearFragmentsInternal();
		}
	}

	public void ShowFragments(EntityComponent entity)
	{
		if (_devModeManager.Enabled)
		{
			_root.ToggleDisplayStyle(visible: true);
			foreach (IEntityPanelFragment diagnosticFragment in _diagnosticFragments)
			{
				diagnosticFragment.ShowFragment(entity);
			}
		}
		_shownEntity = entity;
	}

	public void ClearFragments()
	{
		ClearFragmentsInternal();
		_shownEntity = null;
	}

	public void UpdateFragments()
	{
		if (!_devModeManager.Enabled)
		{
			return;
		}
		foreach (IEntityPanelFragment diagnosticFragment in _diagnosticFragments)
		{
			diagnosticFragment.UpdateFragment();
		}
	}

	private void ClearFragmentsInternal()
	{
		foreach (IEntityPanelFragment diagnosticFragment in _diagnosticFragments)
		{
			diagnosticFragment.ClearFragment();
		}
		_root.ToggleDisplayStyle(visible: false);
	}
}
