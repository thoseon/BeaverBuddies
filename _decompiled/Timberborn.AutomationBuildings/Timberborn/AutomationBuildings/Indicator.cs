using System;
using Timberborn.Automation;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.DuplicationSystem;
using Timberborn.EntitySystem;
using Timberborn.Illumination;
using Timberborn.NotificationSystem;
using Timberborn.Persistence;
using Timberborn.QuickNotificationSystem;
using Timberborn.SingletonSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.AutomationBuildings;

public class Indicator : BaseComponent, IAwakableComponent, IPersistentEntity, IPostLoadableEntity, IDuplicable<Indicator>, IDuplicable, IFinishedStateListener, IAutomatableNeeder, ITerminal, IRegisteredComponent
{
	private static readonly ComponentKey ComponentKey = new ComponentKey("Indicator");

	private static readonly PropertyKey<IndicatorPinnedMode> PinnedModeKey = new PropertyKey<IndicatorPinnedMode>("PinnedMode");

	private static readonly PropertyKey<bool> IsWarningEnabledKey = new PropertyKey<bool>("IsWarningEnabled");

	private static readonly PropertyKey<bool> IsJournalEntryEnabledKey = new PropertyKey<bool>("IsJournalEntryEnabled");

	private static readonly PropertyKey<bool> IsColorReplicationEnabledKey = new PropertyKey<bool>("IsColorReplicationEnabled");

	private readonly QuickNotificationService _quickNotificationService;

	private readonly NotificationBus _notificationBus;

	private readonly EventBus _eventBus;

	private Automator _automator;

	private Automatable _automatable;

	private CustomizableIlluminator _customizableIlluminator;

	private IlluminatorToggle _illuminatorToggle;

	private bool? _previousState;

	private CustomizableIlluminator _inputCustomizableIlluminator;

	public IndicatorPinnedMode PinnedMode { get; private set; }

	public bool IsWarningEnabled { get; private set; }

	public bool IsJournalEntryEnabled { get; private set; }

	public bool IsColorReplicationEnabled { get; private set; }

	public bool NeedsAutomatable => true;

	public string IndicatorName => _automator.AutomatorName;

	public bool State => _automatable.State == ConnectionState.On;

	public event EventHandler PinnedIndicatorModified;

	public Indicator(QuickNotificationService quickNotificationService, EventBus eventBus, NotificationBus notificationBus)
	{
		_quickNotificationService = quickNotificationService;
		_eventBus = eventBus;
		_notificationBus = notificationBus;
	}

	public void Awake()
	{
		_automator = GetComponent<Automator>();
		_automatable = GetComponent<Automatable>();
		_customizableIlluminator = GetComponent<CustomizableIlluminator>();
		_illuminatorToggle = GetComponent<Illuminator>().CreateToggle();
		GetComponent<CustomizableIlluminator>().AppliedColorChanged += OnAppliedColorChanged;
		DisableComponent();
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
		NotifyPinnedIndicatorModified();
		ResubscribeToInputColor();
		ReplicateInputColor();
		_automatable.InputReconnected += OnInputReconnected;
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
		NotifyPinnedIndicatorModified();
		_automatable.InputReconnected -= OnInputReconnected;
		UnsubscribeFromInputColor();
	}

