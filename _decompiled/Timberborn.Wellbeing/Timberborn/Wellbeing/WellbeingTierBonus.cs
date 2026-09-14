namespace Timberborn.Wellbeing;

public readonly struct WellbeingTierBonus
{
	public int Wellbeing { get; }

	public float Bonus { get; }

	public WellbeingTierBonus(int wellbeing, float bonus)
	{
		Wellbeing = wellbeing;
		Bonus = bonus;
	}
}
