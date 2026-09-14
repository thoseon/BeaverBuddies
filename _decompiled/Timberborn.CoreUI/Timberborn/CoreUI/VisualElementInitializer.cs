using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace Timberborn.CoreUI;

public class VisualElementInitializer
{
	private readonly List<IVisualElementInitializer> _visualElementInitializers;

	public VisualElementInitializer(IEnumerable<IVisualElementInitializer> visualElementInitializers)
	{
		_visualElementInitializers = visualElementInitializers.ToList();
	}

	public void InitializeVisualElement(VisualElement visualElement)
	{
		InitializeWithEveryInitializer(visualElement);
		foreach (VisualElement item in visualElement.hierarchy.Children())
		{
			InitializeVisualElement(item);
		}
	}

	private void InitializeWithEveryInitializer(VisualElement visualElement)
	{
		for (int i = 0; i < _visualElementInitializers.Count; i++)
		{
			_visualElementInitializers[i].InitializeVisualElement(visualElement);
		}
	}
}
