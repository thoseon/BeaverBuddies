using Timberborn.Automation;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using UnityEngine.UIElements;

namespace Timberborn.AutomationUI;

internal class SequentialTransmitterResetFragment : IEntityPanelFragment
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly AutomationResetter _automationResetter;

	private VisualElement _root;

	private Automator _automator;

	private ISequentialTransmitter _sequentialTransmitter;

	public SequentialTransmitterResetFragment(VisualElementLoader visualElementLoader, AutomationResetter automationResetter)
	{
		_visualElementLoader = visualElementLoader;
		_automationResetter = automationResetter;
	}

	public VisualElement InitializeFragment()
	{
		string elementName = "Game/EntityPanel/ResettableEvaluatorFragment";
		_root = _visualElementLoader.LoadVisualElement(elementName);
		_root.Q<Button>("Reset").RegisterCallback<ClickEvent>(OnReset);
		_root.Q<Button>("ResetAll").RegisterCallback<ClickEvent>(OnResetAll);
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_sequentialTransmitter = entity.GetComponent<ISequentialTransmitter>();
		if (_sequentialTransmitter != null)
		{
			_automator = entity.GetComponent<Automator>();
			_root.ToggleDisplayStyle(visible: true);
		}
	}

	public void UpdateFragment()
	{
	}

	public void ClearFragment()
	{
		_automator = null;
		_sequentialTransmitter = null;
		_root.ToggleDisplayStyle(visible: false);
	}

	private void OnReset(ClickEvent evt)
	{
		_sequentialTransmitter.Reset();
	}

	private void OnResetAll(ClickEvent evt)
	{
		_automationResetter.ResetPartition(_automator);
	}
}
