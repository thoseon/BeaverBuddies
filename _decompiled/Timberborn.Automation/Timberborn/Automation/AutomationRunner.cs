using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using Timberborn.Common;
using Timberborn.SingletonSystem;
using Timberborn.TickSystem;

namespace Timberborn.Automation;

internal class AutomationRunner : IPostLoadableSingleton, IUpdatableSingleton, ITickableSingleton, ILateTickable, IAutomationRunnerDebugger
{
	private readonly AutomationPartitioner _automationPartitioner;

	private readonly AutomationDebugger _automationDebugger;

	private readonly ISingletonRepository _singletonRepository;

	private readonly AutomatorRegistry _automatorRegistry;

	private readonly List<AutomatorPartition> _scheduledPartitions = new List<AutomatorPartition>();

	private readonly List<AutomatorPartition> _samplingPartitions = new List<AutomatorPartition>();

	private readonly List<AutomatorPartition> _sequentialPartitions = new List<AutomatorPartition>();

	private ImmutableArray<ISamplingSingleton> _samplingSingletons;

	private ImmutableArray<ICommittingSingleton> _committingSingletons;

	private List<AutomatorPartition> _partitions;

	private bool _loaded;

	public int PartitionCount => _partitions?.Count ?? 0;

	public AutomationRunner(AutomationPartitioner automationPartitioner, AutomationDebugger automationDebugger, ISingletonRepository singletonRepository, AutomatorRegistry automatorRegistry)
	{
		_automationPartitioner = automationPartitioner;
		_automationDebugger = automationDebugger;
		_singletonRepository = singletonRepository;
		_automatorRegistry = automatorRegistry;
	}

	public void PostLoad()
	{
		CollectSingletons();
		AssignPartitions();
		SampleSingletons();
		SamplePartitions();
		ScheduleAllPartitions();
		_loaded = true;
	}

	public void Tick()
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		EvaluateScheduled(evaluateNext: false);
		EvaluateNextPartitions();
		CommitTickSingletons();
		CommitTickPartitions();
		SampleSingletons();
		SamplePartitions();
		EvaluateScheduled(evaluateNext: false);
		_automationDebugger.TickEvaluationTimeMs.Register(stopwatch);
	}

	public void UpdateSingleton()
	{
		EvaluateScheduled(evaluateNext: true);
	}

	public void Register(Automator automator)
	{
		_automatorRegistry.Register(automator);
		if (_loaded)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			_automationPartitioner.AddAutomator(automator, _partitions);
			UpdateSecondaryPartitionLists();
			_automationDebugger.AddingTimeMs.Register(stopwatch);
		}
	}

	public void Unregister(Automator automator)
	{
		_automatorRegistry.Unregister(automator);
		if (_loaded)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			_automationPartitioner.RemoveAutomator(automator, _partitions);
			UpdateSecondaryPartitionLists();
			_automationDebugger.RemovingTimeMs.Register(stopwatch);
		}
	}

	public void ReassignExistingPartition(AutomatorPartition partition)
	{
		if (_loaded)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			_automationPartitioner.ReassignExistingPartition(partition, _partitions);
			UpdateSecondaryPartitionLists();
			_automationDebugger.PartitioningTimeMs.Register(stopwatch);
		}
	}

	public void MergePartitions(AutomatorPartition partitionA, AutomatorPartition partitionB)
	{
		if (_loaded)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			_automationPartitioner.MergePartitions(partitionA, partitionB, _partitions);
			UpdateSecondaryPartitionLists();
			_automationDebugger.MergingTimeMs.Register(stopwatch);
		}
	}

	public ImmutableArray<AutomatorPartition> GetPartitionsSnapshot()
	{
		return _partitions.ToImmutableArray();
	}

	public void Schedule(AutomatorPartition automatorPartition)
	{
		if (!automatorPartition.IsScheduled)
		{
			_scheduledPartitions.Add(automatorPartition);
			automatorPartition.IsScheduled = true;
		}
	}

	private void CollectSingletons()
	{
		_samplingSingletons = _singletonRepository.GetSingletons<ISamplingSingleton>().ToImmutableArray();
		_committingSingletons = _singletonRepository.GetSingletons<ICommittingSingleton>().ToImmutableArray();
	}

	private void AssignPartitions()
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		_partitions = _automationPartitioner.AssignPartitions(_automatorRegistry.Automators);
		UpdateSecondaryPartitionLists();
		_automationDebugger.PartitioningTimeMs.Register(stopwatch);
	}

	private void ScheduleAllPartitions()
	{
		for (int i = 0; i < _partitions.Count; i++)
		{
			Schedule(_partitions[i]);
		}
	}

	private void EvaluateScheduled(bool evaluateNext)
	{
		if (_scheduledPartitions.IsEmpty())
		{
			return;
		}
		Stopwatch stopwatch = Stopwatch.StartNew();
		for (int i = 0; i < _scheduledPartitions.Count; i++)
		{
			AutomatorPartition automatorPartition = _scheduledPartitions[i];
			automatorPartition.EvaluateCombinational();
			automatorPartition.EvaluateTerminal();
			if (evaluateNext)
			{
				automatorPartition.EvaluateNext();
			}
		}
		_scheduledPartitions.Clear();
		_automationDebugger.EvaluationTimeMs.Register(stopwatch);
	}

	private void SamplePartitions()
	{
		for (int i = 0; i < _samplingPartitions.Count; i++)
		{
			_samplingPartitions[i].Sample();
		}
	}

	private void EvaluateNextPartitions()
	{
		for (int i = 0; i < _sequentialPartitions.Count; i++)
		{
			_sequentialPartitions[i].EvaluateNext();
		}
	}

	private void CommitTickPartitions()
	{
		for (int i = 0; i < _sequentialPartitions.Count; i++)
		{
			_sequentialPartitions[i].CommitTick();
		}
	}

	private void SampleSingletons()
	{
		for (int i = 0; i < _samplingSingletons.Length; i++)
		{
			_samplingSingletons[i].Sample();
		}
	}

	private void CommitTickSingletons()
	{
		for (int i = 0; i < _committingSingletons.Length; i++)
		{
			_committingSingletons[i].CommitTick();
		}
	}

	private void UpdateSecondaryPartitionLists()
	{
		UpdateAllPlans();
		UpdateSamplingPartitions();
		UpdateSequentialPartitions();
	}

	private void UpdateAllPlans()
	{
		for (int i = 0; i < _partitions.Count; i++)
		{
			_partitions[i].UpdatePlan();
		}
	}

	private void UpdateSamplingPartitions()
	{
		_samplingPartitions.Clear();
		for (int i = 0; i < _partitions.Count; i++)
		{
			AutomatorPartition automatorPartition = _partitions[i];
			if (automatorPartition.IsSampling)
			{
				_samplingPartitions.Add(automatorPartition);
			}
		}
	}

	private void UpdateSequentialPartitions()
	{
		_sequentialPartitions.Clear();
		for (int i = 0; i < _partitions.Count; i++)
		{
			AutomatorPartition automatorPartition = _partitions[i];
			if (automatorPartition.IsSequential)
			{
				_sequentialPartitions.Add(automatorPartition);
			}
		}
	}
}
