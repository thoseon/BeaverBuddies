using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using UnityEngine;

namespace Timberborn.SelectionSystem;

public class Highlighter
{
	private readonly Dictionary<GameObject, HighlightableObject> _primaries = new Dictionary<GameObject, HighlightableObject>();

	private readonly Dictionary<GameObject, HighlightableObject> _secondaries = new Dictionary<GameObject, HighlightableObject>();

	public void HighlightPrimary(BaseComponent target, Color color)
	{
		if ((bool)target)
		{
			GetOrAdd(target, _primaries).HighlightPrimary(this, color);
		}
	}

	public void HighlightSecondary(BaseComponent target, Color color)
	{
		if ((bool)target)
		{
			GetOrAdd(target, _secondaries).HighlightSecondary(this, color);
		}
	}

	public void UnhighlightPrimary(BaseComponent target)
	{
		if ((bool)target)
		{
			HighlightableObject orAdd = GetOrAdd(target, _primaries);
			if ((bool)orAdd)
			{
				orAdd.UnhighlightPrimaryColor(this);
				_primaries.Remove(target.GameObject);
			}
		}
	}

	public void UnhighlightSecondary(BaseComponent target)
	{
		if ((bool)target)
		{
			HighlightableObject orAdd = GetOrAdd(target, _secondaries);
			if ((bool)orAdd)
			{
				orAdd.UnhighlightSecondaryColor(this);
				_secondaries.Remove(target.GameObject);
			}
		}
	}

	public void UnhighlightAllPrimary()
	{
		foreach (HighlightableObject value in _primaries.Values)
		{
			if ((bool)value)
			{
				value.UnhighlightPrimaryColor(this);
			}
		}
		_primaries.Clear();
	}

	public void UnhighlightAllSecondary()
	{
		foreach (HighlightableObject value in _secondaries.Values)
		{
			if ((bool)value)
			{
				value.UnhighlightSecondaryColor(this);
			}
		}
		_secondaries.Clear();
	}

	public void ResetAllHighlights(BaseComponent target)
	{
		if ((bool)target)
		{
			GetOrAdd(target, _primaries).ResetAllHighlights();
		}
	}

	private HighlightableObject GetOrAdd(BaseComponent target, IDictionary<GameObject, HighlightableObject> highlightableObjects)
	{
		if (target is HighlightableObject result)
		{
			return result;
		}
		GameObject gameObject = target.GameObject;
		if (highlightableObjects.TryGetValue(gameObject, out var value))
		{
			return value;
		}
		HighlightableObject component = target.GetComponent<HighlightableObject>();
		highlightableObjects.Add(gameObject, component);
		return component;
	}
}
