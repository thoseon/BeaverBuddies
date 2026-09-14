using UnityEngine;

namespace Timberborn.Rendering;

public static class Layers
{
	private static readonly string UIName = "UI";

	public static readonly int UILayer = LayerMask.NameToLayer(UIName);

	public static readonly LayerMask UIMask = LayerMask.GetMask(UIName);

	public static readonly LayerMask IgnoreRaycastMask = LayerMask.NameToLayer("Ignore Raycast");
}
