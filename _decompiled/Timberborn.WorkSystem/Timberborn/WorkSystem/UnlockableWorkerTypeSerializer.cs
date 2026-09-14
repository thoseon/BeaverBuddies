using Timberborn.Common;
using Timberborn.Persistence;

namespace Timberborn.WorkSystem;

public class UnlockableWorkerTypeSerializer : IValueSerializer<UnlockableWorkerType>
{
	private static readonly PropertyKey<string> WorkplaceTemplateNameKey = new PropertyKey<string>("WorkplaceTemplateName");

	private static readonly PropertyKey<string> WorkerTypeKey = new PropertyKey<string>("WorkerType");

	public void Serialize(UnlockableWorkerType value, IValueSaver valueSaver)
	{
		IObjectSaver objectSaver = valueSaver.AsObject();
		objectSaver.Set(WorkplaceTemplateNameKey, value.WorkplaceTemplateName);
		objectSaver.Set(WorkerTypeKey, value.WorkerType);
	}

	[BackwardCompatible(2025, 9, 16, Compatibility.Save)]
	public Obsoletable<UnlockableWorkerType> Deserialize(IValueLoader valueLoader)
	{
		IObjectLoader objectLoader = valueLoader.AsObject();
		PropertyKey<string> key = new PropertyKey<string>("WorkplacePrefabName");
		return new UnlockableWorkerType(objectLoader.Has(WorkplaceTemplateNameKey) ? objectLoader.Get(WorkplaceTemplateNameKey) : objectLoader.Get(key), objectLoader.Get(WorkerTypeKey));
	}
}
