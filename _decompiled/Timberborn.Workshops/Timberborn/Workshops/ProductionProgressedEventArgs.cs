namespace Timberborn.Workshops;

public class ProductionProgressedEventArgs
{
	public float ProductionProgressChange { get; }

	public ProductionProgressedEventArgs(float productionProgressChange)
	{
		ProductionProgressChange = productionProgressChange;
	}
}
