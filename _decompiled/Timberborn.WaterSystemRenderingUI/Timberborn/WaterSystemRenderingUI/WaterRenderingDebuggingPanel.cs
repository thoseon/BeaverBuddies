using System;
using System.Text;
using Timberborn.Common;
using Timberborn.CursorToolSystem;
using Timberborn.DebuggingUI;
using Timberborn.SingletonSystem;
using Timberborn.WaterSystem;
using Timberborn.WaterSystemRendering;
using UnityEngine;

namespace Timberborn.WaterSystemRenderingUI;

internal class WaterRenderingDebuggingPanel : ILoadableSingleton, IUnloadableSingleton, IDebuggingPanel
{
	private static readonly int TickProgress = Shader.PropertyToID("_TickProgress");

	private readonly DebuggingPanel _debuggingPanel;

	private readonly CursorDebugger _cursorDebugger;

	private readonly IThreadSafeWaterMap _threadSafeWaterMap;

	private readonly StringBuilder _stringBuilder = new StringBuilder();

	private Texture _oldWaterData;

	private Texture _newWaterData;

	private Texture _oldOutflows;

	private Texture _newOutflows;

	private Texture _oldEdgeLinks;

	private Texture _newEdgeLinks;

	private Texture _oldCornerLinks;

	private Texture _newCornerLinks;

	private Texture _oldSkirts;

	private Texture _newSkirts;

	private Texture _oldContaminations;

	private Texture _newContaminations;

	private Texture2D _byteSamplingTexture;

	private RenderTexture _byteSamplingRenderTexture;

	private Texture2D _vector2SamplingTexture;

	private RenderTexture _vector2SamplingRenderTexture;

	private Texture2D _vector4SamplingTexture;

	private RenderTexture _vector4SamplingRenderTexture;

	private Texture2D _colorSamplingTexture;

	private RenderTexture _colorSamplingRenderTexture;

	public WaterRenderingDebuggingPanel(DebuggingPanel debuggingPanel, CursorDebugger cursorDebugger, IThreadSafeWaterMap threadSafeWaterMap)
	{
		_debuggingPanel = debuggingPanel;
		_cursorDebugger = cursorDebugger;
		_threadSafeWaterMap = threadSafeWaterMap;
	}

	public void Load()
	{
		_debuggingPanel.AddDebuggingPanel(this, "Water rendering data");
		_byteSamplingTexture = new Texture2D(1, 1, TextureFormat.R8, mipChain: false, linear: true);
		_byteSamplingRenderTexture = new RenderTexture(1, 1, 0, RenderTextureFormat.R8)
		{
			enableRandomWrite = true
		};
		_vector2SamplingTexture = new Texture2D(1, 1, TextureFormat.RGFloat, mipChain: false, linear: true);
		_vector2SamplingRenderTexture = new RenderTexture(1, 1, 0, RenderTextureFormat.RGFloat)
		{
			enableRandomWrite = true
		};
		_vector4SamplingTexture = new Texture2D(1, 1, TextureFormat.RGBAFloat, mipChain: false, linear: true);
		_vector4SamplingRenderTexture = new RenderTexture(1, 1, 0, RenderTextureFormat.ARGBFloat)
		{
			enableRandomWrite = true
		};
		_colorSamplingTexture = new Texture2D(1, 1, TextureFormat.ARGB32, mipChain: false, linear: true);
		_colorSamplingRenderTexture = new RenderTexture(1, 1, 0, RenderTextureFormat.ARGB32)
		{
			enableRandomWrite = true
		};
	}

	public void Unload()
	{
		UnityEngine.Object.Destroy(_byteSamplingRenderTexture);
		UnityEngine.Object.Destroy(_byteSamplingTexture);
		UnityEngine.Object.Destroy(_vector2SamplingRenderTexture);
		UnityEngine.Object.Destroy(_vector2SamplingTexture);
		UnityEngine.Object.Destroy(_vector4SamplingRenderTexture);
		UnityEngine.Object.Destroy(_vector4SamplingTexture);
		UnityEngine.Object.Destroy(_colorSamplingRenderTexture);
		UnityEngine.Object.Destroy(_colorSamplingTexture);
	}

	public string GetText()
	{
		_stringBuilder.Clear();
		if (_cursorDebugger.Active)
		{
			_stringBuilder.AppendLine($"Tick progress: {Shader.GetGlobalFloat(TickProgress):0.0000}");
			GetGlobalTextures();
			SampleGlobalTextures();
		}
		return _stringBuilder.ToStringWithoutNewLineEnd();
	}

