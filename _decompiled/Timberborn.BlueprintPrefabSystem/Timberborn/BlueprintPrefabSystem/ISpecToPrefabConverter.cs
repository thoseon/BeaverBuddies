using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.BlueprintPrefabSystem;

public interface ISpecToPrefabConverter
{
	bool CanConvert(ComponentSpec spec);

	void Convert(GameObject owner, ComponentSpec spec);
}
