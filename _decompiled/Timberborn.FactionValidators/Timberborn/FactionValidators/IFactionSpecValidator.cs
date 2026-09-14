using Timberborn.FactionSystem;

namespace Timberborn.FactionValidators;

internal interface IFactionSpecValidator
{
	bool IsValid(FactionSpec faction, out string errorMessage);
}