	private void GetGlobalTextures()
	{
		_oldWaterData = Shader.GetGlobalTexture(WaterTextureNames.OldWaterData);
		_newWaterData = Shader.GetGlobalTexture(WaterTextureNames.NewWaterData);
		_oldOutflows = Shader.GetGlobalTexture(WaterTextureNames.OldOutflows);
		_newOutflows = Shader.GetGlobalTexture(WaterTextureNames.NewOutflows);
		_oldEdgeLinks = Shader.GetGlobalTexture(WaterTextureNames.OldEdgeLinks);
		_newEdgeLinks = Shader.GetGlobalTexture(WaterTextureNames.NewEdgeLinks);
		_oldCornerLinks = Shader.GetGlobalTexture(WaterTextureNames.OldCornerLinks);
		_newCornerLinks = Shader.GetGlobalTexture(WaterTextureNames.NewCornerLinks);
		_oldSkirts = Shader.GetGlobalTexture(WaterTextureNames.OldSkirts);
		_newSkirts = Shader.GetGlobalTexture(WaterTextureNames.NewSkirts);
		_oldContaminations = Shader.GetGlobalTexture(WaterTextureNames.OldContaminations);
		_newContaminations = Shader.GetGlobalTexture(WaterTextureNames.NewContaminations);
	}

	private void SampleGlobalTextures()
	{
		RenderTexture active = RenderTexture.active;
		Vector2Int coords = _cursorDebugger.Coordinates.XY();
		for (int i = 0; i < _threadSafeWaterMap.MaxColumnCount; i++)
		{
			SampleTextureSet(coords, i, _oldWaterData, _newWaterData, _oldOutflows, _newOutflows, _oldEdgeLinks, _newEdgeLinks, _oldCornerLinks, _newCornerLinks, _oldSkirts, _newSkirts, _oldContaminations, _newContaminations);
		}
		RenderTexture.active = active;
	}

	private void SampleTextureSet(Vector2Int coords, int columnIndex, Texture oldWaterDataTexture, Texture newWaterDataTexture, Texture oldOutflowsTexture, Texture newOutflowsTexture, Texture oldEdgeLinksTexture, Texture newEdgeLinksTexture, Texture oldCornerLinksTexture, Texture newCornerLinksTexture, Texture oldSkirtVisibilityTexture, Texture newSkirtVisibilityTexture, Texture oldContaminationsTexture, Texture newContaminationsTexture)
	{
		Vector4 vector = SampleVector4Texture(oldWaterDataTexture, columnIndex, coords);
		Vector4 vector2 = SampleVector4Texture(newWaterDataTexture, columnIndex, coords);
		Vector2 input = SampleVector2Texture(oldOutflowsTexture, columnIndex, coords);
		Vector2 input2 = SampleVector2Texture(newOutflowsTexture, columnIndex, coords);
		Vector4 input3 = SampleVector4Texture(oldEdgeLinksTexture, columnIndex, coords);
		Vector4 input4 = SampleVector4Texture(newEdgeLinksTexture, columnIndex, coords);
		float num = SampleByteTexture(oldContaminationsTexture, columnIndex, coords);
		float num2 = SampleByteTexture(newContaminationsTexture, columnIndex, coords);
		if (vector.x > 0f || vector2.x > 0f || HasAnyConnection(input3) || HasAnyConnection(input4) || num > 0f || num2 > 0f || Math.Abs(input.x) > 0f || Math.Abs(input.y) > 0f || Math.Abs(input2.x) > 0f || Math.Abs(input2.y) > 0f)
		{
			Vector4 input5 = SampleVector4Texture(oldCornerLinksTexture, columnIndex, coords);
			Color32 input6 = SampleColorTexture(oldSkirtVisibilityTexture, columnIndex, coords);
			_stringBuilder.AppendLine($"Column index: {columnIndex}");
			_stringBuilder.AppendLine("Old:");
			_stringBuilder.AppendLine($"Depth: {vector.x:0.000000}, " + $"Floor: {vector.y:0.} Ceiling: {vector.z:0.}");
			_stringBuilder.AppendLine("Edge: " + FormatEdge(input3));
			_stringBuilder.AppendLine("Corner: " + FormatCorner(input5));
			_stringBuilder.AppendLine("Skirts: " + FormatSkirts(input6));
			_stringBuilder.AppendLine($"Contamination: {num:0.000}");
			_stringBuilder.AppendLine("Outflows: " + FormatOutflows(input));
			Vector4 input7 = SampleVector4Texture(newCornerLinksTexture, columnIndex, coords);
			Color32 input8 = SampleColorTexture(newSkirtVisibilityTexture, columnIndex, coords);
			_stringBuilder.AppendLine("New:");
			_stringBuilder.AppendLine($"Depth: {vector2.x:0.000000}, " + $"Floor: {vector2.y:0.} Ceiling: {vector2.z:0.}");
			_stringBuilder.AppendLine("Edge: " + FormatEdge(input4));
			_stringBuilder.AppendLine("Corner: " + FormatCorner(input7));
			_stringBuilder.AppendLine("Skirts: " + FormatSkirts(input8));
			_stringBuilder.AppendLine($"Contamination: {num2:0.000}");
			_stringBuilder.AppendLine("Outflows: " + FormatOutflows(input2));
		}
	}

