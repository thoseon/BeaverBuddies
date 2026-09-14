using Timberborn.BlueprintPrefabSystem;
using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.UnityEngineSpecs;

internal class TransformSpecPrefabConverter : ISpecToPrefabConverter
{
	public bool CanConvert(ComponentSpec spec)
	{
		return spec is TransformSpec;
	}

	public void Convert(GameObject owner, ComponentSpec spec)
	{
		TransformSpec transformSpec = (TransformSpec)spec;
		owner.transform.SetLocalPositionAndRotation(transformSpec.Position, Quaternion.Euler(transformSpec.Rotation));
		owner.transform.localScale = transformSpec.Scale;
	}
}
