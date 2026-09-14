namespace Timberborn.BonusSystem;

public readonly struct BonusValueChangedEventArgs
{
	public string BonusId { get; }

	public float Value { get; }

	public BonusValueChangedEventArgs(string bonusId, float value)
	{
		BonusId = bonusId;
		Value = value;
	}
}
