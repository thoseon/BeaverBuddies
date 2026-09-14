using Timberborn.BlueprintPrefabSystem;
using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.Timbermesh;

internal class TimbermeshSpecConverter : ISpecToPrefabConverter
{
	public bool CanConvert(ComponentSpec spec)
	{
		return spec is TimbermeshSpec;
	}

	public void Convert(GameObject owner, ComponentSpec spec)
	{
		owner.AddComponent<TimbermeshDescription>().SetModelName(((TimbermeshSpec)spec).Model.Path);
	}
}
