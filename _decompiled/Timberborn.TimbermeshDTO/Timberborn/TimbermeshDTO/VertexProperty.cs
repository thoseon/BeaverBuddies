using ProtoBuf;

namespace Timberborn.TimbermeshDTO;

[ProtoContract]
public class VertexProperty
{
	[ProtoMember(1)]
	public string Name { get; }

	[ProtoMember(2)]
	public ScalarType ScalarType { get; }

	[ProtoMember(3)]
	public int ScalarTypeDimension { get; }

	[ProtoMember(4)]
	public byte[] Data { get; }
}
