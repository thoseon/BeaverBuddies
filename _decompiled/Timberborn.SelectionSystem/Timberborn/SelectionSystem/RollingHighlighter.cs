using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using UnityEngine;

namespace Timberborn.SelectionSystem;

public class RollingHighlighter
{
	private readonly Highlighter _highlighter;

	private HashSet<BaseComponent> _current = new HashSet<BaseComponent>();

	private HashSet<BaseComponent> _previous = new HashSet<BaseComponent>();

	public RollingHighlighter(Highlighter highlighter)
	{
		_highlighter = highlighter;
	}

	public void HighlightPrimary(BaseComponent component, Color color)
	{
		Swap();
		_current.Add(component);
		HighlightPrimary(color);
	}

	public void HighlightPrimary(IEnumerable<BaseComponent> components, Color color)
	{
		Swap();
		foreach (BaseComponent component in components)
		{
			_current.Add(component);
		}
		HighlightPrimary(color);
	}

	public void UnhighlightAllPrimary()
	{
		_highlighter.UnhighlightAllPrimary();
		_current.Clear();
		_previous.Clear();
	}

	private void Swap()
	{
		HashSet<BaseComponent> current = _current;
		HashSet<BaseComponent> previous = _previous;
		_previous = current;
		_current = previous;
	}

	private void HighlightPrimary(Color color)
	{
		foreach (BaseComponent item in _current)
		{
			if (_previous.Contains(item))
			{
				_previous.Remove(item);
			}
			else
			{
				_highlighter.HighlightPrimary(item, color);
			}
		}
		foreach (BaseComponent previou in _previous)
		{
			_highlighter.UnhighlightPrimary(previou);
		}
		_previous.Clear();
	}
}
