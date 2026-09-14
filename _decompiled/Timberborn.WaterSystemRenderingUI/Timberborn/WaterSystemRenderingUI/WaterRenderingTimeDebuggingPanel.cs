using System.Text;
using Timberborn.Common;
using Timberborn.DebuggingUI;
using Timberborn.SingletonSystem;
using Timberborn.WaterSystemRendering;

namespace Timberborn.WaterSystemRenderingUI;

internal class WaterRenderingTimeDebuggingPanel : ILoadableSingleton, IDebuggingPanel
{
	private readonly IWaterRenderer _waterRenderer;

	private readonly DebuggingPanel _debuggingPanel;

	private readonly StringBuilder _stringBuilder = new StringBuilder();

	public WaterRenderingTimeDebuggingPanel(IWaterRenderer waterRenderer, DebuggingPanel debuggingPanel)
	{
		_debuggingPanel = debuggingPanel;
		_waterRenderer = waterRenderer;
	}

	public void Load()
	{
		_debuggingPanel.AddDebuggingPanel(this, "Water rendering times");
	}

	public string GetText()
	{
		_stringBuilder.Clear();
		_stringBuilder.AppendLine($"Update mesh: {_waterRenderer.UpdateMeshTime}ms");
		_stringBuilder.AppendLine($"Update textures: {_waterRenderer.UpdateTexturesTime}ms");
		return _stringBuilder.ToStringWithoutNewLineEnd();
	}
}
