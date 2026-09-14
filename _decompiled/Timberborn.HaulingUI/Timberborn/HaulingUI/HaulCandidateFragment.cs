using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Hauling;
using Timberborn.InputSystemUI;
using UnityEngine.UIElements;

namespace Timberborn.HaulingUI;

internal class HaulCandidateFragment : IEntityPanelFragment
{
	private static readonly string ToggleHaulingPriorityKey = "ToggleHaulingPriority";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly BindableToggleFactory _bindableToggleFactory;

	private BindableToggle _priorityToggle;

	private HaulCandidate _haulCandidate;

	private HaulPrioritizable _haulPrioritizable;

	private VisualElement _root;

	public HaulCandidateFragment(VisualElementLoader visualElementLoader, BindableToggleFactory bindableToggleFactory)
	{
		_visualElementLoader = visualElementLoader;
		_bindableToggleFactory = bindableToggleFactory;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/HaulCandidateFragment");
		_priorityToggle = _bindableToggleFactory.Create(_root.Q<Toggle>("Toggle"), ToggleHaulingPriorityKey, delegate(bool value)
		{
			_haulPrioritizable.Prioritized = value;
		}, () => _haulPrioritizable.Prioritized);
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		HaulCandidateUIBlocker component = entity.GetComponent<HaulCandidateUIBlocker>();
		if ((bool)component && component.ShouldShowUI())
		{
			_haulCandidate = entity.GetComponent<HaulCandidate>();
			_haulPrioritizable = entity.GetComponent<HaulPrioritizable>();
			_priorityToggle.Bind();
			_priorityToggle.Enable();
			_root.ToggleDisplayStyle(visible: true);
		}
		else
		{
			_root.ToggleDisplayStyle(visible: false);
		}
	}

	public void ClearFragment()
	{
		_root.ToggleDisplayStyle(visible: false);
		_priorityToggle.Disable();
		_priorityToggle.Unbind();
		_haulPrioritizable = null;
		_haulCandidate = null;
	}

	public void UpdateFragment()
	{
		if ((bool)_haulCandidate)
		{
			_priorityToggle.Update();
		}
	}
}
