using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.Demolishing;
using Timberborn.DemolishingUI;
using Timberborn.EntityPanelSystem;
using UnityEngine.UIElements;

namespace Timberborn.MapEditorDemolishingUI;

internal class DemolishableScienceRewardFragment : IEntityPanelFragment
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly DemolishableScienceRewardLabelFactory _demolishableScienceRewardLabelFactory;

	private VisualElement _root;

	private DemolishableScienceRewardLabel _demolishableScienceRewardLabel;

	public DemolishableScienceRewardFragment(VisualElementLoader visualElementLoader, DemolishableScienceRewardLabelFactory demolishableScienceRewardLabelFactory)
	{
		_visualElementLoader = visualElementLoader;
		_demolishableScienceRewardLabelFactory = demolishableScienceRewardLabelFactory;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("MapEditor/EntityPanel/DemolishableScienceRewardFragment");
		_demolishableScienceRewardLabel = _demolishableScienceRewardLabelFactory.Create();
		_root.Add(_demolishableScienceRewardLabel.Root);
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		DemolishableScienceRewardSpec component = entity.GetComponent<DemolishableScienceRewardSpec>();
		if (component != null)
		{
			_demolishableScienceRewardLabel.Show(component);
			_root.ToggleDisplayStyle(visible: true);
		}
	}

	public void ClearFragment()
	{
		_root.ToggleDisplayStyle(visible: false);
	}

	public void UpdateFragment()
	{
	}
}
