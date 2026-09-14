using ProtoBuf;

namespace Timberborn.TimbermeshDTO;

[ProtoContract]
public class Model
{
	[ProtoMember(1)]
	public int Version { get; }

	[ProtoMember(2)]
	public string Name { get; }

	[ProtoMember(3)]
	public Node[] Nodes { get; }
}
