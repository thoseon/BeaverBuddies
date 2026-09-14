using System;
using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.SerializationSystem;
using Timberborn.Versioning;

namespace Timberborn.WorldSerialization;

public class SerializedWorld
{
	private readonly Dictionary<string, SerializedSingleton> _singletons = new Dictionary<string, SerializedSingleton>();

	private readonly List<SerializedEntity> _entities = new List<SerializedEntity>();

	public Timberborn.Versioning.Version Version { get; }

	public SerializedWorld(Timberborn.Versioning.Version version)
	{
		Version = version;
	}

	public IEnumerable<SerializedEntity> Entities()
	{
		return _entities.AsReadOnlyEnumerable();
	}

	public IEnumerable<SerializedSingleton> Singletons()
	{
		return _singletons.Values.AsReadOnlyEnumerable();
	}

	public void AddEntity(SerializedEntity serializedEntity)
	{
		_entities.Add(serializedEntity);
	}

	public void AddSingleton(SerializedSingleton serializedSingleton)
	{
		_singletons.Add(serializedSingleton.Name, serializedSingleton);
	}

	public SerializedObject GetOrAddSingleton(string name)
	{
		if (!_singletons.TryGetValue(name, out var value))
		{
			value = new SerializedSingleton(name);
			_singletons.Add(name, value);
		}
		return value.Value;
	}

	public SerializedObject GetSingleton(string name)
	{
		if (_singletons.TryGetValue(name, out var value))
		{
			return value.Value;
		}
		throw new ArgumentException("Singleton '" + name + "' does not exist.");
	}

	public bool HasSingleton(string name)
	{
		return _singletons.ContainsKey(name);
	}
}
