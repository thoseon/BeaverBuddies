using System;
using Timberborn.BlueprintPrefabSystem;
using Timberborn.BlueprintSystem;
using Timberborn.Coordinates;
using UnityEngine;

namespace Timberborn.UnityEngineSpecs;

internal class CollidersSpecPrefabConverter : ISpecToPrefabConverter
{
	public bool CanConvert(ComponentSpec spec)
	{
		return spec is CollidersSpec;
	}

	public void Convert(GameObject owner, ComponentSpec spec)
	{
		CollidersSpec collidersSpec = (CollidersSpec)spec;
		foreach (BoxColliderSpec boxCollider2 in collidersSpec.BoxColliders)
		{
			BoxCollider boxCollider = owner.AddComponent<BoxCollider>();
			boxCollider.center = boxCollider2.Center;
			boxCollider.size = boxCollider2.Size;
		}
		foreach (SphereColliderSpec sphereCollider2 in collidersSpec.SphereColliders)
		{
			SphereCollider sphereCollider = owner.AddComponent<SphereCollider>();
			sphereCollider.center = sphereCollider2.Center;
			sphereCollider.radius = sphereCollider2.Radius;
		}
		foreach (CapsuleColliderSpec capsuleCollider2 in collidersSpec.CapsuleColliders)
		{
			CapsuleCollider capsuleCollider = owner.AddComponent<CapsuleCollider>();
			capsuleCollider.center = capsuleCollider2.Center;
			capsuleCollider.radius = capsuleCollider2.Radius;
			capsuleCollider.height = capsuleCollider2.Height;
			capsuleCollider.direction = GetDirection(capsuleCollider2.Axis);
		}
	}

	private static int GetDirection(Axis axis)
	{
		return axis switch
		{
			Axis.X => 0, 
			Axis.Y => 1, 
			Axis.Z => 2, 
			_ => throw new ArgumentOutOfRangeException("axis", axis, null), 
		};
	}
}
