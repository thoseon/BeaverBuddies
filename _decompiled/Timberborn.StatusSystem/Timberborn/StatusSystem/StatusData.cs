namespace Timberborn.StatusSystem;

public readonly struct StatusData
{
	public int Count { get; }

	public float Value { get; }

	public StatusWarningType StatusWarningType { get; }

	public StatusData(int count, float value, StatusWarningType statusWarningType)
	{
		Count = count;
		Value = value;
		StatusWarningType = statusWarningType;
	}
}
