namespace Timberborn.BatchControl;

public class BatchControlTabShownEvent
{
	public BatchControlTab BatchControlTab { get; }

	public BatchControlTabShownEvent(BatchControlTab batchControlTab)
	{
		BatchControlTab = batchControlTab;
	}
}
