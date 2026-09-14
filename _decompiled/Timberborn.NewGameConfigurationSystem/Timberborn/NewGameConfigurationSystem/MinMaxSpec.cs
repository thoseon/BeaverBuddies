using Timberborn.BlueprintSystem;

namespace Timberborn.NewGameConfigurationSystem;

public record MinMaxSpec<T> : ComponentSpec
{
	[Serialize]
	public T Min { get; init; }

	[Serialize]
	public T Max { get; init; }

	public override string ToString()
	{
		return $"{Min} - {Max}";
	}
}
