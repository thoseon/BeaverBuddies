using System.Collections.Generic;

namespace Timberborn.BlueprintSystem;

public interface ISpecService
{
	Blueprint GetBlueprint(string blueprintPath);

	T GetSingleSpec<T>() where T : ComponentSpec;

	IEnumerable<T> GetSpecs<T>() where T : ComponentSpec;
}
