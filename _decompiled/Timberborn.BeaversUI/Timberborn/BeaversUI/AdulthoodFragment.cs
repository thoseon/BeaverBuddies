using Timberborn.BaseComponentSystem;
using Timberborn.Beavers;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.BeaversUI;

internal class AdulthoodFragment : IEntityPanelFragment
{
	private readonly VisualElementLoader _visualElementLoader;

	private Child _child;

	private VisualElement _root;

	private Timberborn.CoreUI.ProgressBar _progressBar;

	public AdulthoodFragment(VisualElementLoader visualElementLoader)
	{
		_visualElementLoader = visualElementLoader;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/AdulthoodFragment");
		_progressBar = _root.Q<Timberborn.CoreUI.ProgressBar>("ProgressBar");
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_child = entity.GetComponent<Child>();
		if ((bool)(BaseComponent)(object)_child)
		{
			_root.ToggleDisplayStyle(visible: true);
		}
	}

	public void ClearFragment()
	{
		_root.ToggleDisplayStyle(visible: false);
		_child = null;
	}

	public void UpdateFragment()
	{
		if ((bool)(BaseComponent)(object)_child)
		{
			float progress = Mathf.Clamp01(_child.GrowthProgress);
			_progressBar.SetProgress(progress);
		}
	}
}