	private float SampleByteTexture(Texture sourceTexture, int layer, Vector2Int coordinates)
	{
		Graphics.CopyTexture(sourceTexture, layer, 0, coordinates.x, coordinates.y, 1, 1, _byteSamplingRenderTexture, 0, 0, 0, 0);
		RenderTexture.active = _byteSamplingRenderTexture;
		_byteSamplingTexture.ReadPixels(new Rect(0f, 0f, 1f, 1f), 0, 0, recalculateMipMaps: false);
		return (float)(int)_byteSamplingTexture.GetRawTextureData<byte>()[0] / 255f;
	}

	private Vector2 SampleVector2Texture(Texture sourceTexture, int layer, Vector2Int coordinates)
	{
		Graphics.CopyTexture(sourceTexture, layer, 0, coordinates.x, coordinates.y, 1, 1, _vector2SamplingRenderTexture, 0, 0, 0, 0);
		RenderTexture.active = _vector2SamplingRenderTexture;
		_vector2SamplingTexture.ReadPixels(new Rect(0f, 0f, 1f, 1f), 0, 0, recalculateMipMaps: false);
		return _vector2SamplingTexture.GetRawTextureData<Vector2>()[0];
	}

	private Vector4 SampleVector4Texture(Texture sourceTexture, int layer, Vector2Int coordinates)
	{
		Graphics.CopyTexture(sourceTexture, layer, 0, coordinates.x, coordinates.y, 1, 1, _vector4SamplingRenderTexture, 0, 0, 0, 0);
		RenderTexture.active = _vector4SamplingRenderTexture;
		_vector4SamplingTexture.ReadPixels(new Rect(0f, 0f, 1f, 1f), 0, 0, recalculateMipMaps: false);
		return _vector4SamplingTexture.GetRawTextureData<Vector4>()[0];
	}

	private Color32 SampleColorTexture(Texture sourceTexture, int layer, Vector2Int coordinates)
	{
		Graphics.CopyTexture(sourceTexture, layer, 0, coordinates.x, coordinates.y, 1, 1, _colorSamplingRenderTexture, 0, 0, 0, 0);
		RenderTexture.active = _colorSamplingRenderTexture;
		_colorSamplingTexture.ReadPixels(new Rect(0f, 0f, 1f, 1f), 0, 0, recalculateMipMaps: false);
		return _colorSamplingTexture.GetRawTextureData<Color32>()[0];
	}

	private static bool HasAnyConnection(Vector4 input)
	{
		if (!(input.x >= 0f) && !(input.y >= 0f) && !(input.z >= 0f))
		{
			return input.w >= 0f;
		}
		return true;
	}

	private static string FormatEdge(Vector4 input)
	{
		return $"T: {input.x:0.}, L: {input.y:0.}, B: {input.z:0.}, R: {input.w:0.}";
	}

	private static string FormatCorner(Vector4 input)
	{
		return $"TL: {input.x:0.}, TR: {input.y:0.}, BL: {input.z:0.}, BR: {input.w:0.}";
	}

	private static string FormatSkirts(Color32 input)
	{
		return $"T: {input.r}, L: {input.g}, B: {input.b}, R: {input.a}";
	}

	private static string FormatOutflows(Vector2 input)
	{
		return $"H: {input.x:0.000}, V: {input.y:0.000}";
	}
}
