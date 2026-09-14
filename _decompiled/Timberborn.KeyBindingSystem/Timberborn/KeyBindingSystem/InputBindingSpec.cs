using Timberborn.BlueprintSystem;

namespace Timberborn.KeyBindingSystem;

public record InputBindingSpec : ComponentSpec
{
	[Serialize]
	public string Path { get; init; }

	[Serialize]
	public InputModifiers InputModifiers { get; init; }

	[Serialize]
	public bool Unchangeable { get; init; }

	public bool IsDefined => !string.IsNullOrEmpty(Path);
}
