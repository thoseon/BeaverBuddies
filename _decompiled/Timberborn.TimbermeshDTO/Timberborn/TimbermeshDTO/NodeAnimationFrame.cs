using JetBrains.Annotations;
using ProtoBuf;

namespace Timberborn.TimbermeshDTO;

[UsedImplicitly]
[ProtoContract]
public class NodeAnimationFrame
{
	[ProtoMember(1)]
	public Vector3Float Position { get; }

	[ProtoMember(2)]
	public QuaternionFloat Rotation { get; }

	[ProtoMember(3)]
	public Vector3Float Scale { get; }
}
