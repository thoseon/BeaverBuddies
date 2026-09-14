namespace Timberborn.ConstructionSites;

public readonly struct ConstructionSiteReservationEventArgs
{
	public Builder Builder { get; }

	public ConstructionSiteReservationEventArgs(Builder builder)
	{
		Builder = builder;
	}
}
