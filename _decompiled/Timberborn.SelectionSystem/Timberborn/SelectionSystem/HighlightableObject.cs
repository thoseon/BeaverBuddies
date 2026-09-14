using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.Rendering;
using UnityEngine;

namespace Timberborn.SelectionSystem;

public class HighlightableObject : BaseComponent, IDeletableEntity
{
	private readonly struct HighlighterColor : IEquatable<HighlighterColor>
	{
		public Highlighter Highlighter { get; }

		public Color Color { get; }

		public HighlighterColor(Highlighter highlighter, Color color)
		{
			Highlighter = highlighter;
			Color = color;
		}

		public bool Equals(HighlighterColor other)
		{
			if (object.Equals(Highlighter, other.Highlighter))
			{
				return Color.Equals(other.Color);
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj is HighlighterColor other)
			{
				return Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Highlighter, Color);
		}

		public static bool operator ==(HighlighterColor left, HighlighterColor right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(HighlighterColor left, HighlighterColor right)
		{
			return !left.Equals(right);
		}
	}

	private readonly MaterialColorer _materialColorer;

	private readonly HighlightRenderingService _highlightRenderingService;

	private readonly List<HighlighterColor> _primaryColors = new List<HighlighterColor>();

	private readonly List<HighlighterColor> _secondaryColors = new List<HighlighterColor>();

	private bool _isHighlighted;

	public HighlightableObject(MaterialColorer materialColorer, HighlightRenderingService highlightRenderingService)
	{
		_materialColorer = materialColorer;
		_highlightRenderingService = highlightRenderingService;
	}

	public void DeleteEntity()
	{
		if (_isHighlighted)
		{
			_highlightRenderingService.RemoveFromHighlight(base.GameObject);
		}
	}

	public void HighlightPrimary(Highlighter highlighter, Color color)
	{
		HighlightColor(_primaryColors, highlighter, color);
	}

	public void HighlightSecondary(Highlighter highlighter, Color color)
	{
		HighlightColor(_secondaryColors, highlighter, color);
	}

	public void UnhighlightPrimaryColor(Highlighter highlighter)
	{
		RemoveHighlighterColor(_primaryColors, highlighter);
		UpdateColorAndHighlight();
	}

	public void UnhighlightSecondaryColor(Highlighter highlighter)
	{
		RemoveHighlighterColor(_secondaryColors, highlighter);
		UpdateColorAndHighlight();
	}

	public void UpdateColorAndHighlight()
	{
		if (HasHighlightColor(out var color))
		{
			_materialColorer.SetEmissionColor(this, color);
			_highlightRenderingService.AddToHighlight(base.GameObject);
			_isHighlighted = true;
		}
		else
		{
			_materialColorer.ResetEmissionColor(this);
			_highlightRenderingService.RemoveFromHighlight(base.GameObject);
			_isHighlighted = false;
		}
	}

	public void ResetAllHighlights()
	{
		_primaryColors.Clear();
		_secondaryColors.Clear();
		UpdateColorAndHighlight();
	}

	public void RefreshHighlight()
	{
		if (_isHighlighted && HasHighlightColor(out var color))
		{
			_materialColorer.SetEmissionColor(this, color);
		}
	}

	private void HighlightColor(IList<HighlighterColor> highlighterColors, Highlighter highlighter, Color color)
	{
		if (!TryGetHighlighterColor(highlighterColors, highlighter, out var highlighterColor) || highlighterColor.Color != color)
		{
			highlighterColors.Remove(highlighterColor);
			HighlighterColor item = new HighlighterColor(highlighter, color);
			highlighterColors.Add(item);
			UpdateColorAndHighlight();
		}
	}

	private static void RemoveHighlighterColor(IList<HighlighterColor> highlighterColors, Highlighter highlighter)
	{
		for (int i = 0; i < highlighterColors.Count; i++)
		{
			if (highlighterColors[i].Highlighter == highlighter)
			{
				highlighterColors.RemoveAt(i);
				break;
			}
		}
	}

	private bool HasHighlightColor(out Color color)
	{
		if (_primaryColors.Count > 0)
		{
			color = _primaryColors.Last().Color;
			return true;
		}
		if (_secondaryColors.Count > 0)
		{
			color = _secondaryColors.Last().Color;
			return true;
		}
		color = default(Color);
		return false;
	}

	private static bool TryGetHighlighterColor(IList<HighlighterColor> highlighterColors, Highlighter highlighter, out HighlighterColor highlighterColor)
	{
		for (int i = 0; i < highlighterColors.Count; i++)
		{
			if (highlighterColors[i].Highlighter == highlighter)
			{
				highlighterColor = highlighterColors[i];
				return true;
			}
		}
		highlighterColor = default(HighlighterColor);
		return false;
	}
}
