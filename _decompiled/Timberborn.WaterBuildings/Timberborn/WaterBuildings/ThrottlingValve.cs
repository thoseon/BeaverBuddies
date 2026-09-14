using System;
using Timberborn.Automation;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.DuplicationSystem;
using Timberborn.Persistence;
using Timberborn.TickSystem;
using Timberborn.WaterSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.WaterBuildings;

public class ThrottlingValve : TickableComponent, IAwakableComponent, IFinishedStateListener, IUnfinishedStateListener, IPersistentEntity, IDuplicable<ThrottlingValve>, IDuplicable, ITerminal
{
	public static readonly float ReactionSpeedMin = 0.01f;

	public static readonly float ReactionSpeedMax = 1f;

	private static readonly ComponentKey ComponentKey = new ComponentKey("ThrottlingValve");

	private static readonly PropertyKey<bool> IsSynchronizedKey = new PropertyKey<bool>("IsSynchronized");

	private static readonly PropertyKey<bool> OutflowLimitEnabledKey = new PropertyKey<bool>("OutflowLimitEnabled");

	private static readonly PropertyKey<float> OutflowLimitKey = new PropertyKey<float>("OutflowLimit");

	private static readonly PropertyKey<bool> AutomationOutflowLimitEnabledKey = new PropertyKey<bool>("AutomationOutflowLimitEnabled");

	private static readonly PropertyKey<float> AutomationOutflowLimitKey = new PropertyKey<float>("AutomationOutflowLimit");

	private static readonly PropertyKey<float> ReactionSpeedKey = new PropertyKey<float>("ReactionSpeed");

	private static readonly PropertyKey<float> CurrentOutflowLimitKey = new PropertyKey<float>("CurrentOutflowLimit");

	private static readonly PropertyKey<int> LastSignKey = new PropertyKey<int>("LastSign");

	private static readonly PropertyKey<int> TicksWithCurrentSignKey = new PropertyKey<int>("TicksWithCurrentSign");

	private readonly IWaterService _waterService;

	private readonly ThrottlingValveSynchronizer _throttlingValveSynchronizer;

	private BlockObject _blockObject;

	private Automatable _automatable;

	private ThrottlingValveSpec _throttlingValveSpec;

	private WaterObstacleController _waterObstacleController;

	private int _lastSign;

	private int _ticksWithCurrentSign;

	private bool _isDuplicated;

	public bool IsSynchronized { get; private set; } = true;

	public bool OutflowLimitEnabled { get; private set; }

	public float OutflowLimit { get; private set; }

	public bool AutomationOutflowLimitEnabled { get; private set; }

	public float AutomationOutflowLimit { get; private set; }

	public float ReactionSpeed { get; private set; }

	public float? CurrentOutflowLimit { get; private set; }

	public float MaxOutflowLimit => _throttlingValveSpec.MaxOutflowLimit;

	public float OutflowLimitStep => _throttlingValveSpec.OutflowLimitStep;

	public float ReactionSpeedStep => _throttlingValveSpec.ReactionSpeedStep;

	public bool IsAutomated => _automatable.IsAutomated;

	public bool IsInputOn => _automatable.State == ConnectionState.On;

	public ThrottlingValveState? State
	{
		get
		{
			if (base.Enabled && _automatable.IsAutomated)
			{
				float num = GetTargetOutflowLimit() ?? float.PositiveInfinity;
				float num2 = CurrentOutflowLimit ?? float.PositiveInfinity;
				if (num.Equals(num2))
				{
					return ThrottlingValveState.Idle;
				}
				if (num > num2)
				{
					return ThrottlingValveState.Opening;
				}
				if (num < num2)
				{
					return ThrottlingValveState.Closing;
				}
			}
			return null;
		}
	}

	private float EffectiveReactionSpeed
	{
		get
		{
			if (!_automatable.IsAutomated)
			{
				return 1f;
			}
			return ReactionSpeed;
		}
	}

	internal ThrottlingValve(IWaterService waterService, ThrottlingValveSynchronizer throttlingValveSynchronizer)
	{
		_waterService = waterService;
		_throttlingValveSynchronizer = throttlingValveSynchronizer;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_automatable = GetComponent<Automatable>();
		_throttlingValveSpec = GetComponent<ThrottlingValveSpec>();
		_waterObstacleController = GetComponent<WaterObstacleController>();
		OutflowLimitEnabled = _throttlingValveSpec.DefaultOutflowLimitEnabled;
		OutflowLimit = _throttlingValveSpec.DefaultOutflowLimit;
		AutomationOutflowLimitEnabled = _throttlingValveSpec.DefaultAutomationOutflowLimitEnabled;
		AutomationOutflowLimit = _throttlingValveSpec.DefaultAutomationOutflowLimit;
		ReactionSpeed = 1f;
		DisableComponent();
		_automatable.InputReconnected += OnAutomatableInputReconnected;
	}

