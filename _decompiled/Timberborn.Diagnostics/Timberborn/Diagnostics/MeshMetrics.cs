namespace Timberborn.Diagnostics;

public class MeshMetrics
{
	public string Name { get; }

	public int NumberOfVertices { get; }

	public int NumberOfTriangles { get; }

	public int? NumberOfTrianglesPerTile { get; }

	public int NumberOfSubmeshes { get; }

	public MeshMetrics(string name, int numberOfVertices, int numberOfTriangles, int? numberOfTrianglesPerTile, int numberOfSubmeshes)
	{
		Name = name;
		NumberOfVertices = numberOfVertices;
		NumberOfTriangles = numberOfTriangles;
		NumberOfTrianglesPerTile = numberOfTrianglesPerTile;
		NumberOfSubmeshes = numberOfSubmeshes;
	}

	public override string ToString()
	{
		return "Name: " + Name + string.Format(", {0}: {1}", "NumberOfVertices", NumberOfVertices) + string.Format(", {0}: {1}", "NumberOfTriangles", NumberOfTriangles) + string.Format(", {0}: {1}", "NumberOfTrianglesPerTile", NumberOfTrianglesPerTile) + string.Format(", {0}: {1}", "NumberOfSubmeshes", NumberOfSubmeshes);
	}
}
