using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.DuplicationSystem;
using Timberborn.GameDistricts;
using Timberborn.Persistence;
using Timberborn.TemplateSystem;
using Timberborn.WorkerTypes;
using Timberborn.WorldPersistence;

namespace Timberborn.WorkSystem;

public class WorkplaceWorkerType : BaseComponent, IAwakableComponent, IPersistentEntity, IDuplicable<WorkplaceWorkerType>, IDuplicable, IUnfinishedStateListener, IFinishedStateListener
{
	private static readonly ComponentKey WorkplaceWorkerTypeKey = new ComponentKey("WorkplaceWorkerType");

	private static readonly PropertyKey<string> WorkerTypeKey = new PropertyKey<string>("WorkerType");

	private static readonly PropertyKey<bool> WasSetKey = new PropertyKey<bool>("WasSet");

	private readonly WorkerTypeService _workerTypeService;

	private readonly WorkplaceUnlockingService _workplaceUnlockingService;

	private DistrictBuilding _districtBuilding;

	private TemplateSpec _templateSpec;

	private WorkplaceSpec _workplaceSpec;

	private Workplace _workplace;

	private bool _wasSet;

	public string WorkerType { get; private set; }

	public event EventHandler<WorkerTypeChangedEventArgs> WorkerTypeChanged;

	public WorkplaceWorkerType(WorkerTypeService workerTypeService, WorkplaceUnlockingService workplaceUnlockingService)
	{
		_workerTypeService = workerTypeService;
		_workplaceUnlockingService = workplaceUnlockingService;
	}

	public void Awake()
	{
		_districtBuilding = GetComponent<DistrictBuilding>();
		_templateSpec = GetComponent<TemplateSpec>();
		_workplaceSpec = GetComponent<WorkplaceSpec>();
		_workplace = GetComponent<Workplace>();
		WorkerType = _workplaceSpec.DefaultWorkerType;
	}

	public void SetWorkerType(string workerType)
	{
		if (workerType != WorkerType && IsWorkerTypeAllowed(workerType) && IsWorkerTypeUnlocked(workerType))
		{
			_workplace.UnassignAllWorkers();
			string workerType2 = WorkerType;
			WorkerType = workerType;
			WorkerTypeChanged?.Invoke(this, new WorkerTypeChangedEventArgs(workerType2, WorkerType));
		}
		_wasSet = true;
	}

	public void OnEnterUnfinishedState()
	{
		if (!_wasSet)
		{
			_districtBuilding.ReassignedConstructionDistrict += OnReassignedConstructionDistrict;
		}
	}

	public void OnExitUnfinishedState()
	{
	}

	public void OnEnterFinishedState()
	{
		if ((bool)_districtBuilding.InstantDistrict)
		{
			SetWorkerTypeToDistrict(_districtBuilding.InstantDistrict);
		}
		else if (!_wasSet)
		{
			_districtBuilding.ReassignedInstantDistrict += OnReassignedInstantDistrict;
		}
	}

	public void OnExitFinishedState()
	{
	}

	public void Save(IEntitySaver entitySaver)
	{
		IObjectSaver component = entitySaver.GetComponent(WorkplaceWorkerTypeKey);
		component.Set(WorkerTypeKey, WorkerType);
		component.Set(WasSetKey, _wasSet);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(WorkplaceWorkerTypeKey);
		string workerType = _workerTypeService.GetWorkerType(component.Get(WorkerTypeKey));
		if (IsWorkerTypeAllowed(workerType) && IsWorkerTypeUnlocked(workerType))
		{
			WorkerType = workerType;
		}
		_wasSet = component.Get(WasSetKey);
	}

	public void DuplicateFrom(WorkplaceWorkerType source)
	{
		string workerType = source.WorkerType;
		if (IsWorkerTypeAllowed(workerType) && IsWorkerTypeUnlocked(workerType))
		{
			SetWorkerType(workerType);
		}
	}

	private void OnReassignedConstructionDistrict(object sender, EventArgs e)
	{
		SetWorkerTypeToDistrict(_districtBuilding.ConstructionDistrict);
		_districtBuilding.ReassignedConstructionDistrict -= OnReassignedConstructionDistrict;
	}

	private void OnReassignedInstantDistrict(object sender, EventArgs e)
	{
		SetWorkerTypeToDistrict(_districtBuilding.InstantDistrict);
		_districtBuilding.ReassignedInstantDistrict -= OnReassignedInstantDistrict;
	}

	private bool IsWorkerTypeAllowed(string workerType)
	{
		if (_workplaceSpec.DisallowOtherWorkerTypes)
		{
			return _workplaceSpec.DefaultWorkerType == workerType;
		}
		return true;
	}

	private bool IsWorkerTypeUnlocked(string workerType)
	{
		UnlockableWorkerType unlockableWorkerType = new UnlockableWorkerType(_templateSpec.TemplateName, workerType);
		return _workplaceUnlockingService.Unlocked(unlockableWorkerType);
	}

	private void SetWorkerTypeToDistrict(DistrictCenter districtCenter)
	{
		if (!_wasSet && (bool)districtCenter)
		{
			DistrictDefaultWorkerType component = districtCenter.GetComponent<DistrictDefaultWorkerType>();
			SetWorkerType(component.WorkerType);
		}
	}
}
