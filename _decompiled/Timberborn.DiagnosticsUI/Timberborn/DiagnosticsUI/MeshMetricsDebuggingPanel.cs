using Timberborn.DebuggingUI;
using Timberborn.Diagnostics;
using Timberborn.SingletonSystem;

namespace Timberborn.DiagnosticsUI;

public class MeshMetricsDebuggingPanel : ILoadableSingleton, IDebuggingPanel
{
	private readonly DebuggingPanel _debuggingPanel;

	private readonly SelectedMeshMetrics _selectedMeshMetrics;

	public MeshMetricsDebuggingPanel(DebuggingPanel debuggingPanel, SelectedMeshMetrics selectedMeshMetrics)
	{
		_debuggingPanel = debuggingPanel;
		_selectedMeshMetrics = selectedMeshMetrics;
	}

	public void Load()
	{
		_debuggingPanel.AddDebuggingPanel(this, "Mesh metrics");
	}

	public string GetText()
	{
		MeshMetrics meshMetrics = _selectedMeshMetrics.MeshMetrics;
		if (meshMetrics != null)
		{
			return $"Verts: {meshMetrics.NumberOfVertices:N0}" + $"\nTris: {meshMetrics.NumberOfTriangles:N0}" + $"\nTris/tile: {meshMetrics.NumberOfTrianglesPerTile:N0}" + $"\nSubmeshes: {meshMetrics.NumberOfSubmeshes:N0}";
		}
		return "Nothing selected";
	}
}