	public void Save(IEntitySaver entitySaver)
	{
		IObjectSaver component = entitySaver.GetComponent(ComponentKey);
		if (PinnedMode != IndicatorPinnedMode.Never)
		{
			component.Set(PinnedModeKey, PinnedMode);
		}
		if (IsWarningEnabled)
		{
			component.Set(IsWarningEnabledKey, IsWarningEnabled);
		}
		if (IsJournalEntryEnabled)
		{
			component.Set(IsJournalEntryEnabledKey, IsJournalEntryEnabled);
		}
		if (IsColorReplicationEnabled)
		{
			component.Set(IsColorReplicationEnabledKey, IsColorReplicationEnabled);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(ComponentKey);
		PinnedMode = (component.Has(PinnedModeKey) ? component.Get(PinnedModeKey) : IndicatorPinnedMode.Never);
		IsWarningEnabled = component.Has(IsWarningEnabledKey) && component.Get(IsWarningEnabledKey);
		IsJournalEntryEnabled = component.Has(IsJournalEntryEnabledKey) && component.Get(IsJournalEntryEnabledKey);
		IsColorReplicationEnabled = component.Has(IsColorReplicationEnabledKey) && component.Get(IsColorReplicationEnabledKey);
	}

	public void PostLoadEntity()
	{
		if (IsColorReplicationEnabled)
		{
			UpdateReplicationState();
		}
	}

	public void Evaluate()
	{
		bool flag = _automatable.State == ConnectionState.On;
		if (_previousState != flag)
		{
			_illuminatorToggle.Toggle(flag);
			if (_previousState.HasValue & flag)
			{
				EvaluateRisingEdge();
			}
			_previousState = flag;
			NotifyPinnedIndicatorModified();
		}
	}

	public void DuplicateFrom(Indicator source)
	{
		SetPinnedMode(source.PinnedMode);
		SetWarningEnabled(source.IsWarningEnabled);
		SetJournalEntryEnabled(source.IsJournalEntryEnabled);
		SetColorReplicationEnabled(source.IsColorReplicationEnabled);
	}

	public void SetPinnedMode(IndicatorPinnedMode value)
	{
		if (PinnedMode != value)
		{
			PinnedMode = value;
			_eventBus.Post(new IndicatorPinnedModeChangedEvent());
		}
	}

	public void SetWarningEnabled(bool value)
	{
		IsWarningEnabled = value;
	}

	public void SetJournalEntryEnabled(bool value)
	{
		IsJournalEntryEnabled = value;
	}

	public void SetColorReplicationEnabled(bool value)
	{
		IsColorReplicationEnabled = value;
		UpdateReplicationState();
	}

	private void OnAppliedColorChanged(object sender, EventArgs e)
	{
		NotifyPinnedIndicatorModified();
	}

	private void EvaluateRisingEdge()
	{
		if (IsWarningEnabled)
		{
			ShowWarning();
		}
		if (IsJournalEntryEnabled)
		{
			AddJournalEntry();
		}
	}

	private void ShowWarning()
	{
		_quickNotificationService.SendWarningNotification(IndicatorName);
	}

	private void AddJournalEntry()
	{
		_notificationBus.Post(IndicatorName, this);
	}

	private void NotifyPinnedIndicatorModified()
	{
		if (PinnedMode != IndicatorPinnedMode.Never)
		{
			PinnedIndicatorModified?.Invoke(this, EventArgs.Empty);
		}
	}

	private void OnInputReconnected(object sender, EventArgs e)
	{
		ResubscribeToInputColor();
		ReplicateInputColor();
	}

	private void UpdateReplicationState()
	{
		_customizableIlluminator.SetIsCustomized(IsColorReplicationEnabled);
		if (IsColorReplicationEnabled)
		{
			_customizableIlluminator.Lock();
		}
		else
		{
			_customizableIlluminator.Unlock();
		}
		ResubscribeToInputColor();
		ReplicateInputColor();
	}

	private void ResubscribeToInputColor()
	{
		UnsubscribeFromInputColor();
		if (IsColorReplicationEnabled)
		{
			_inputCustomizableIlluminator = _automatable.Input?.GetComponent<CustomizableIlluminator>();
			if (_inputCustomizableIlluminator != null && (bool)_inputCustomizableIlluminator)
			{
				_inputCustomizableIlluminator.CustomColorChanged += OnInputCustomColorChanged;
			}
		}
	}

	private void UnsubscribeFromInputColor()
	{
		if (_inputCustomizableIlluminator != null)
		{
			_inputCustomizableIlluminator.CustomColorChanged -= OnInputCustomColorChanged;
			_inputCustomizableIlluminator = null;
		}
	}

	private void OnInputCustomColorChanged(object sender, EventArgs e)
	{
		if ((bool)_customizableIlluminator)
		{
			ReplicateInputColor();
		}
	}

	private void ReplicateInputColor()
	{
		if (_inputCustomizableIlluminator != null)
		{
			_customizableIlluminator.SetCustomColor(_inputCustomizableIlluminator.CustomColor);
		}
	}
}
