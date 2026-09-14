using System;
using System.Collections.Generic;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.WaterSystem;

[BackwardCompatible(2025, 7, 16, Compatibility.Map)]
internal class WaterSimulationMigrator : ILoadableSingleton, ISaveableSingleton, IPostLoadableSingleton
{
	private static readonly float ScaleRatio = 0.5f;

	private static readonly SingletonKey WaterSimulationMigratorKey = new SingletonKey("WaterSimulationMigrator");

	private static readonly PropertyKey<bool> IsMigratedKey = new PropertyKey<bool>("IsMigrated");

	private readonly ISingletonLoader _singletonLoader;

	private readonly EventBus _eventBus;

	private readonly HashSet<IWaterSource> _waterSourcesToMigrate = new HashSet<IWaterSource>();

	private bool _isMigrated;

	private bool _isMigrationScheduled;

	public WaterSimulationMigrator(ISingletonLoader singletonLoader, EventBus eventBus)
	{
		_singletonLoader = singletonLoader;
		_eventBus = eventBus;
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		singletonSaver.GetSingleton(WaterSimulationMigratorKey).Set(IsMigratedKey, _isMigrated);
	}

	public void Load()
	{
		_isMigrated = _singletonLoader.TryGetSingleton(WaterSimulationMigratorKey, out var objectLoader) && objectLoader.Get(IsMigratedKey);
		if (!_isMigrated)
		{
			_isMigrationScheduled = true;
			_eventBus.Register(this);
		}
	}

	public void PostLoad()
	{
		if (_isMigrationScheduled)
		{
			MigrateWaterSources();
			_eventBus.Unregister(this);
			_isMigrated = true;
		}
	}

	[OnEvent]
	public void OnBlockObjectSet(BlockObjectSetEvent blockObjectSetEvent)
	{
		IWaterSource component = blockObjectSetEvent.BlockObject.GetComponent<IWaterSource>();
		if (component != null)
		{
			_waterSourcesToMigrate.Add(component);
		}
	}

	public void MigrateOutflows(Span<ColumnOutflows> outflows)
	{
		if (_isMigrationScheduled)
		{
			int length = outflows.Length;
			for (int i = 0; i < length; i++)
			{
				ScaleOutflows(ref outflows[i]);
			}
		}
	}

	private void MigrateWaterSources()
	{
		foreach (IWaterSource item in _waterSourcesToMigrate)
		{
			item.SetSpecifiedStrength(ScaleRatio * item.SpecifiedStrength);
		}
	}

	private static void ScaleOutflows(ref ColumnOutflows outflows)
	{
		outflows.BottomFlow = new TargetedFlow(outflows.BottomFlow.Flow * ScaleRatio, outflows.BottomFlow.Index3D);
		outflows.LeftFlow = new TargetedFlow(outflows.LeftFlow.Flow * ScaleRatio, outflows.LeftFlow.Index3D);
		outflows.TopFlow = new TargetedFlow(outflows.TopFlow.Flow * ScaleRatio, outflows.TopFlow.Index3D);
		outflows.RightFlow = new TargetedFlow(outflows.RightFlow.Flow * ScaleRatio, outflows.RightFlow.Index3D);
		if (outflows.Outflows != null)
		{
			for (int i = 0; i < outflows.Outflows.Count; i++)
			{
				TargetedFlow targetedFlow = outflows.Outflows[i];
				outflows.Outflows[i] = new TargetedFlow(targetedFlow.Flow * ScaleRatio, targetedFlow.Index3D);
			}
		}
	}
}
