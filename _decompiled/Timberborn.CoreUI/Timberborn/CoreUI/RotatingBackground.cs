using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.CoreUI;

[UxmlElement]
public class RotatingBackground : VisualElement
{
	[Serializable]
	[CompilerGenerated]
	public new class UxmlSerializedData : VisualElement.UxmlSerializedData
	{
		public override object CreateInstance()
		{
			return new RotatingBackground();
		}
	}

	private static readonly CustomStyleProperty<string> BackgroundImageProperty = new CustomStyleProperty<string>("--background-image");

	private static readonly Color32 White = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	private Sprite _image;

	private float _angle;

	public RotatingBackground()
	{
		base.generateVisualContent = (Action<MeshGenerationContext>)Delegate.Combine(base.generateVisualContent, new Action<MeshGenerationContext>(OnGenerateVisualContent));
		RegisterCallback<CustomStyleResolvedEvent>(OnCustomStyleResolved);
	}

	public void SetRotation(float angle)
	{
		if (_angle != angle)
		{
			_angle = angle;
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
		Vector3 center = new Vector3(width / 2f, height / 2f, 0f);
		Quaternion quaternion = Quaternion.AngleAxis(_angle, Vector3.forward);
		if (width > 0.001f && height > 0.001f)
		{
			Vertex nextVertex = new Vertex
			{
				position = RotateVertex(new Vector3(0f, 0f, Vertex.nearZ), center, quaternion),
				uv = new Vector2(0f, 1f),
				tint = White
			};
			Vertex nextVertex2 = new Vertex
			{
				position = RotateVertex(new Vector3(width, 0f, Vertex.nearZ), center, quaternion),
				uv = new Vector2(1f, 1f),
				tint = White
			};
			Vertex nextVertex3 = new Vertex
			{
				position = RotateVertex(new Vector3(width, height, Vertex.nearZ), center, quaternion),
				uv = new Vector2(1f, 0f),
				tint = White
			};
			Vertex nextVertex4 = new Vertex
			{
				position = RotateVertex(new Vector3(0f, height, Vertex.nearZ), center, quaternion),
				uv = new Vector2(0f, 0f),
				tint = White
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

	private static Vector3 RotateVertex(Vector3 input, Vector3 center, Quaternion quaternion)
	{
		return quaternion * (input - center) + center;
	}
}
