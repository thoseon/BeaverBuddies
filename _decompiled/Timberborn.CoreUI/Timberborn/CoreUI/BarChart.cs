using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.CoreUI;

[UxmlElement]
public class BarChart : VisualElement
{
	[Serializable]
	[CompilerGenerated]
	public new class UxmlSerializedData : VisualElement.UxmlSerializedData
	{
		[UxmlAttribute("max-bars-amount")]
		[SerializeField]
		private int _maxBarsAmount;

		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UxmlAttributeFlags _maxBarsAmount_UxmlAttributeFlags;

		[RegisterUxmlCache]
		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			UxmlDescriptionCache.RegisterType(typeof(UxmlSerializedData), new UxmlAttributeNames[1]
			{
				new UxmlAttributeNames("_maxBarsAmount", "max-bars-amount", null)
			});
		}

		public override object CreateInstance()
		{
			return new BarChart();
		}

		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			BarChart barChart = (BarChart)obj;
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(_maxBarsAmount_UxmlAttributeFlags))
			{
				barChart._maxBarsAmount = _maxBarsAmount;
			}
		}
	}

	private static readonly float MinimumNonZeroFillRate = 0.025f;

	private static readonly CustomStyleProperty<Color> BackgroundHoverProperty = new CustomStyleProperty<Color>("--background-hover");

	[UxmlAttribute("max-bars-amount")]
	private int _maxBarsAmount = 15;

	private readonly List<IBarChartDataPoint> _dataPoints = new List<IBarChartDataPoint>();

	private float _maxValue;

	private Color _color;

	private Color _hoverColor;

	private int _hoveredBarIndex = -1;

	public IBarChartDataPoint HoveredDataPoint
	{
		get
		{
			if (_hoveredBarIndex < 0 || _hoveredBarIndex >= _dataPoints.Count)
			{
				return null;
			}
			return _dataPoints[_hoveredBarIndex];
		}
	}

	public int MaxBarsAmount => _maxBarsAmount;

	public BarChart()
	{
		base.generateVisualContent = (Action<MeshGenerationContext>)Delegate.Combine(base.generateVisualContent, new Action<MeshGenerationContext>(OnGenerateVisualContent));
		RegisterCallback<MouseMoveEvent>(OnMouseMove);
		RegisterCallback<MouseLeaveEvent>(delegate
		{
			SetHoveredBar(-1);
		});
	}

	public void Update(List<IBarChartDataPoint> dataPoints, float maxValue, Color color, Color hoverColor)
	{
		_dataPoints.Clear();
		_dataPoints.AddRange((dataPoints.Count > _maxBarsAmount) ? dataPoints.GetRange(dataPoints.Count - _maxBarsAmount, _maxBarsAmount) : dataPoints);
		_maxValue = maxValue;
		_color = color;
		_hoverColor = hoverColor;
		MarkDirtyRepaint();
	}

	private void OnMouseMove(MouseMoveEvent evt)
	{
		if (_dataPoints.Count != 0)
		{
			float num = base.contentRect.width / (float)_dataPoints.Count;
			int num2 = Mathf.FloorToInt(evt.localMousePosition.x / num);
			num2 = ((num2 < _dataPoints.Count) ? num2 : (-1));
			SetHoveredBar(num2);
		}
	}

	private void SetHoveredBar(int index)
	{
		_hoveredBarIndex = index;
		MarkDirtyRepaint();
	}

	private void OnGenerateVisualContent(MeshGenerationContext mgc)
	{
		if (_dataPoints.Count != 0)
		{
			Painter2D painter2D = mgc.painter2D;
			float width = base.contentRect.width;
			float height = base.contentRect.height;
			float barWidth = width / (float)_dataPoints.Count;
			DrawBars(painter2D, height, width, barWidth);
			DrawHover(painter2D, height, barWidth);
		}
	}

	private void DrawBars(Painter2D painter, float totalHeight, float totalWidth, float barWidth)
	{
		painter.BeginPath();
		painter.MoveTo(new Vector2(0f, totalHeight));
		for (int i = 0; i < _dataPoints.Count; i++)
		{
			float fillRate = GetFillRate(_dataPoints[i].Value, _maxValue);
			float y = SnapToPixel((1f - fillRate) * totalHeight);
			painter.LineTo(new Vector2((float)i * barWidth, y));
			painter.LineTo(new Vector2((float)(i + 1) * barWidth, y));
		}
		painter.LineTo(new Vector2(totalWidth, totalHeight));
		painter.ClosePath();
		painter.fillColor = _color;
		painter.Fill();
	}

	private void DrawHover(Painter2D painter, float totalHeight, float barWidth)
	{
		int num = ((_hoveredBarIndex >= 0 && _hoveredBarIndex < _dataPoints.Count) ? _hoveredBarIndex : (-1));
		if (num >= 0 && base.customStyle.TryGetValue(BackgroundHoverProperty, out var value))
		{
			painter.fillColor = value;
			DrawRect(painter, (float)num * barWidth, 0f, barWidth, totalHeight);
			painter.fillColor = _hoverColor;
			float fillRate = GetFillRate(_dataPoints[num].Value, _maxValue);
			float num2 = SnapToPixel((1f - fillRate) * totalHeight);
			DrawRect(painter, (float)num * barWidth, num2, barWidth, totalHeight - num2);
		}
	}

	private static void DrawRect(Painter2D painter, float x, float y, float width, float height)
	{
		painter.BeginPath();
		painter.MoveTo(new Vector2(x, y));
		painter.LineTo(new Vector2(x + width, y));
		painter.LineTo(new Vector2(x + width, y + height));
		painter.LineTo(new Vector2(x, y + height));
		painter.ClosePath();
		painter.Fill();
	}

	private static float GetFillRate(float value, float maxValue)
	{
		if (value == 0f)
		{
			return 0f;
		}
		if (maxValue == 0f)
		{
			return 1f;
		}
		return Mathf.Max(Mathf.Clamp01(value / maxValue), MinimumNonZeroFillRate);
	}

	private float SnapToPixel(float value)
	{
		float num = base.panel?.scaledPixelsPerPoint ?? 1f;
		return Mathf.Round(value * num) / num;
	}
}
