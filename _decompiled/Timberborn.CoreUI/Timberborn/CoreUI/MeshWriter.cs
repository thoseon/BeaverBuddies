using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.CoreUI;

internal struct MeshWriter
{
	private bool _writing;

	private MeshWriteData _meshWriteData;

	private int _indexCount;

	public int VertexCount { get; private set; }

	public MeshWriter(int vertexCount, int indexCount)
	{
		VertexCount = vertexCount;
		_indexCount = indexCount;
		_meshWriteData = null;
		_writing = false;
	}

	public void StartWriting(MeshGenerationContext meshGenerationContext, Texture texture = null)
	{
		if (_writing)
		{
			throw new InvalidOperationException("This MeshWriter is already writing.");
		}
		_writing = true;
		_meshWriteData = ((_indexCount > 0) ? meshGenerationContext.Allocate(VertexCount, _indexCount, texture) : null);
		VertexCount = 0;
		_indexCount = 0;
	}

	public void SetNextIndex(ushort index)
	{
		_indexCount++;
		if (_writing)
		{
			_meshWriteData?.SetNextIndex(index);
		}
	}

	public void SetNextVertex(Vertex vertex)
	{
		VertexCount++;
		if (_writing)
		{
			_meshWriteData?.SetNextVertex(vertex);
		}
	}
}
