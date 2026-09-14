using Timberborn.Persistence;

namespace Timberborn.SaveMetadataSystem;

public class ModReferenceSerializer : IValueSerializer<ModReference>
{
	private static readonly PropertyKey<string> IdKey = new PropertyKey<string>("Id");

	private static readonly PropertyKey<string> NameKey = new PropertyKey<string>("Name");

	private static readonly PropertyKey<string> VersionKey = new PropertyKey<string>("Version");

	public void Serialize(ModReference value, IValueSaver valueSaver)
	{
		IObjectSaver objectSaver = valueSaver.AsObject();
		objectSaver.Set(IdKey, value.Id);
		objectSaver.Set(NameKey, value.Name);
		objectSaver.Set(VersionKey, value.Version);
	}

	public Obsoletable<ModReference> Deserialize(IValueLoader valueLoader)
	{
		IObjectLoader objectLoader = valueLoader.AsObject();
		return new ModReference(objectLoader.Get(IdKey), objectLoader.Get(NameKey), objectLoader.Get(VersionKey));
	}
}
