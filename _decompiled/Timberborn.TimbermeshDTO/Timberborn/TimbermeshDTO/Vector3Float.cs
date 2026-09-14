using ProtoBuf;

namespace Timberborn.TimbermeshDTO;

[ProtoContract]
public class Vector3Float
{
	[ProtoMember(1)]
	public float X { get; }

	[ProtoMember(2)]
	public float Y { get; }

	[ProtoMember(3)]
	public float Z { get; }
}
