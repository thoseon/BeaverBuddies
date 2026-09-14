using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.CoreUI;

[UxmlElement]
public class DoubleSidedProgressBar : VisualElement
{
	[Serializable]
	[CompilerGenerated]
	public new class UxmlSerializedData : VisualElement.UxmlSerializedData
	{
		public override object CreateInstance()
		{
			return new DoubleSidedProgressBar();
		}
	}

	private static readonly CustomStyleProperty<string> BackgroundImageProperty = new CustomStyleProperty<string>("--background-image");

	private Sprite _image;

	private int _minimumLengthPx;

	private float _min;

	private float _max;

	private float _progress;

	public DoubleSidedProgressBar()
	{
		base.generateVisualContent = (Action<MeshGenerationContext>)Delegate.Combine(base.generateVisualContent, new Action<MeshGenerationContext>(OnGenerateVisualContent));
		RegisterCallback<CustomStyleResolvedEvent>(OnCustomStyleResolved);
	}

	public void SetMinimumLength(int minimumLengthPx)
	{
		_minimumLengthPx = minimumLengthPx;
	}

	public void UpdateProgress(float progress, float min, float max)
	{
		if (!Mathf.Approximately(_progress, progress) || !Mathf.Approximately(_min, min) || !Mathf.Approximately(_max, max))
		{
			_progress = progress;
			_min = min;
			_max = max;
			MarkDirtyRepaint();
		}
	}

	private void OnCustomStyleResolved(CustomStyleResolvedEvent e)
	{
		_image = (base.customStyle.TryGetValue(BackgroundImageProperty, out var value) ? Resources.Load<Sprite>(value) : null);
	}

	private void OnGenerateVisualContent(MeshGenerationContext mgc)
	{
		if (_image != null)
		{
			MeshWriter meshWriter = new MeshWriter(4, 6);
			meshWriter.StartWriting(mgc, _image.texture);
			WriteMesh(ref meshWriter);
		}
	}

	private void WriteMesh(ref MeshWriter meshWriter)
	{
		Rect rect = base.paddingRect;
		float width = rect.width;
		float height = rect.height;
		if (width > 0.001f && height > 0.001f)
		{
			CalculateBarParameters(width, out var startWidth, out var endWidth, out var progress);
			int vertexCount = meshWriter.VertexCount;
			meshWriter.SetNextVertex(CreateVertex(startWidth, 0f, 0f, 1f));
			meshWriter.SetNextVertex(CreateVertex(endWidth, 0f, progress, 1f));
			meshWriter.SetNextVertex(CreateVertex(endWidth, height, progress, 0f));
			meshWriter.SetNextVertex(CreateVertex(startWidth, height, 0f, 0f));
			meshWriter.SetNextIndex((ushort)vertexCount);
			meshWriter.SetNextIndex((ushort)(vertexCount + 1));
			meshWriter.SetNextIndex((ushort)(vertexCount + 2));
			meshWriter.SetNextIndex((ushort)(vertexCount + 2));
			meshWriter.SetNextIndex((ushort)(vertexCount + 3));
			meshWriter.SetNextIndex((ushort)vertexCount);
		}
	}

	private void CalculateBarParameters(float rectWidth, out float startWidth, out float endWidth, out float progress)
	{
		startWidth = 0f;
		endWidth = 0f;
		progress = 0f;
		if (_progress != 0f)
		{
			progress = Mathf.Clamp01(Mathf.InverseLerp(_min, _max, _progress));
			if (IsDoubleSided())
			{
				CalculateDoubleSidedWidths(rectWidth, progress, out startWidth, out endWidth);
			}
			else
			{
				endWidth = CalculateSingleSidedWidth(rectWidth, progress);
			}
		}
	}

	private bool IsDoubleSided()
	{
		if (_min != 0f && _max != 0f)
		{
			return Math.Sign(_min) != Math.Sign(_max);
		}
		return false;
	}

	private void CalculateDoubleSidedWidths(float rectWidth, float progress, out float startWidth, out float endWidth)
	{
		float num = Mathf.Clamp01(Mathf.InverseLerp(_min, _max, 0f));
		bool num2 = progress > num;
		float val = rectWidth * Math.Abs(progress - num);
		startWidth = rectWidth * num;
		endWidth = startWidth + Math.Max(_minimumLengthPx, val);
		if (!num2)
		{
			FlipDoubleSidedWidths(ref startWidth, ref endWidth);
		}
	}

	private static void FlipDoubleSidedWidths(ref float startWidth, ref float endWidth)
	{
		endWidth = 2f * startWidth - endWidth;
		float num = endWidth;
		float num2 = startWidth;
		startWidth = num;
		endWidth = num2;
	}

	private float CalculateSingleSidedWidth(float rectWidth, float progress)
	{
		float val = ((_progress < 0f) ? (rectWidth * (1f - progress)) : (rectWidth * progress));
		return Math.Max(_minimumLengthPx, val);
	}

	private static Vertex CreateVertex(float x, float y, float uvX, float uvY)
	{
		return new Vertex
		{
			position = new Vector3(x, y, Vertex.nearZ),
			uv = new Vector2(uvX, uvY),
			tint = Color.white
		};
	}
}
