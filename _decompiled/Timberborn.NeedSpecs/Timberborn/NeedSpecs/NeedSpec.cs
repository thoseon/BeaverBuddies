using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using Timberborn.LocalizationSerialization;

namespace Timberborn.NeedSpecs;

public record NeedSpec : ComponentSpec
{
	[Serialize]
	public string Id { get; init; }

	[Serialize]
	public ImmutableArray<string> BackwardCompatibleIds { get; init; }

	[Serialize]
	public int Order { get; init; }

	[Serialize]
	public string NeedGroupId { get; init; }

	[Serialize]
	public string CharacterType { get; init; }

	[Serialize("DisplayNameLocKey")]
	public LocalizedText DisplayName { get; init; }

	[Serialize]
	public float StartingValue { get; init; }

	[Serialize]
	public float MinimumValue { get; init; }

	[Serialize]
	public float MaximumValue { get; init; }

	[Serialize]
	public float DailyDelta { get; init; }

	[Serialize]
	public float ImportanceMultiplier { get; init; }

	[Serialize]
	public float Effectiveness { get; init; }

	[Serialize]
	public bool Wastable { get; init; }

	[Serialize]
	public float HoursWarningThreshold { get; init; }

	[Serialize]
	public string DisplayNameLocKey { get; init; }

	[Serialize]
	private int FavorableWellbeing { get; init; }

	[Serialize]
	private int UnfavorableWellbeing { get; init; }

	public bool IsNeverNegative => MinimumValue >= 0f;

	public bool IsNeverPositive => MaximumValue <= 0f;

	public bool AffectsWellbeing
	{
		get
		{
			if (FavorableWellbeing == 0)
			{
				return UnfavorableWellbeing != 0;
			}
			return true;
		}
	}

	public int GetFavorableWellbeing()
	{
		return GetWellbeing(isFavourable: true);
	}

	public int GetUnfavorableWellbeing()
	{
		return GetWellbeing(isFavourable: false);
	}

	private int GetWellbeing(bool isFavourable)
	{
		if (!isFavourable)
		{
			return UnfavorableWellbeing;
		}
		return FavorableWellbeing;
	}
}
