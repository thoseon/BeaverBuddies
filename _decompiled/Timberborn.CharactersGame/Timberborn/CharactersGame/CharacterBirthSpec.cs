using Timberborn.BlueprintSystem;

namespace Timberborn.CharactersGame;

internal record CharacterBirthSpec : ComponentSpec
{
	[Serialize]
	public string NotificationLocKey { get; init; }
}
