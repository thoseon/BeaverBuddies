using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.TemplateAttachmentSystem;

internal record AttachmentDefinition
{
	[Serialize]
	public string AttachmentId { get; init; }

	[Serialize]
	public AssetRef<GameObject> Prefab { get; init; }

	[Serialize]
	public string Parent { get; init; }

	[Serialize]
	public Vector3 Position { get; init; }

	[Serialize]
	public Vector3 Rotation { get; init; }

	[Serialize]
	public Vector3 Scale { get; init; } = Vector3.one;

	[Serialize]
	public bool CreateInstantly { get; init; } = true;
}
