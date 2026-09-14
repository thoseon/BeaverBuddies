using System;
using Timberborn.Automation;
using Timberborn.BaseComponentSystem;
using Timberborn.DuplicationSystem;
using Timberborn.EntitySystem;
using Timberborn.Illumination;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.AutomationBuildings;

public class Lever : BaseComponent, IAwakableComponent, IAutomatorListener, IPersistentEntity, IDuplicable<Lever>, IDuplicable, IRegisteredComponent, ITransmitter
{
	private static readonly ComponentKey LeverKey = new ComponentKey("Lever");

	private static readonly PropertyKey<bool> IsOnKey = new PropertyKey<bool>("IsOn");

	private static readonly PropertyKey<bool> IsSpringReturnKey = new PropertyKey<bool>("IsSpringReturn");

	private static readonly PropertyKey<bool> IsPinnedKey = new PropertyKey<bool>("IsPinned");

	private readonly SpringReturnService _springReturnService;

	private readonly EventBus _eventBus;

	private Automator _automator;

	private bool _registeredForSpringReturn;

	private bool _isPressed;

	public bool IsOn { get; private set; }

	public bool IsSpringReturn { get; private set; }

	public bool IsPinned { get; private set; }

	public string LeverName => _automator.AutomatorName;

	public event EventHandler IsSpringReturnChanged;

	internal Lever(SpringReturnService springReturnService, EventBus eventBus)
	{
		_springReturnService = springReturnService;
		_eventBus = eventBus;
	}

	public void Awake()
	{
		_automator = GetComponent<Automator>();
		GetComponent<CustomizableIlluminator>().AppliedColorChanged += OnAppliedColorChanged;
	}

	public void Save(IEntitySaver entitySaver)
	{
		IObjectSaver component = entitySaver.GetComponent(LeverKey);
		if (IsOn)
		{
			component.Set(IsOnKey, value: true);
		}
		if (IsSpringReturn)
		{
			component.Set(IsSpringReturnKey, value: true);
		}
		if (IsPinned)
		{
			component.Set(IsPinnedKey, value: true);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(LeverKey, out var objectLoader))
		{
			if (objectLoader.Has(IsOnKey) && objectLoader.Get(IsOnKey))
			{
				SwitchOn();
			}
			IsPinned = objectLoader.Has(IsPinnedKey) && objectLoader.Get(IsPinnedKey);
			SetSpringReturn(objectLoader.Has(IsSpringReturnKey) && objectLoader.Get(IsSpringReturnKey));
		}
	}

	public void DuplicateFrom(Lever source)
	{
		IsOn = source.IsOn;
		SetSpringReturn(source.IsSpringReturn);
		SetPinned(source.IsPinned);
		UpdateOutputState();
	}

	public void Press()
	{
		if (!_isPressed)
		{
			if (IsSpringReturn)
			{
				Toggle();
			}
			_isPressed = true;
		}
	}

	public void Release()
	{
		if (!_isPressed)
		{
			return;
		}
		if (IsSpringReturn)
		{
			if (IsOn && !_registeredForSpringReturn)
			{
				SwitchOff();
			}
		}
		else
		{
			Toggle();
		}
		_isPressed = false;
	}

	public void SwitchState(bool newValue)
	{
		if (IsOn != newValue)
		{
			IsOn = newValue;
			UpdateOutputState();
		}
	}

	public void SetSpringReturn(bool value)
	{
		if (IsSpringReturn != value)
		{
			IsSpringReturn = value;
			RegisterForSpringReturn();
			IsSpringReturnChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	public void SetPinned(bool value)
	{
		if (IsPinned != value)
		{
			IsPinned = value;
			_eventBus.Post(new LeverPinnedChangedEvent());
		}
	}

	public void OnAutomatorStateChanged()
	{
		PostPinnedLeverModified();
	}

	internal void SpringReturnToOff()
	{
		if (IsSpringReturn && !_isPressed)
		{
			SwitchOff();
		}
		_registeredForSpringReturn = false;
	}

	private void OnAppliedColorChanged(object sender, EventArgs e)
	{
		PostPinnedLeverModified();
	}

	private void SwitchOn()
	{
		SwitchState(newValue: true);
	}

	private void SwitchOff()
	{
		SwitchState(newValue: false);
	}

	private void Toggle()
	{
		SwitchState(!IsOn);
	}

	private void UpdateOutputState()
	{
		_automator.SetState(IsOn);
		RegisterForSpringReturn();
	}

	private void RegisterForSpringReturn()
	{
		if (IsOn && IsSpringReturn && !_registeredForSpringReturn)
		{
			_springReturnService.Register(this);
			_registeredForSpringReturn = true;
		}
	}

	private void PostPinnedLeverModified()
	{
		if (IsPinned)
		{
			_eventBus.Post(new PinnedLeverModified(this));
		}
	}
}
