using System;
using UnityEngine;

namespace Timberborn.TimbermeshEditorTools;

[Serializable]
public class NodeMetadata
{
	[SerializeField]
	private string _name;

	[SerializeField]
	private int _treeDepth;

	[SerializeField]
	private int _vertexCount;

	public string Name => _name;

	public int TreeDepth => _treeDepth;

	public int VertexCount => _vertexCount;

	public NodeMetadata(string name, int treeDepth, int vertexCount)
	{
		_name = name;
		_treeDepth = treeDepth;
		_vertexCount = vertexCount;
	}
}
