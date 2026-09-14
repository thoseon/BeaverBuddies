using System.Collections.Generic;
using ProtoBuf;

namespace Timberborn.TimbermeshDTO;

[ProtoContract]
public class Node
{
	[ProtoMember(1)]
	public int Parent { get; }

	[ProtoMember(2)]
	public string Name { get; }

	[ProtoMember(3)]
	public Vector3Float Position { get; } = new Vector3Float();

	[ProtoMember(4)]
	public QuaternionFloat Rotation { get; } = new QuaternionFloat();

	[ProtoMember(5)]
	public Vector3Float Scale { get; } = new Vector3Float();

	[ProtoMember(6)]
	public int VertexCount { get; }

	[ProtoMember(7)]
	public List<VertexProperty> VertexProperties { get; } = new List<VertexProperty>();

	[ProtoMember(8)]
	public List<Mesh> Meshes { get; } = new List<Mesh>();

	[ProtoMember(9)]
	public List<VertexAnimation> VertexAnimations { get; } = new List<VertexAnimation>();

	[ProtoMember(10)]
	public List<NodeAnimation> NodeAnimations { get; } = new List<NodeAnimation>();
}
