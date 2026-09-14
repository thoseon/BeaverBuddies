using System;
using System.IO;
using System.Linq;
using System.Text;
using Timberborn.Debugging;
using Timberborn.FileSystem;
using Timberborn.PlatformUtilities;
using Timberborn.PrefabOptimization;
using UnityEngine;

namespace Timberborn.Diagnostics;

public class MeshMetricsDumper : IDevModule
{
	private readonly MeshMetricsRetriever _meshMetricsRetriever;

	private readonly IPrefabOptimizationChain _prefabOptimizationChain;

	private readonly IFileService _fileService;

	private readonly IExplorerOpener _explorerOpener;

	public MeshMetricsDumper(MeshMetricsRetriever meshMetricsRetriever, IPrefabOptimizationChain prefabOptimizationChain, IFileService fileService, IExplorerOpener explorerOpener)
	{
		_meshMetricsRetriever = meshMetricsRetriever;
		_prefabOptimizationChain = prefabOptimizationChain;
		_fileService = fileService;
		_explorerOpener = explorerOpener;
	}

	public DevModuleDefinition GetDefinition()
	{
		return new DevModuleDefinition.Builder().AddMethod(DevMethod.Create("Dump mesh metrics", DumpMeshMetrics)).Build();
	}

	private void DumpMeshMetrics()
	{
		string text = DumpMeshMetricsToString();
		string text2 = CreateFilePath();
		string directoryName = Path.GetDirectoryName(text2);
		_fileService.WriteTextToFile(text2, text);
		Debug.Log("Dumped mesh metrics to " + text2);
		_explorerOpener.OpenDirectory(directoryName);
	}

	private string DumpMeshMetricsToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		AppendHeader(stringBuilder);
		AppendMetrics(stringBuilder);
		return stringBuilder.ToString();
	}

	private void AppendMetrics(StringBuilder meshMetricsDescription)
	{
		foreach (MeshMetrics item in from prefab in _prefabOptimizationChain.GetCached()
			select _meshMetricsRetriever.GetMeshMetrics(prefab) into prefab
			orderby prefab.NumberOfTriangles descending
			select prefab)
		{
			meshMetricsDescription.Append(item.Name);
			meshMetricsDescription.Append(",");
			meshMetricsDescription.Append(item.NumberOfVertices);
			meshMetricsDescription.Append(",");
			meshMetricsDescription.Append(item.NumberOfTriangles);
			meshMetricsDescription.Append(",");
			meshMetricsDescription.Append(item.NumberOfTrianglesPerTile);
			meshMetricsDescription.Append(",");
			meshMetricsDescription.Append(item.NumberOfSubmeshes);
			meshMetricsDescription.AppendLine();
		}
	}

	private static void AppendHeader(StringBuilder meshMetricsDescription)
	{
		meshMetricsDescription.Append("Name");
		meshMetricsDescription.Append(",");
		meshMetricsDescription.Append("NumberOfVertices");
		meshMetricsDescription.Append(",");
		meshMetricsDescription.Append("NumberOfTriangles");
		meshMetricsDescription.Append(",");
		meshMetricsDescription.Append("NumberOfTrianglesPerTile");
		meshMetricsDescription.Append(",");
		meshMetricsDescription.Append("NumberOfSubmeshes");
		meshMetricsDescription.AppendLine();
	}

	private static string CreateFilePath()
	{
		string text = DateTime.Now.ToString("yyyy-MM-dd HH\\hmm\\mss\\s");
		string path = "MeshMetrics " + text + ".csv";
		return Path.Combine(UserDataFolder.Folder, "MeshMetrics", path);
	}
}
