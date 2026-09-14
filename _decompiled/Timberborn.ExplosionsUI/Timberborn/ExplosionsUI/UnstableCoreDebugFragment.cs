using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.EntitySystem;
using Timberborn.Explosions;
using Timberborn.InputSystem;
using UnityEngine.UIElements;

namespace Timberborn.ExplosionsUI;

internal class UnstableCoreDebugFragment : IEntityPanelFragment, IInputProcessor
{
	private static readonly string DetonationDelayKey = "DetonationDelay";

	private static readonly string LongDetonationDelayKey = "LongDetonationDelay";

	private static readonly string DetonateKey = "DetonateUnstableCore";

	private readonly DebugFragmentFactory _debugFragmentFactory;

	private readonly InputService _inputService;

	private readonly EntityService _entityService;

	private VisualElement _root;

	private UnstableCore _unstableCore;

	private UnstableCoreExplosionBlocker _unstableCoreExplosionBlocker;

	public UnstableCoreDebugFragment(DebugFragmentFactory debugFragmentFactory, InputService inputService, EntityService entityService)
	{
		_debugFragmentFactory = debugFragmentFactory;
		_inputService = inputService;
		_entityService = entityService;
	}

	public VisualElement InitializeFragment()
	{
		_root = _debugFragmentFactory.Create(new DebugFragmentButton(OnExplodeClicked, "Explode"), new DebugFragmentButton(OnRemoveButtonClicked, "Delete without exploding"));
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_unstableCore = entity.GetComponent<UnstableCore>();
		if ((bool)_unstableCore)
		{
			_unstableCoreExplosionBlocker = _unstableCore.GetComponent<UnstableCoreExplosionBlocker>();
			_inputService.AddInputProcessor(this);
			_root.ToggleDisplayStyle(visible: true);
		}
	}

	public void UpdateFragment()
	{
	}

	public void ClearFragment()
	{
		if ((bool)_unstableCore)
		{
			_inputService.RemoveInputProcessor(this);
			_unstableCore = null;
			_unstableCoreExplosionBlocker = null;
			_root.ToggleDisplayStyle(visible: false);
		}
	}

	public bool ProcessInput()
	{
		if (_inputService.IsKeyDown(DetonateKey))
		{
			DetonateSelected();
			return true;
		}
		return false;
	}

	private void OnExplodeClicked()
	{
		DetonateSelected();
	}

	private void OnRemoveButtonClicked()
	{
		_unstableCoreExplosionBlocker.BlockExplosion();
		_entityService.Delete(_unstableCore);
	}

	private void DetonateSelected()
	{
		_unstableCoreExplosionBlocker.Disable();
		if (_inputService.IsKeyHeld(DetonationDelayKey))
		{
			_unstableCore.ActivateDelayed(10f);
		}
		else if (_inputService.IsKeyHeld(LongDetonationDelayKey))
		{
			_unstableCore.ActivateDelayed(20f);
		}
		else
		{
			_unstableCore.Activate();
		}
	}
}
