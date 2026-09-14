using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.Common;

namespace Timberborn.Automation;

internal class AutomationPlan
{
	private readonly AutomationPlanVersioner _automationPlanVersioner;

	private readonly Queue<Automator> _queue = new Queue<Automator>();

	private readonly List<Automator> _combinationalPlan = new List<Automator>();

	private readonly List<Automator> _samplingPlan = new List<Automator>();

	private readonly List<Automator> _sequentialPlan = new List<Automator>();

	private readonly List<Automator> _terminalPlan = new List<Automator>();

	public bool IsSampling => !_samplingPlan.IsEmpty();

	public bool IsSequential => !_sequentialPlan.IsEmpty();

	public AutomationPlan(AutomationPlanVersioner automationPlanVersioner)
	{
		_automationPlanVersioner = automationPlanVersioner;
	}

	public void Clear()
	{
		_combinationalPlan.Clear();
		_samplingPlan.Clear();
		_sequentialPlan.Clear();
		_terminalPlan.Clear();
		_queue.Clear();
	}

	public void Build(List<Automator> all)
	{
		Clear();
		long num = _automationPlanVersioner.AcquirePlanVersion();
		for (int i = 0; i < all.Count; i++)
		{
			all[i].Indegree = 0;
		}
		for (int j = 0; j < all.Count; j++)
		{
			Automator automator = all[j];
			if (!automator.IsCombinationalTransmitter)
			{
				continue;
			}
			for (int k = 0; k < automator.InputConnections.Count; k++)
			{
				AutomatorConnection automatorConnection = automator.InputConnections[k];
				if (automatorConnection.IsConnected && automatorConnection.Transmitter.RegisteredForRunning && automatorConnection.Transmitter.IsCombinationalTransmitter)
				{
					automator.Indegree++;
				}
			}
		}
		for (int l = 0; l < all.Count; l++)
		{
			Automator automator2 = all[l];
			if (automator2.IsCombinationalTransmitter && automator2.Indegree == 0)
			{
				_queue.Enqueue(automator2);
			}
		}
		Automator result;
		while (_queue.TryDequeue(out result))
		{
			_combinationalPlan.Add(result);
			result.PlanVersion = num;
			for (int m = 0; m < result.OutputConnections.Count; m++)
			{
				Automator receiver = result.OutputConnections[m].Receiver;
				if (receiver.IsCombinationalTransmitter)
				{
					receiver.Indegree--;
					if (receiver.Indegree == 0)
					{
						_queue.Enqueue(receiver);
					}
				}
			}
		}
		for (int n = 0; n < all.Count; n++)
		{
			Automator automator3 = all[n];
			bool flag = automator3.IsCombinationalTransmitter && automator3.PlanVersion != num;
			automator3.SetCyclicOrBlocked(flag);
			if (flag)
			{
				_combinationalPlan.Add(automator3);
			}
		}
		for (int num2 = 0; num2 < all.Count; num2++)
		{
			Automator automator4 = all[num2];
			if (automator4.IsSamplingTransmitter)
			{
				_samplingPlan.Add(automator4);
			}
		}
		for (int num3 = 0; num3 < all.Count; num3++)
		{
			Automator automator5 = all[num3];
			if (automator5.IsSequentialTransmitter)
			{
				_sequentialPlan.Add(automator5);
			}
		}
		for (int num4 = 0; num4 < all.Count; num4++)
		{
			Automator automator6 = all[num4];
			if (automator6.IsTerminal)
			{
				_terminalPlan.Add(automator6);
			}
		}
	}

	public void Sample()
	{
		for (int i = 0; i < _samplingPlan.Count; i++)
		{
			_samplingPlan[i].Sample();
		}
	}

	public void EvaluateCombinational()
	{
		for (int i = 0; i < _combinationalPlan.Count; i++)
		{
			_combinationalPlan[i].EvaluateCombinational();
		}
	}

	public void EvaluateNext()
	{
		for (int i = 0; i < _sequentialPlan.Count; i++)
		{
			_sequentialPlan[i].EvaluateNext();
		}
	}

	public void CommitTick()
	{
		for (int i = 0; i < _sequentialPlan.Count; i++)
		{
			_sequentialPlan[i].CommitTick();
		}
	}

	public void EvaluateTerminal()
	{
		for (int i = 0; i < _terminalPlan.Count; i++)
		{
			_terminalPlan[i].EvaluateTerminal();
		}
	}

	public ImmutableArray<Automator> GetSnapshot()
	{
		return _combinationalPlan.ToImmutableArray();
	}
}
