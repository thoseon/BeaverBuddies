using UnityEngine;

namespace Timberborn.Rendering;

public class MaterialHeightCutoffSetter
{
	private static readonly int HeightCutoffId = Shader.PropertyToID("_HeightCutoff");

	public void SetCutoff(Material material, float height)
	{
		material.SetFloat(HeightCutoffId, height);
	}
}
