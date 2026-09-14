using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.Debugging;
using Timberborn.EntityPanelSystem;
using Timberborn.EntityUndoSystem;
using Timberborn.Explosions;
using Timberborn.MapStateSystem;
using Timberborn.SingletonSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.ExplosionsUI;

internal class UnstableCoreFragment : IEntityPanelFragment
{
	private readonly EntityChangeRecorderFactory _entityChangeRecorderFactory;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly DevModeManager _devModeManager;

	private readonly MapEditorMode _mapEditorMode;

	private readonly EventBus _eventBus;

	private VisualElement _root;

	private UnstableCore _unstableCore;

	private IntegerField _explosionRadiusInput;

	private bool Visible
	{
		get
		{
			if (!_devModeManager.Enabled)
			{
				return _mapEditorMode.IsMapEditor;
			}
			return true;
		}
	}

	public UnstableCoreFragment(EntityChangeRecorderFactory entityChangeRecorderFactory, VisualElementLoader visualElementLoader, DevModeManager devModeManager, MapEditorMode mapEditorMode, EventBus eventBus)
	{
		_entityChangeRecorderFactory = entityChangeRecorderFactory;
		_visualElementLoader = visualElementLoader;
		_devModeManager = devModeManager;
		_mapEditorMode = mapEditorMode;
		_eventBus = eventBus;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Common/EntityPanel/UnstableCoreFragment");
		_explosionRadiusInput = _root.Q<IntegerField>("ExplosionRadiusInput");
		_explosionRadiusInput.RegisterValueChangedCallback(OnRadiusChanged);
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_unstableCore = entity.GetComponent<UnstableCore>();
		if ((bool)_unstableCore && Visible)
		{
			_explosionRadiusInput.SetValueWithoutNotify(_unstableCore.ExplosionRadius);
			_root.ToggleDisplayStyle(visible: true);
		}
		_eventBus.Register(this);
	}

	public void ClearFragment()
	{
		_root.ToggleDisplayStyle(visible: false);
		_unstableCore = null;
		_eventBus.Unregister((object)this);
	}

	public void UpdateFragment()
	{
	}

	[OnEvent]
	public void OnDevModeToggled(DevModeToggledEvent devModeToggledEvent)
	{
		if ((bool)_unstableCore)
		{
			_root.ToggleDisplayStyle(Visible);
			if (Visible)
			{
				_explosionRadiusInput.SetValueWithoutNotify(_unstableCore.ExplosionRadius);
			}
		}
	}

	private void OnRadiusChanged(ChangeEvent<int> evt)
	{
		using (_entityChangeRecorderFactory.CreateChangeRecorder(_unstableCore))
		{
			int radius = Mathf.Clamp(evt.newValue, _unstableCore.MinExplosionRadius, _unstableCore.MaxExplosionRadius);
			_unstableCore.SetRadius(radius);
		}
	}
}
