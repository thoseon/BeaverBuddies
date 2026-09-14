using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.CoreUI;

public class NineSliceBackground
{
	private static readonly CustomStyleProperty<string> BackgroundImageProperty = new CustomStyleProperty<string>("--background-image");

	private static readonly CustomStyleProperty<Color> BackgroundTintProperty = new CustomStyleProperty<Color>("--background-tint");

	private static readonly CustomStyleProperty<int> BackgroundSliceProperty = new CustomStyleProperty<int>("--background-slice");

	private static readonly CustomStyleProperty<int> BackgroundSliceTopProperty = new CustomStyleProperty<int>("--background-slice-top");

	private static readonly CustomStyleProperty<int> BackgroundSliceRightProperty = new CustomStyleProperty<int>("--background-slice-right");

	private static readonly CustomStyleProperty<int> BackgroundSliceBottomProperty = new CustomStyleProperty<int>("--background-slice-bottom");

	private static readonly CustomStyleProperty<int> BackgroundSliceLeftProperty = new CustomStyleProperty<int>("--background-slice-left");

	private static readonly CustomStyleProperty<float> BackgroundSliceScaleProperty = new CustomStyleProperty<float>("--background-slice-scale");

	private Sprite _image;

	private Color32 _tint;

	private int _sliceBottom;

	private int _sliceLeft;

	private int _sliceRight;

	private float _sliceScale;

	private int _sliceTop;

	public bool IsNineSlice => _image != null;

	public void GetDataFromStyle(ICustomStyle customStyle)
	{
		_image = (customStyle.TryGetValue(BackgroundImageProperty, out var value) ? Resources.Load<Sprite>(value) : null);
		_tint = (customStyle.TryGetValue(BackgroundTintProperty, out var value2) ? value2 : Color.white);
		customStyle.TryGetValue(BackgroundSliceProperty, out var value3);
		_sliceTop = (customStyle.TryGetValue(BackgroundSliceTopProperty, out var value4) ? value4 : value3);
		_sliceRight = (customStyle.TryGetValue(BackgroundSliceRightProperty, out var value5) ? value5 : value3);
		_sliceBottom = (customStyle.TryGetValue(BackgroundSliceBottomProperty, out var value6) ? value6 : value3);
		_sliceLeft = (customStyle.TryGetValue(BackgroundSliceLeftProperty, out var value7) ? value7 : value3);
		_sliceScale = (customStyle.TryGetValue(BackgroundSliceScaleProperty, out var value8) ? value8 : 1f);
	}

	public void GenerateVisualContent(MeshGenerationContext mgc, Rect paddingRect)
	{
		if (IsNineSlice)
		{
			MeshWriter meshWriter = default(MeshWriter);
			WriteMesh(ref meshWriter, paddingRect);
			meshWriter.StartWriting(mgc, _image.texture);
			WriteMesh(ref meshWriter, paddingRect);
		}
	}

