using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.CoreUI;

[UxmlElement]
public class SimpleProgressBar : VisualElement
{
	[Serializable]
	[CompilerGenerated]
	public new class UxmlSerializedData : VisualElement.UxmlSerializedData
	{
		public override object CreateInstance()
		{
			return new SimpleProgressBar();
		}
	}

	private static readonly CustomStyleProperty<string> BackgroundImageProperty = new CustomStyleProperty<string>("--background-image");

	private Sprite _image;

	private float _progress;

	public SimpleProgressBar()
	{
		base.generateVisualContent = (Action<MeshGenerationContext>)Delegate.Combine(base.generateVisualContent, new Action<MeshGenerationContext>(OnGenerateVisualContent));
		RegisterCallback<CustomStyleResolvedEvent>(OnCustomStyleResolved);
	}

	public void SetProgress(float progress)
	{
		if (_progress != progress)
		{
			_progress = Mathf.Clamp01(progress);
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
		float x = width * _progress;
		if (width > 0.001f && height > 0.001f)
		{
			int vertexCount = meshWriter.VertexCount;
			meshWriter.SetNextVertex(CreateVertex(0f, 0f, 0f, 1f));
			meshWriter.SetNextVertex(CreateVertex(x, 0f, _progress, 1f));
			meshWriter.SetNextVertex(CreateVertex(x, height, _progress, 0f));
			meshWriter.SetNextVertex(CreateVertex(0f, height, 0f, 0f));
			meshWriter.SetNextIndex((ushort)vertexCount);
			meshWriter.SetNextIndex((ushort)(vertexCount + 1));
			meshWriter.SetNextIndex((ushort)(vertexCount + 2));
			meshWriter.SetNextIndex((ushort)(vertexCount + 2));
			meshWriter.SetNextIndex((ushort)(vertexCount + 3));
			meshWriter.SetNextIndex((ushort)vertexCount);
		}
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
