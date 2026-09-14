using System.Collections.Generic;
using JetBrains.Annotations;
using ProtoBuf;

namespace Timberborn.TimbermeshDTO;

[UsedImplicitly]
[ProtoContract]
public class VertexAnimationFrame
{
	[ProtoMember(1)]
	public List<VertexProperty> VertexProperties { get; } = new List<VertexProperty>();
}
