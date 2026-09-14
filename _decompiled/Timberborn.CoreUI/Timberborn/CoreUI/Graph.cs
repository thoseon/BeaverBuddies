using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Timberborn.Common;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.CoreUI;

[UxmlElement]
public class Graph : VisualElement
{
	[Serializable]
	[CompilerGenerated]
	public new class UxmlSerializedData : VisualElement.UxmlSerializedData
	{
		public override object CreateInstance()
		{
			return new Graph();
		}
	}

	private readonly List<GraphDefinition> _graphDefinitions = new List<GraphDefinition>();

	private Action<VisualElement, int> _tooltipRegistrarAction;

	public override VisualElement contentContainer { get; }

	public Graph()
	{
		Resources.Load<VisualTreeAsset>("UI/Views/Core/Graph").CloneTree(this);
		contentContainer = this.Q<VisualElement>("Graph");
		base.generateVisualContent = (Action<MeshGenerationContext>)Delegate.Combine(base.generateVisualContent, new Action<MeshGenerationContext>(OnGenerateVisualContent));
	}

	public void AddGraphDefinition(GraphDefinition graphDefinition, Action<VisualElement, int> tooltipRegistrarAction)
	{
		_graphDefinitions.Add(graphDefinition);
		_tooltipRegistrarAction = tooltipRegistrarAction;
	}

	public void ClearGraphDefinitions()
	{
		_graphDefinitions.Clear();
	}

	private void OnGenerateVisualContent(MeshGenerationContext ctx)
	{
		Painter2D painter2D = ctx.painter2D;
		painter2D.lineWidth = 1.4f;
		float num = float.MaxValue;
		float num2 = 20f;
		int num3 = 0;
		foreach (GraphDefinition graphDefinition in _graphDefinitions)
		{
			if (graphDefinition.IsVisible && graphDefinition.MinValue < num)
			{
				num = graphDefinition.MinValue;
			}
			if (graphDefinition.IsVisible && graphDefinition.MaxValue > num2)
			{
				num2 = graphDefinition.MaxValue;
			}
			if (graphDefinition.IsVisible && graphDefinition.DataPoints.Count > num3)
			{
				num3 = graphDefinition.DataPoints.Count;
			}
		}
		if (Mathf.Approximately(num2, num))
		{
			num2 = num + 1f;
		}
		foreach (GraphDefinition graphDefinition2 in _graphDefinitions)
		{
			PaintGraph(graphDefinition2, painter2D, num, num2);
		}
		AddBackgrounds(num3);
	}

	private void PaintGraph(GraphDefinition graphDefinition, Painter2D painter, float minValue, float maxValue)
	{
		ReadOnlyList<float> dataPoints = graphDefinition.DataPoints;
		if (dataPoints.Count < 2 || !graphDefinition.IsVisible)
		{
			return;
		}
		painter.strokeColor = graphDefinition.Color;
		painter.lineJoin = LineJoin.Round;
		Rect rect = contentContainer.ChangeCoordinatesTo(this, contentContainer.contentRect);
		float x = rect.x;
		float y = rect.y;
		float width = rect.width;
		float height = rect.height;
		painter.BeginPath();
		for (int i = 0; i < dataPoints.Count - 1; i++)
		{
			float x2 = x + (float)i / (float)(dataPoints.Count - 1) * width;
			float y2 = y + height - Mathf.Clamp01((dataPoints[i] - minValue) / (maxValue - minValue)) * height;
			float x3 = x + (float)(i + 1) / (float)(dataPoints.Count - 1) * width;
			float y3 = y + height - Mathf.Clamp01((dataPoints[i + 1] - minValue) / (maxValue - minValue)) * height;
			if (i == 0)
			{
				painter.MoveTo(new Vector2(x2, y2));
			}
			painter.LineTo(new Vector2(x3, y3));
		}
		painter.Stroke();
	}

	private void AddBackgrounds(int dataPoints)
	{
		float num = 1f / (float)(dataPoints - 1);
		int num2 = contentContainer.childCount;
		for (int i = 0; i < num2; i++)
		{
			contentContainer[i].style.display = DisplayStyle.None;
		}
		for (int j = 0; j < dataPoints; j++)
		{
			float num3 = ((j == 0 || j == dataPoints - 1) ? (num / 2f) : num);
			if (j < num2)
			{
				VisualElement visualElement = contentContainer[j];
				visualElement.style.display = DisplayStyle.Flex;
				visualElement.style.width = Length.Percent(num3 * 100f);
				EnableEdgeClasses(visualElement, j == 0, j == dataPoints - 1);
			}
			else
			{
				VisualElement visualElement2 = new VisualElement();
				visualElement2.AddToClassList("graph-background");
				visualElement2.style.width = Length.Percent(num3 * 100f);
				visualElement2.Add(new VisualElement());
				_tooltipRegistrarAction(visualElement2, j);
				contentContainer.Add(visualElement2);
				EnableEdgeClasses(visualElement2, j == 0, j == dataPoints - 1);
			}
		}
	}

	private static void EnableEdgeClasses(VisualElement root, bool isFirst, bool isLast)
	{
		root.EnableInClassList("first", isFirst);
		root.EnableInClassList("last", isLast);
	}
}
