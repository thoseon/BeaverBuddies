using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using UnityEngine;

namespace Timberborn.Diagnostics;

public class MeshMetricsRetriever
{
	private class MutableMetrics
	{
		public int NumberOfVertices { get; set; }

		public int NumberOfTriangles { get; set; }

		public int NumberOfSubmeshes { get; set; }
	}

	public MeshMetrics GetMeshMetrics(GameObject root)
	{
		MutableMetrics mutableMetrics = new MutableMetrics();
		Visit(root, mutableMetrics);
		BlockObject componentSlow = root.GetComponentSlow<BlockObject>();
		int? numberOfTrianglesPerTile = (componentSlow ? new int?(mutableMetrics.NumberOfTriangles / NumberOfTiles(componentSlow)) : ((int?)null));
		string name = root.name;
		int numberOfVertices = mutableMetrics.NumberOfVertices;
		int numberOfTriangles = mutableMetrics.NumberOfTriangles;
		int numberOfSubmeshes = mutableMetrics.NumberOfSubmeshes;
		return new MeshMetrics(name, numberOfVertices, numberOfTriangles, numberOfTrianglesPerTile, numberOfSubmeshes);
	}

	private static void Visit(GameObject gameObject, MutableMetrics mutableMetrics)
	{
		if (!gameObject.activeSelf || NameIsIgnored(gameObject.name))
		{
			return;
		}
		SkinnedMeshRenderer component2;
		if (gameObject.TryGetComponent<MeshFilter>(out var component))
		{
			if (component.GetComponent<MeshRenderer>().enabled)
			{
				CountMesh(component.sharedMesh, mutableMetrics);
			}
		}
		else if (gameObject.TryGetComponent<SkinnedMeshRenderer>(out component2) && component2.enabled)
		{
			CountMesh(component2.sharedMesh, mutableMetrics);
		}
		VisitChildren(gameObject, mutableMetrics);
	}

	private static void VisitChildren(GameObject gameObject, MutableMetrics mutableMetrics)
	{
		int childCount = gameObject.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Visit(gameObject.transform.GetChild(i).gameObject, mutableMetrics);
		}
	}

	private static void CountMesh(Mesh mesh, MutableMetrics mutableMetrics)
	{
		mutableMetrics.NumberOfVertices += mesh.vertexCount;
		mutableMetrics.NumberOfTriangles += mesh.triangles.Length / 3;
		mutableMetrics.NumberOfSubmeshes += mesh.subMeshCount;
	}

	private static int NumberOfTiles(BlockObject blockObject)
	{
		Vector3Int[] array = blockObject.Blocks.GetOccupiedCoordinates().ToArray();
		return (array.Any() ? array : blockObject.Blocks.GetAllCoordinates().ToArray()).Select((Vector3Int coords) => coords.XY()).Distinct().Count();
	}

	private static bool NameIsIgnored(string name)
	{
		if (!name.Contains("Marker") && !name.Contains("StatusIcon"))
		{
			return name.Contains("#Unfinished");
		}
		return true;
	}
}
