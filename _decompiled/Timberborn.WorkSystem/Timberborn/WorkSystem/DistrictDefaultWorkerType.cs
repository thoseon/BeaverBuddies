using Timberborn.BaseComponentSystem;
using Timberborn.DuplicationSystem;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;

namespace Timberborn.WorkSystem;

public class DistrictDefaultWorkerType : BaseComponent, IPersistentEntity, IDuplicable<DistrictDefaultWorkerType>, IDuplicable
{
	private static readonly string DefaultWorkerType = "Beaver";

	private static readonly ComponentKey DistrictDefaultWorkerTypeKey = new ComponentKey("DistrictDefaultWorkerType");

	private static readonly PropertyKey<string> WorkerTypeKey = new PropertyKey<string>("WorkerType");

	public string WorkerType { get; private set; } = DefaultWorkerType;

	public void SetWorkerType(string workerType)
	{
		if (workerType != WorkerType)
		{
			WorkerType = workerType;
		}
	}

	public void Save(IEntitySaver entitySaver)
	{
		entitySaver.GetComponent(DistrictDefaultWorkerTypeKey).Set(WorkerTypeKey, WorkerType);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(DistrictDefaultWorkerTypeKey);
		WorkerType = component.Get(WorkerTypeKey);
	}

	public void DuplicateFrom(DistrictDefaultWorkerType source)
	{
		SetWorkerType(source.WorkerType);
	}
}
