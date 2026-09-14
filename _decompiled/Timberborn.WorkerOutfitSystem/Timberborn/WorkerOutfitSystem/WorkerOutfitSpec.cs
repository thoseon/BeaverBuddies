using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.WorkerOutfitSystem;

internal record WorkerOutfitSpec : ComponentSpec
{
	[Serialize]
	public string Id { get; init; }

	[Serialize]
	public string FactionId { get; init; }

	[Serialize]
	public string WorkerType { get; init; }

	[Serialize]
	public AssetRef<Texture2D> DiffuseTexture { get; init; }

	[Serialize]
	public AssetRef<Texture2D> NormalTexture { get; init; }

	[Serialize]
	public ImmutableArray<string> Attachments { get; init; }
}
