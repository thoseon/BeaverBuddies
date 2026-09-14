using System.Collections.Generic;
using ProtoBuf;

namespace Timberborn.TimbermeshDTO;

[ProtoContract]
public class Mesh
{
	[ProtoMember(1)]
	public List<int> Indices { get; } = new List<int>();

	[ProtoMember(2)]
	public string Material { get; }
}
