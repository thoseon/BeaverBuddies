using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.Common;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.EntityNaming;

internal class NumberedEntityNamerService : ILoadableSingleton, ISaveableSingleton
{
	private static readonly SingletonKey SingletonKey = new SingletonKey("NumberedEntityNamerService");

	private static readonly ListKey<SerializedEntityNameNumber> NextNumbersKey = new ListKey<SerializedEntityNameNumber>("NextNumbers");

	private readonly ISingletonLoader _singletonLoader;

	private readonly SerializedEntityNameNumberSerializer _serializedEntityNameNumberSerializer;

	private readonly Dictionary<string, int> _nextNumbers = new Dictionary<string, int>();

	public NumberedEntityNamerService(ISingletonLoader singletonLoader, SerializedEntityNameNumberSerializer serializedEntityNameNumberSerializer)
	{
		_singletonLoader = singletonLoader;
		_serializedEntityNameNumberSerializer = serializedEntityNameNumberSerializer;
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		singletonSaver.GetSingleton(SingletonKey).Set(NextNumbersKey, SerializeNextNumbers(), _serializedEntityNameNumberSerializer);
	}

	public void Load()
	{
		if (_singletonLoader.TryGetSingleton(SingletonKey, out var objectLoader))
		{
			_nextNumbers.AddRange(DeserializeNextNumbers(objectLoader.Get(NextNumbersKey, _serializedEntityNameNumberSerializer)));
		}
	}

	public int GenerateNumber(string group)
	{
		int num = ((!_nextNumbers.TryGetValue(group, out var value)) ? 1 : (value + 1));
		_nextNumbers[group] = num;
		return num;
	}

	private ImmutableArray<SerializedEntityNameNumber> SerializeNextNumbers()
	{
		return _nextNumbers.Select((KeyValuePair<string, int> nextNumber) => new SerializedEntityNameNumber(nextNumber.Key, nextNumber.Value)).ToImmutableArray();
	}

	private static IEnumerable<KeyValuePair<string, int>> DeserializeNextNumbers(List<SerializedEntityNameNumber> nextPersistentNameNumbers)
	{
		return nextPersistentNameNumbers.Select((SerializedEntityNameNumber nextNumber) => new KeyValuePair<string, int>(nextNumber.Group, nextNumber.NextNumber));
	}
}