	public void Save(IEntitySaver entitySaver)
	{
		IObjectSaver component = entitySaver.GetComponent(ComponentKey);
		component.Set(IsSynchronizedKey, IsSynchronized);
		component.Set(OutflowLimitEnabledKey, OutflowLimitEnabled);
		component.Set(OutflowLimitKey, OutflowLimit);
		component.Set(AutomationOutflowLimitEnabledKey, AutomationOutflowLimitEnabled);
		component.Set(AutomationOutflowLimitKey, AutomationOutflowLimit);
		if (CurrentOutflowLimit.HasValue)
		{
			component.Set(CurrentOutflowLimitKey, CurrentOutflowLimit.Value);
		}
		component.Set(ReactionSpeedKey, ReactionSpeed);
		component.Set(LastSignKey, _lastSign);
		component.Set(TicksWithCurrentSignKey, _ticksWithCurrentSign);
	}

	[BackwardCompatible(2026, 4, 16, Compatibility.Save)]
	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader objectLoader = (entityLoader.HasComponent(ComponentKey) ? entityLoader.GetComponent(ComponentKey) : entityLoader.GetComponent(new ComponentKey("Valve")));
		IsSynchronized = objectLoader.Get(IsSynchronizedKey);
		SetOutflowLimitEnabled(objectLoader.Has(OutflowLimitEnabledKey) && objectLoader.Get(OutflowLimitEnabledKey));
		SetOutflowLimit(objectLoader.Has(OutflowLimitKey) ? objectLoader.Get(OutflowLimitKey) : 0f);
		SetAutomationOutflowLimitEnabled(objectLoader.Has(AutomationOutflowLimitEnabledKey) && objectLoader.Get(AutomationOutflowLimitEnabledKey));
		SetAutomationOutflowLimit(objectLoader.Has(AutomationOutflowLimitKey) ? objectLoader.Get(AutomationOutflowLimitKey) : 0f);
		SetReactionSpeed(objectLoader.Has(ReactionSpeedKey) ? objectLoader.Get(ReactionSpeedKey) : ReactionSpeedMax);
		CurrentOutflowLimit = (objectLoader.Has(CurrentOutflowLimitKey) ? new float?(objectLoader.Get(CurrentOutflowLimitKey)) : ((float?)null));
		_lastSign = (objectLoader.Has(LastSignKey) ? objectLoader.Get(LastSignKey) : 0);
		_ticksWithCurrentSign = (objectLoader.Has(TicksWithCurrentSignKey) ? objectLoader.Get(TicksWithCurrentSignKey) : 0);
	}

	public void DuplicateFrom(ThrottlingValve source)
	{
		IsSynchronized = source.IsSynchronized;
		SetOutflowLimit(source.OutflowLimit);
		SetOutflowLimitEnabled(source.OutflowLimitEnabled);
		SetAutomationOutflowLimit(source.AutomationOutflowLimit);
		SetAutomationOutflowLimitEnabled(source.AutomationOutflowLimitEnabled);
		SetReactionSpeed(source.ReactionSpeed);
		_isDuplicated = true;
	}

	public void OnEnterUnfinishedState()
	{
		if (_isDuplicated)
		{
			SynchronizeNeighbors();
		}
		else
		{
			_throttlingValveSynchronizer.SynchronizeWithUnfinishedNeighbors(this);
		}
	}

	public void OnExitUnfinishedState()
	{
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
		_waterService.AddDirectionLimiter(_blockObject.Coordinates, _blockObject.Orientation.ToFlowDirection());
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
		ClearLimit();
		_waterService.RemoveDirectionLimiter(_blockObject.Coordinates);
	}

	public override void Tick()
	{
		TickCurrentOutflowLimit();
		ApplyCurrentOutflowLimit();
	}

	public void SetOutflowLimitEnabledAndSynchronize(bool value)
	{
		SetOutflowLimitEnabled(value);
		SynchronizeNeighbors();
	}

	public void SetOutflowLimitEnabled(bool value)
	{
		OutflowLimitEnabled = value;
	}

	public void SetOutflowLimitAndSynchronize(float value)
	{
		SetOutflowLimit(value);
		SynchronizeNeighbors();
	}

	public void SetOutflowLimit(float value)
	{
		OutflowLimit = value;
	}

	public void SetAutomationOutflowLimitEnabledAndSynchronize(bool value)
	{
		SetAutomationOutflowLimitEnabled(value);
		SynchronizeNeighbors();
	}

	public void SetAutomationOutflowLimitEnabled(bool value)
	{
		AutomationOutflowLimitEnabled = value;
	}

	public void SetAutomationOutflowLimitAndSynchronize(float value)
	{
		SetAutomationOutflowLimit(value);
		SynchronizeNeighbors();
	}

	public void SetAutomationOutflowLimit(float value)
	{
		AutomationOutflowLimit = value;
	}

	public void SetReactionSpeedAndSynchronize(float value)
	{
		SetReactionSpeed(value);
		SynchronizeNeighbors();
	}

	public void SetReactionSpeed(float value)
	{
		ReactionSpeed = Mathf.Clamp(value, ReactionSpeedMin, ReactionSpeedMax);
	}

	public void ToggleSynchronization(bool value)
	{
		IsSynchronized = value;
		_throttlingValveSynchronizer.SynchronizeWithAllNeighbors(this);
	}

	public void Evaluate()
	{
	}

	private void OnAutomatableInputReconnected(object sender, EventArgs e)
	{
		if (IsSynchronized)
		{
			SynchronizeNeighbors();
		}
	}

	private void TickCurrentOutflowLimit()
	{
		float? targetOutflowLimit = GetTargetOutflowLimit();
		UpdateTicksWithCurrentSign(targetOutflowLimit);
		if (!CurrentOutflowLimit.Equals(targetOutflowLimit))
		{
			float num = targetOutflowLimit ?? float.PositiveInfinity;
			float? currentOutflowLimit = CurrentOutflowLimit;
			float valueOrDefault = currentOutflowLimit.GetValueOrDefault();
			if (!currentOutflowLimit.HasValue)
			{
				valueOrDefault = MaxOutflowLimit;
				float? currentOutflowLimit2 = valueOrDefault;
				CurrentOutflowLimit = currentOutflowLimit2;
			}
			if (CurrentOutflowLimit < num)
			{
				CurrentOutflowLimit = Mathf.Min(CurrentOutflowLimit.Value + RateOfChange(), num);
			}
			else if (CurrentOutflowLimit > num)
			{
				CurrentOutflowLimit = Mathf.Max(CurrentOutflowLimit.Value - RateOfChange(), num);
			}
			if (CurrentOutflowLimit > MaxOutflowLimit)
			{
				CurrentOutflowLimit = null;
			}
		}
	}

	private void ClearLimit()
	{
		CurrentOutflowLimit = null;
		ApplyCurrentOutflowLimit();
	}

	private void ApplyCurrentOutflowLimit()
	{
		_waterObstacleController.UpdateState(CurrentOutflowLimit == 0f);
		if (CurrentOutflowLimit.HasValue)
		{
			_waterService.SetInflowLimit(_blockObject.Coordinates, CurrentOutflowLimit.Value);
		}
		else
		{
			_waterService.RemoveInflowLimit(_blockObject.Coordinates);
		}
	}

	private float? GetTargetOutflowLimit()
	{
		if (base.Enabled)
		{
			if (_automatable.IsAutomated && _automatable.State == ConnectionState.On)
			{
				if (!AutomationOutflowLimitEnabled)
				{
					return null;
				}
				return Mathf.Min(AutomationOutflowLimit, MaxOutflowLimit);
			}
			if (!OutflowLimitEnabled)
			{
				return null;
			}
			return Mathf.Min(OutflowLimit, MaxOutflowLimit);
		}
		return null;
	}

	private void UpdateTicksWithCurrentSign(float? targetOutflowLimit)
	{
		float num = targetOutflowLimit ?? float.PositiveInfinity;
		int num2 = (CurrentOutflowLimit.HasValue ? Math.Sign(num - CurrentOutflowLimit.Value) : (targetOutflowLimit.HasValue ? (-1) : 0));
		if (num2 != _lastSign)
		{
			_lastSign = num2;
			_ticksWithCurrentSign = 0;
		}
		else
		{
			_ticksWithCurrentSign++;
		}
	}

	private float RateOfChange()
	{
		float t = Mathf.Pow(EffectiveReactionSpeed, _throttlingValveSpec.ReactionSpeedExponent);
		float a = Mathf.Lerp(_throttlingValveSpec.RateOfChangeLowPrimary, _throttlingValveSpec.RateOfChangeHighPrimary, t);
		float b = Mathf.Lerp(_throttlingValveSpec.RateOfChangeLowSecondary, _throttlingValveSpec.RateOfChangeHighSecondary, t);
		return Mathf.Lerp(a, b, (float)(_ticksWithCurrentSign - _throttlingValveSpec.RateOfChangePrimaryTicks) / (float)_throttlingValveSpec.RateOfChangePrimaryToSecondaryTicks);
	}

	private void SynchronizeNeighbors()
	{
		_throttlingValveSynchronizer.SynchronizeAllNeighbors(this);
	}
}