	private void WriteMesh(ref MeshWriter meshWriter, Rect paddingRect)
	{
		Rect rect = paddingRect;
		float width = rect.width;
		float height = rect.height;
		if (!(width > 0.01f) || !(height > 0.01f))
		{
			return;
		}
		Texture2D texture = _image.texture;
		int width2 = texture.width;
		int height2 = texture.height;
		float num = (float)_sliceTop * _sliceScale;
		float num2 = (float)_sliceRight * _sliceScale;
		float num3 = (float)_sliceBottom * _sliceScale;
		float num4 = (float)_sliceLeft * _sliceScale;
		float num5 = Mathf.Min(width / (num4 + num2), 1f);
		float num6 = Mathf.Min(height / (num + num3), 1f);
		float num7 = num * num6;
		float num8 = num2 * num5;
		float num9 = num3 * num6;
		float num10 = num4 * num5;
		float num11 = (float)_sliceTop / (float)height2 * num6;
		float num12 = (float)_sliceRight / (float)width2 * num5;
		float num13 = (float)_sliceBottom / (float)height2 * num6;
		float num14 = (float)_sliceLeft / (float)width2 * num5;
		bool flag = num5 >= 1f;
		bool num15 = num6 >= 1f;
		int num16 = width2 - _sliceLeft - _sliceRight;
		int num17 = height2 - _sliceTop - _sliceBottom;
		float num18 = width - num4 - num2;
		float num19 = height - num - num3;
		float f = num18 / (_sliceScale * (float)num16);
		float f2 = num19 / (_sliceScale * (float)num17);
		int num20 = (flag ? Math.Max(Mathf.RoundToInt(f), 1) : 0);
		int num21 = (num15 ? Math.Max(Mathf.RoundToInt(f2), 1) : 0);
		float num22 = (flag ? (num18 / (float)num20) : 0f);
		float num23 = (num15 ? (num19 / (float)num21) : 0f);
		AddRectangle(ref meshWriter, 0f, 0f, num10, num7, 0f, 1f, num14, 1f - num11);
		AddRectangle(ref meshWriter, width - num8, 0f, width, num7, 1f - num12, 1f, 1f, 1f - num11);
		AddRectangle(ref meshWriter, 0f, height - num9, num10, height, 0f, num13, num14, 0f);
		AddRectangle(ref meshWriter, width - num8, height - num9, width, height, 1f - num12, num13, 1f, 0f);
		for (int i = 0; i < num20; i++)
		{
			AddRectangle(ref meshWriter, num10 + (float)i * num22, 0f, num10 + (float)(i + 1) * num22, num7, num14, 1f, 1f - num12, 1f - num11);
		}
		for (int j = 0; j < num20; j++)
		{
			AddRectangle(ref meshWriter, num10 + (float)j * num22, height - num9, num10 + (float)(j + 1) * num22, height, num14, num13, 1f - num12, 0f);
		}
		for (int k = 0; k < num21; k++)
		{
			AddRectangle(ref meshWriter, 0f, num7 + (float)k * num23, num10, num7 + (float)(k + 1) * num23, 0f, 1f - num11, num14, num13);
		}
		for (int l = 0; l < num21; l++)
		{
			AddRectangle(ref meshWriter, width - num8, num7 + (float)l * num23, width, num7 + (float)(l + 1) * num23, 1f - num12, 1f - num11, 1f, num13);
		}
		for (int m = 0; m < num21; m++)
		{
			for (int n = 0; n < num20; n++)
			{
				AddRectangle(ref meshWriter, num10 + (float)n * num22, num7 + (float)m * num23, num10 + (float)(n + 1) * num22, num7 + (float)(m + 1) * num23, num14, 1f - num11, 1f - num12, num13);
			}
		}
	}

	private void AddRectangle(ref MeshWriter meshWriter, float x0, float y0, float x1, float y1, float u0, float v0, float u1, float v1)
	{
		if (x1 - x0 > 0.001f && y1 - y0 > 0.001f)
		{
			Vertex nextVertex = new Vertex
			{
				position = new Vector3(x0, y0, Vertex.nearZ),
				uv = new Vector2(u0, v0),
				tint = _tint
			};
			Vertex nextVertex2 = new Vertex
			{
				position = new Vector3(x1, y0, Vertex.nearZ),
				uv = new Vector2(u1, v0),
				tint = _tint
			};
			Vertex nextVertex3 = new Vertex
			{
				position = new Vector3(x1, y1, Vertex.nearZ),
				uv = new Vector2(u1, v1),
				tint = _tint
			};
			Vertex nextVertex4 = new Vertex
			{
				position = new Vector3(x0, y1, Vertex.nearZ),
				uv = new Vector2(u0, v1),
				tint = _tint
			};
			int vertexCount = meshWriter.VertexCount;
			meshWriter.SetNextVertex(nextVertex);
			meshWriter.SetNextVertex(nextVertex2);
			meshWriter.SetNextVertex(nextVertex3);
			meshWriter.SetNextVertex(nextVertex4);
			meshWriter.SetNextIndex((ushort)vertexCount);
			meshWriter.SetNextIndex((ushort)(vertexCount + 1));
			meshWriter.SetNextIndex((ushort)(vertexCount + 2));
			meshWriter.SetNextIndex((ushort)(vertexCount + 2));
			meshWriter.SetNextIndex((ushort)(vertexCount + 3));
			meshWriter.SetNextIndex((ushort)vertexCount);
		}
	}
}
