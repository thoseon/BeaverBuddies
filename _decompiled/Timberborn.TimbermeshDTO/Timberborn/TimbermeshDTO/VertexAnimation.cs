using System.Collections.Generic;
using JetBrains.Annotations;
using ProtoBuf;

namespace Timberborn.TimbermeshDTO;

[UsedImplicitly]
[ProtoContract]
public class VertexAnimation : IAnimation
{
	[ProtoMember(1)]
	public string Name { get; }

	[ProtoMember(2)]
	public float Framerate { get; }

	[ProtoMember(3)]
	public int AnimatedVertexCount { get; }

	[ProtoMember(4)]
	public List<VertexAnimationFrame> Frames { get; } = new List<VertexAnimationFrame>();

	public float Length => (float)Frames.Count / Framerate;
}
