using System;
using Timberborn.MapStateSystem;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.ScienceSystem;

public class ScienceService : ISaveableSingleton, ILoadableSingleton
{
	private static readonly SingletonKey ScienceServiceKey = new SingletonKey("ScienceService");

	private static readonly PropertyKey<int> SciencePointsKey = new PropertyKey<int>("SciencePoints");

	private readonly ISingletonLoader _singletonLoader;

	private readonly MapEditorMode _mapEditorMode;

	public int SciencePoints { get; private set; }

	public ScienceService(ISingletonLoader singletonLoader, MapEditorMode mapEditorMode)
	{
		_singletonLoader = singletonLoader;
		_mapEditorMode = mapEditorMode;
	}

	public void Load()
	{
		if (_singletonLoader.TryGetSingleton(ScienceServiceKey, out var objectLoader))
		{
			SciencePoints = objectLoader.Get(SciencePointsKey);
		}
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		if (!_mapEditorMode.IsMapEditor)
		{
			singletonSaver.GetSingleton(ScienceServiceKey).Set(SciencePointsKey, SciencePoints);
		}
	}

	public void AddPoints(int amount)
	{
		SciencePoints += amount;
	}

	public void SubtractPoints(int amount)
	{
		if (SciencePoints - amount < 0)
		{
			throw new ArgumentException($"Can't subtract {amount} science points, " + $"there are only {SciencePoints} points stored");
		}
		SciencePoints -= amount;
	}
}
