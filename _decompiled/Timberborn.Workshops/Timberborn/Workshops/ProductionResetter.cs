using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.Localization;
using Timberborn.Persistence;
using Timberborn.StatusSystem;
using Timberborn.TickSystem;
using Timberborn.TimeSystem;
using Timberborn.WorkSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.Workshops;

internal class ProductionResetter : TickableComponent, IAwakableComponent, IInitializableEntity, IPersistentEntity
{
	private static readonly ComponentKey ProductionResetterKey = new ComponentKey("ProductionResetter");

	private static readonly PropertyKey<float> ResetTimerKey = new PropertyKey<float>("ResetTimer");

	private static readonly PropertyKey<bool> DeactivationTriggeredKey = new PropertyKey<bool>("DeactivationTriggered");

	private readonly IDayNightCycle _dayNightCycle;

	private readonly ILoc _loc;

	private Workplace _workplace;

	private ProductionResetterSpec _productionResetterSpec;

	private Manufactory _manufactory;

	private StatusToggle _productionStoppedStatus;

	private float _resetTimer;

	private bool _deactivationTriggered;

	public ProductionResetter(IDayNightCycle dayNightCycle, ILoc loc)
	{
		_dayNightCycle = dayNightCycle;
		_loc = loc;
	}

	public void Awake()
	{
		_workplace = GetComponent<Workplace>();
		_productionResetterSpec = GetComponent<ProductionResetterSpec>();
		_manufactory = GetComponent<Manufactory>();
		_manufactory.ProductionProgressed += OnProductionProgressed;
		_productionStoppedStatus = StatusToggle.CreatePriorityStatusWithAlertAndFloatingIcon(_productionResetterSpec.StatusIcon, _loc.T(_productionResetterSpec.StatusLocKey), _loc.T(_productionResetterSpec.AlertLocKey));
	}

	public void InitializeEntity()
	{
		GetComponent<StatusSubject>().RegisterStatus(_productionStoppedStatus);
		UpdateStatus();
	}

	public override void Tick()
	{
		EvaluateTriggerConditions();
		UpdateTimer();
	}

	public void Save(IEntitySaver entitySaver)
	{
		IObjectSaver component = entitySaver.GetComponent(ProductionResetterKey);
		component.Set(ResetTimerKey, _resetTimer);
		component.Set(DeactivationTriggeredKey, _deactivationTriggered);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(ProductionResetterKey);
		_resetTimer = component.Get(ResetTimerKey);
		_deactivationTriggered = component.Get(DeactivationTriggeredKey);
	}

	private void OnProductionProgressed(object sender, ProductionProgressedEventArgs e)
	{
		if (e.ProductionProgressChange > 0f)
		{
			_deactivationTriggered = true;
		}
	}

	private void UpdateStatus()
	{
		if (_resetTimer > 0f)
		{
			_productionStoppedStatus.Activate();
		}
		else
		{
			_productionStoppedStatus.Deactivate();
		}
	}

	private void EvaluateTriggerConditions()
	{
		bool flag = (bool)_workplace && _workplace.NumberOfAssignedWorkers == 0;
		bool flag2 = _manufactory.ProductionProgress > 0f && (!_manufactory.IsReadyToProduce | flag);
		if (flag2 && _resetTimer == 0f)
		{
			ActivateTimer();
		}
		else if (!flag2 && _deactivationTriggered && _resetTimer > 0f)
		{
			DeactivateTimer();
		}
	}

	private void UpdateTimer()
	{
		if (_resetTimer > 0f)
		{
			_resetTimer -= _dayNightCycle.FixedDeltaTimeInHours;
			if (_resetTimer <= 0f)
			{
				_manufactory.ResetProductionProgress();
				DeactivateTimer();
			}
		}
	}

	private void ActivateTimer()
	{
		_resetTimer = _productionResetterSpec.HoursToResetProgress;
		_deactivationTriggered = false;
		UpdateStatus();
	}

	private void DeactivateTimer()
	{
		_resetTimer = 0f;
		_deactivationTriggered = false;
		UpdateStatus();
	}
}
