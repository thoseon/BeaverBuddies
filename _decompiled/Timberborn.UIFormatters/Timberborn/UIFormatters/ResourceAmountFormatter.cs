using Timberborn.Localization;

namespace Timberborn.UIFormatters;

public class ResourceAmountFormatter
{
	private static readonly string ResourceNameAndAmountLocKey = "Core.GoodNameAndAmount";

	private static readonly string ResourceNameAndAmountPerHourLocKey = "Core.GoodNameAndAmountPerHour";

	private readonly ILoc _loc;

	public ResourceAmountFormatter(ILoc loc)
	{
		_loc = loc;
	}

	public string Format(string resourceName, int amount)
	{
		return _loc.T(ResourceNameAndAmountLocKey, resourceName, amount.ToString());
	}

	public string FormatPerHour(string resourceName, float amount)
	{
		return _loc.T(ResourceNameAndAmountPerHourLocKey, resourceName, amount.ToString("0.#"));
	}
}
