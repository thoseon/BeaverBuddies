using Timberborn.BlueprintSystem;

namespace Timberborn.MortalSystem;

internal record DeadStatusSpec : ComponentSpec
{
	[Serialize]
	public string DiedOldAgeStatusLocKey { get; init; }

	[Serialize]
	public string DiedTragicallyStatusLocKey { get; init; }

	[Serialize]
	public string DiedTragicallyAlertLocKey { get; init; }
}
