namespace Timberborn.BlueprintSystem;

public record NestedBlueprintSpec : ComponentSpec
{
	[Serialize]
	public string BlueprintPath { get; init; }

	[Serialize]
	public string Modification { get; init; }

	public static readonly string BlueprintPathKey = "BlueprintPath";

	public static readonly string ModificationKey = "Modification";
}
