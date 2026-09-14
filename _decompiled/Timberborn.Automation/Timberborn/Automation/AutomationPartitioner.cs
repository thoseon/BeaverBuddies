using System.Collections.Generic;
using Timberborn.Common;

namespace Timberborn.Automation;

internal class AutomationPartitioner
{
	private readonly AutomatorPartitionFactory _automatorPartitionFactory;

	private readonly Queue<Automator> _queue = new Queue<Automator>();

	public AutomationPartitioner(AutomatorPartitionFactory automatorPartitionFactory)
	{
		_automatorPartitionFactory = automatorPartitionFactory;
	}

	public List<AutomatorPartition> AssignPartitions(ReadOnlyList<Automator> automators)
	{
		List<AutomatorPartition> list = new List<AutomatorPartition>();
		AssignPartitions(automators, list);
		return list;
	}

	public void ReassignExistingPartition(AutomatorPartition partition, List<AutomatorPartition> partitions)
	{
		AssignPartitions(partition.Automators.AsReadOnlyList(), partitions);
		partitions.Remove(partition);
		partition.Clear();
	}

	public void MergePartitions(AutomatorPartition partitionA, AutomatorPartition partitionB, List<AutomatorPartition> partitions)
	{
		if (partitionA != partitionB)
		{
			AutomatorPartition automatorPartition = ((partitionA.Size >= partitionB.Size) ? partitionA : partitionB);
			AutomatorPartition automatorPartition2 = ((automatorPartition == partitionA) ? partitionB : partitionA);
			automatorPartition2.MergeInto(automatorPartition);
			partitions.Remove(automatorPartition2);
			automatorPartition2.Clear();
		}
	}

	public void AddAutomator(Automator automator, List<AutomatorPartition> partitions)
	{
		AutomatorPartition automatorPartition = (automator.Partition = _automatorPartitionFactory.Create());
		automatorPartition.Add(automator);
		partitions.Add(automatorPartition);
		for (int i = 0; i < automator.InputConnections.Count; i++)
		{
			AutomatorConnection automatorConnection = automator.InputConnections[i];
			if (automatorConnection.IsConnected && automatorConnection.Transmitter.RegisteredForRunning)
			{
				AutomatorPartition partition = automatorConnection.Transmitter.Partition;
				if (partition != automator.Partition)
				{
					MergePartitions(partition, automator.Partition, partitions);
				}
			}
		}
		for (int j = 0; j < automator.OutputConnections.Count; j++)
		{
			AutomatorConnection automatorConnection2 = automator.OutputConnections[j];
			if (automatorConnection2.Receiver.RegisteredForRunning)
			{
				AutomatorPartition partition2 = automatorConnection2.Receiver.Partition;
				if (partition2 != automator.Partition)
				{
					MergePartitions(partition2, automator.Partition, partitions);
				}
			}
		}
	}

	public void RemoveAutomator(Automator automator, List<AutomatorPartition> partitions)
	{
		AutomatorPartition partition = automator.Partition;
		automator.Partition = null;
		partition.Automators.Remove(automator);
		AssignPartitions(partition.Automators.AsReadOnlyList(), partitions);
		partitions.Remove(partition);
		partition.Clear();
	}

	private void AssignPartitions(ReadOnlyList<Automator> automators, List<AutomatorPartition> partitions)
	{
		for (int i = 0; i < automators.Count; i++)
		{
			automators[i].Partition = null;
		}
		Asserts.CollectionIsEmpty(_queue, "_queue");
		for (int j = 0; j < automators.Count; j++)
		{
			Automator automator = automators[j];
			if (automator.Partition == null)
			{
				AutomatorPartition automatorPartition = _automatorPartitionFactory.Create();
				partitions.Add(automatorPartition);
				automator.Partition = automatorPartition;
				_queue.Enqueue(automator);
				Automator result;
				while (_queue.TryDequeue(out result))
				{
					automatorPartition.Add(result);
					EnqueueInputs(result, automatorPartition);
					EnqueueOutputs(result, automatorPartition);
				}
			}
		}
	}

	private void EnqueueInputs(Automator current, AutomatorPartition partition)
	{
		ReadOnlyList<AutomatorConnection> inputConnections = current.InputConnections;
		for (int i = 0; i < inputConnections.Count; i++)
		{
			AutomatorConnection automatorConnection = inputConnections[i];
			if (automatorConnection.IsConnected)
			{
				Automator transmitter = automatorConnection.Transmitter;
				if (transmitter.RegisteredForRunning && transmitter.Partition == null)
				{
					transmitter.Partition = partition;
					_queue.Enqueue(transmitter);
				}
			}
		}
	}

	private void EnqueueOutputs(Automator current, AutomatorPartition partition)
	{
		ReadOnlyList<AutomatorConnection> outputConnections = current.OutputConnections;
		for (int i = 0; i < outputConnections.Count; i++)
		{
			Automator receiver = outputConnections[i].Receiver;
			if (receiver.RegisteredForRunning && receiver.Partition == null)
			{
				receiver.Partition = partition;
				_queue.Enqueue(receiver);
			}
		}
	}
}
