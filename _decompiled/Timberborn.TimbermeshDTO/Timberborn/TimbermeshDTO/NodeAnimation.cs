using System.Collections.Generic;
using JetBrains.Annotations;
using ProtoBuf;

namespace Timberborn.TimbermeshDTO;

[UsedImplicitly]
[ProtoContract]
public class NodeAnimation : IAnimation
{
	[ProtoMember(1)]
	public string Name { get; }

	[ProtoMember(2)]
	public float Framerate { get; }

	[ProtoMember(3)]
	public List<NodeAnimationFrame> Frames { get; } = new List<NodeAnimationFrame>();

	public float Length => (float)Frames.Count / Framerate;
}
