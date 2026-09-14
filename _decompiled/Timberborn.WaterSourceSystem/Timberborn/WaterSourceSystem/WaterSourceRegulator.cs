using System;
using Timberborn.Automation;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.DuplicationSystem;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;

namespace Timberborn.WaterSourceSystem;

public class WaterSourceRegulator : BaseComponent, IAwakableComponent, IPersistentEntity, IDuplicable<WaterSourceRegulator>, IDuplicable, IWaterStrengthModifier, IFinishedStateListener, IAutomatableNeeder, ITerminal
{
	private enum RegulatorState
	{
		Open,
		Closed,
		Automated
	}

	private static readonly ComponentKey WaterSourceRegulatorKey = new ComponentKey("WaterSourceRegulator");

	private static readonly PropertyKey<RegulatorState> RegulatorStateKey = new PropertyKey<RegulatorState>("RegulatorState");

	private UnderlyingWaterSource _underlyingWaterSource;

	private Automatable _automatable;

	private RegulatorState _regulatorState = RegulatorState.Closed;

	public bool IsOpen { get; private set; }

	public bool OpenMode => _regulatorState == RegulatorState.Open;

	public bool ClosedMode => _regulatorState == RegulatorState.Closed;

	public bool AutomatedMode => _regulatorState == RegulatorState.Automated;

	public bool NeedsAutomatable => _regulatorState == RegulatorState.Automated;

	public event EventHandler<bool> OpenStateChanged;

	public void Awake()
	{
		_underlyingWaterSource = GetComponent<UnderlyingWaterSource>();
		_automatable = GetComponent<Automatable>();
		DisableComponent();
	}

	public void Save(IEntitySaver entitySaver)
	{
		entitySaver.GetComponent(WaterSourceRegulatorKey).Set(RegulatorStateKey, _regulatorState);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(WaterSourceRegulatorKey);
		LoadOpenStateFromComponent(component);
	}

	public void DuplicateFrom(WaterSourceRegulator source)
	{
		SetRegulatorState(source._regulatorState);
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
		UpdateModifierState();
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
		_underlyingWaterSource.RemoveWaterStrengthModifier(this);
	}

	public void Open()
	{
		SetRegulatorState(RegulatorState.Open);
	}

	public void Close()
	{
		SetRegulatorState(RegulatorState.Closed);
	}

	public void Automate()
	{
		SetRegulatorState(RegulatorState.Automated);
	}

	public float GetStrengthModifier()
	{
		return 0f;
	}

	public void Evaluate()
	{
		if (_regulatorState == RegulatorState.Automated)
		{
			UpdateOpenState();
		}
	}

	[BackwardCompatible(2025, 12, 19, Compatibility.Save)]
	private void LoadOpenStateFromComponent(IObjectLoader component)
	{
		if (component.Has(RegulatorStateKey))
		{
			_regulatorState = component.Get(RegulatorStateKey);
		}
		else
		{
			PropertyKey<bool> key = new PropertyKey<bool>("IsOpen");
			if (component.Has(key) && component.Get(key))
			{
				_regulatorState = RegulatorState.Open;
			}
		}
		UpdateOpenState();
	}

	private void SetRegulatorState(RegulatorState regulatorState)
	{
		if (_regulatorState != regulatorState)
		{
			_regulatorState = regulatorState;
			UpdateOpenState();
		}
	}

	private void UpdateOpenState()
	{
		bool flag = _regulatorState == RegulatorState.Open || (_regulatorState == RegulatorState.Automated && _automatable.State == ConnectionState.On);
		if (IsOpen != flag)
		{
			IsOpen = flag;
			UpdateModifierState();
			OpenStateChanged?.Invoke(this, IsOpen);
		}
	}

	private void UpdateModifierState()
	{
		if (base.Enabled)
		{
			if (IsOpen)
			{
				_underlyingWaterSource.RemoveWaterStrengthModifier(this);
			}
			else
			{
				_underlyingWaterSource.AddWaterStrengthModifier(this);
			}
		}
	}
}
