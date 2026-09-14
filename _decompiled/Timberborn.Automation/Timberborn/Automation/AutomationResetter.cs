using System.Collections.Generic;

namespace Timberborn.Automation;

public class AutomationResetter
{
	private readonly List<ISequentialTransmitter> _reusableSequentialTransmitters = new List<ISequentialTransmitter>();

	public void ResetPartition(Automator seedAutomator)
	{
		AutomatorPartition partition = seedAutomator.Partition;
		if (partition == null)
		{
			return;
		}
		foreach (Automator automator in partition.Automators)
		{
			automator.GetComponents(_reusableSequentialTransmitters);
			foreach (ISequentialTransmitter reusableSequentialTransmitter in _reusableSequentialTransmitters)
			{
				reusableSequentialTransmitter.Reset();
			}
			_reusableSequentialTransmitters.Clear();
		}
	}
}
