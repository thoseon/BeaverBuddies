using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Timberborn.Automation;

public class AutomatorPartition
{
	internal readonly List<Automator> Automators = new List<Automator>();

	internal bool IsScheduled;

	private readonly AutomationPlan _automationPlan;

	private readonly AutomationDebugger _automationDebugger;

	private readonly Queue<Automator> _postponedAutomatorListeners = new Queue<Automator>();

	private bool _planReady;

	private bool _evaluating;

	public int Size => Automators.Count;

	public string DebuggingId => $"{GetHashCode():x8}";

	internal bool IsSampling => _automationPlan.IsSampling;

	internal bool IsSequential => _automationPlan.IsSequential;

	internal AutomatorPartition(AutomationPlan automationPlan, AutomationDebugger automationDebugger)
	{
		_automationPlan = automationPlan;
		_automationDebugger = automationDebugger;
	}

	internal void Clear()
	{
		Automators.Clear();
		_postponedAutomatorListeners.Clear();
		InvalidatePlan();
	}

	internal void EvaluateCombinational()
	{
		if (!_evaluating)
		{
			_evaluating = true;
			UpdatePlan();
			_automationPlan.EvaluateCombinational();
			EvaluatePostponedAutomatorListeners();
			_evaluating = false;
			IsScheduled = false;
		}
	}

	internal void EvaluateNext()
	{
		_automationPlan.EvaluateNext();
	}

	internal void CommitTick()
	{
		_automationPlan.CommitTick();
	}

	internal void Sample()
	{
		_automationPlan.Sample();
	}

	internal void EvaluateTerminal()
	{
		_automationPlan.EvaluateTerminal();
	}

	internal void Add(Automator automator)
	{
		Automators.Add(automator);
	}

	internal void MergeInto(AutomatorPartition destination)
	{
		for (int i = 0; i < Automators.Count; i++)
		{
			Automator automator = Automators[i];
			automator.Partition = destination;
			destination.Add(automator);
		}
		destination.InvalidatePlan();
	}

	internal void NotifyOrPostponeAutomatorListeners(Automator automator)
	{
		if (_evaluating)
		{
			if (!automator.PostponedNotifyListeners)
			{
				automator.PostponedNotifyListeners = true;
				_postponedAutomatorListeners.Enqueue(automator);
			}
		}
		else
		{
			automator.NotifyListenersNow();
		}
	}

	internal void UpdatePlan()
	{
		if (!_planReady)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			_automationPlan.Build(Automators);
			_planReady = true;
			_automationDebugger.PlanningTimeMs.Register(stopwatch);
		}
	}

	internal void InvalidatePlan()
	{
		_automationPlan.Clear();
		_planReady = false;
	}

	internal ImmutableArray<Automator> GetPlanSnapshot()
	{
		return _automationPlan.GetSnapshot();
	}

	private void EvaluatePostponedAutomatorListeners()
	{
		Automator result;
		while (_postponedAutomatorListeners.TryDequeue(out result))
		{
			result.PostponedNotifyListeners = false;
			result.NotifyListenersNow();
		}
	}
}
