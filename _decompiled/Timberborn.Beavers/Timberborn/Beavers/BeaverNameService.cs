using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.Common;
using Timberborn.Localization;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.Beavers;

internal class BeaverNameService : ISaveableSingleton, ILoadableSingleton
{
	private static readonly string BeaverNamePoolLocKey = "Beaver.NamePool";

	private static readonly SingletonKey BeaverNameServiceKey = new SingletonKey("BeaverNameService");

	private static readonly ListKey<string> NamesKey = new ListKey<string>("Names");

	private readonly ISingletonLoader _singletonLoader;

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private readonly ILoc _loc;

	private readonly List<string> _completeNamePool = new List<string>();

	private readonly List<string> _names = new List<string>();

	public BeaverNameService(ISingletonLoader singletonLoader, IRandomNumberGenerator randomNumberGenerator, ILoc loc)
	{
		_singletonLoader = singletonLoader;
		_randomNumberGenerator = randomNumberGenerator;
		_loc = loc;
	}

	public void Load()
	{
		InitializeCompleteNamePool();
		if (_singletonLoader.TryGetSingleton(BeaverNameServiceKey, out var objectLoader))
		{
			List<string> first = objectLoader.Get(NamesKey);
			_names.Clear();
			_names.AddRange(first.Intersect(_completeNamePool));
		}
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		singletonSaver.GetSingleton(BeaverNameServiceKey).Set(NamesKey, _names);
	}

	public string RandomName()
	{
		if (_names.Count == 0)
		{
			_names.AddRange(_completeNamePool);
		}
		string listElement = _randomNumberGenerator.GetListElement(_names);
		_names.Remove(listElement);
		return listElement;
	}

	private void InitializeCompleteNamePool()
	{
		IEnumerable<string> collection = from name in _loc.T(BeaverNamePoolLocKey).Split('\n').Select(SanitizeName)
			where name.Length > 0
			select name;
		_completeNamePool.AddRange(collection);
		if (_completeNamePool.IsEmpty())
		{
			throw new Exception("Name pool is empty.");
		}
	}

	private static string SanitizeName(string name)
	{
		return name.Replace("\r", "").Trim();
	}
}
