using Timberborn.BlueprintSystem;

namespace Timberborn.Bots;

internal record BotColorsSpec : ComponentSpec
{
	[Serialize]
	public string BotColorId { get; init; }
}
