namespace Timberborn.Cutting;

public class CuttableCutEvent
{
	public Cuttable Cuttable { get; }

	public CuttableCutEvent(Cuttable cuttable)
	{
		Cuttable = cuttable;
	}
}
