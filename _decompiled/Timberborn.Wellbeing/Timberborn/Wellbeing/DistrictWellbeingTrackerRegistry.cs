using Timberborn.BaseComponentSystem;

namespace Timberborn.Wellbeing;

internal class DistrictWellbeingTrackerRegistry : BaseComponent
{
	public WellbeingTrackerRegistry Registry { get; } = new WellbeingTrackerRegistry();
}
