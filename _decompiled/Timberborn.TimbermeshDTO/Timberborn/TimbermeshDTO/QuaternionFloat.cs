using ProtoBuf;

namespace Timberborn.TimbermeshDTO;

[ProtoContract]
public class QuaternionFloat
{
	[ProtoMember(1)]
	public float X { get; }

	[ProtoMember(2)]
	public float Y { get; }

	[ProtoMember(3)]
	public float Z { get; }

	[ProtoMember(4)]
	public float W { get; }
}
