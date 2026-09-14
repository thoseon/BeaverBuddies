using Timberborn.RootProviders;
using Timberborn.SingletonSystem;
using UnityEngine.UIElements;

namespace Timberborn.CoreUI;

public class Underlay : ILoadableSingleton
{
	private readonly RootVisualElementProvider _rootVisualElementProvider;

	public VisualElement Root { get; private set; }

	public Underlay(RootVisualElementProvider rootVisualElementProvider)
	{
		_rootVisualElementProvider = rootVisualElementProvider;
	}

	public void Load()
	{
		VisualElement e = _rootVisualElementProvider.Create("Underlay", "Core/Underlay", 0);
		Root = e.Q<VisualElement>("Underlay");
		Disable();
	}

	public void Add(VisualElement element)
	{
		Root.Add(element);
		if (Root.childCount == 1)
		{
			Enable();
		}
	}

	public void Remove(VisualElement element)
	{
		Root.Remove(element);
		if (Root.childCount == 0)
		{
			Disable();
		}
	}

	private void Disable()
	{
		Root.ToggleDisplayStyle(visible: false);
	}

	private void Enable()
	{
		Root.ToggleDisplayStyle(visible: true);
	}
}
