using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.TimbermeshDTO;
using UnityEngine;

namespace Timberborn.Timbermesh;

public static class TimbermeshExtensions
{
	public static Vector3 ToVector3(this Vector3Float self)
	{
		return new Vector3(self.X, self.Y, self.Z);
	}

	public static Quaternion ToQuaternion(this QuaternionFloat self)
	{
		return new Quaternion(self.X, self.Y, self.Z, self.W);
	}

	public static void ReadProperties(this Node node, string name, ICollection<Vector2> target)
	{
		VertexProperty vertexProperty = node.VertexProperties.Get(name);
		if (vertexProperty != null)
		{
			for (int i = 0; i < node.VertexCount; i++)
			{
				target.Add(ReadFloat2(vertexProperty, i));
			}
		}
	}

	public static void ReadProperties(this Node node, string name, ICollection<Vector3> target)
	{
		VertexProperty vertexProperty = node.VertexProperties.Get(name);
		if (vertexProperty != null)
		{
			for (int i = 0; i < node.VertexCount; i++)
			{
				target.Add(ReadFloat3(vertexProperty, i));
			}
		}
	}

	public static void ReadProperties(this Node node, string name, ICollection<Vector4> target)
	{
		VertexProperty vertexProperty = node.VertexProperties.Get(name);
		if (vertexProperty != null)
		{
			for (int i = 0; i < node.VertexCount; i++)
			{
				target.Add(ReadFloat4(vertexProperty, i));
			}
		}
	}

	public static void ReadProperties(this Node node, string name, ICollection<Color> target)
	{
		VertexProperty vertexProperty = node.VertexProperties.Get(name);
		if (vertexProperty != null)
		{
			for (int i = 0; i < node.VertexCount; i++)
			{
				target.Add(ReadFloat4(vertexProperty, i));
			}
		}
	}

	public static VertexProperty Get(this IEnumerable<VertexProperty> properties, string name)
	{
		return properties.FirstOrDefault((VertexProperty p) => p.Name == name);
	}

	private static Vector2 ReadFloat2(VertexProperty property, int itemIndex)
	{
		int num = itemIndex * 8;
		float x = BitConverter.ToSingle(property.Data, num);
		float y = BitConverter.ToSingle(property.Data, num + 4);
		return new Vector2(x, y);
	}

	private static Vector3 ReadFloat3(VertexProperty property, int itemIndex)
	{
		int num = itemIndex * 12;
		float x = BitConverter.ToSingle(property.Data, num);
		float y = BitConverter.ToSingle(property.Data, num + 4);
		float z = BitConverter.ToSingle(property.Data, num + 8);
		return new Vector3(x, y, z);
	}

	private static Vector4 ReadFloat4(VertexProperty property, int itemIndex)
	{
		int num = itemIndex * 16;
		float x = BitConverter.ToSingle(property.Data, num);
		float y = BitConverter.ToSingle(property.Data, num + 4);
		float z = BitConverter.ToSingle(property.Data, num + 8);
		float w = BitConverter.ToSingle(property.Data, num + 12);
		return new Vector4(x, y, z, w);
	}
}
