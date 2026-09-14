using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.InputSystem;

internal record CustomCursorSpec : ComponentSpec
{
	[Serialize]
	public string Id { get; init; }

	[Serialize]
	public AssetRef<Texture2D> WindowsCursor { get; init; }

	[Serialize]
	public AssetRef<Texture2D> MacOsCursor { get; init; }

	[Serialize]
	public Vector2 Hotspot { get; init; }

	[Serialize]
	public Vector2 WindowsCursorOffset { get; init; }

	[Serialize]
	public Vector2 MacOsCursorOffset { get; init; }
}
