using Timberborn.AchievementSystem;
using Timberborn.MechanicalSystem;
using Timberborn.TickSystem;

namespace Timberborn.Achievements;

internal class BatteryChargeStorageAchievement : Achievement, ITickableSingleton
{
	private static readonly float RequiredCharge = 655321f;

	private readonly MechanicalGraphRegistry _mechanicalGraphRegistry;

	public override string Id => "BATTERY_CHARGE_STORAGE";

	public BatteryChargeStorageAchievement(MechanicalGraphRegistry mechanicalGraphRegistry)
	{
		_mechanicalGraphRegistry = mechanicalGraphRegistry;
	}

	public void Tick()
	{
		if (base.IsEnabled)
		{
			ValidateTotalCharge();
		}
	}

	private void ValidateTotalCharge()
	{
		float num = 0f;
		foreach (MechanicalGraph mechanicalGraph in _mechanicalGraphRegistry.MechanicalGraphs)
		{
			num += (float)mechanicalGraph.BatteryCharge;
		}
		if (num >= RequiredCharge)
		{
			Unlock();
		}
	}
}
