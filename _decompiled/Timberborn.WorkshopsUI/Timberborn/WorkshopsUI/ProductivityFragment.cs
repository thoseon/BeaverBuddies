using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;
using Timberborn.UIFormatters;
using Timberborn.Workshops;
using UnityEngine.UIElements;

namespace Timberborn.WorkshopsUI;

internal class ProductivityFragment : IEntityPanelFragment
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly ILoc _loc;

	private WorkshopProductivityCounter _workshopProductivityCounter;

	private VisualElement _root;

	private Label _text;

	private readonly Phrase _productivityPhrase = Phrase.New("Work.Productivity").FormatPercentCeiled();

	public ProductivityFragment(VisualElementLoader visualElementLoader, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_loc = loc;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/ProductivityFragment");
		_text = _root.Q<Label>("Text");
		VisualElement tooltip = _root.Q<VisualElement>("Tooltip");
		_root.RegisterCallback<MouseEnterEvent>(delegate
		{
			tooltip.ToggleDisplayStyle(visible: true);
		});
		_root.RegisterCallback<MouseLeaveEvent>(delegate
		{
			tooltip.ToggleDisplayStyle(visible: false);
		});
		tooltip.ToggleDisplayStyle(visible: false);
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_workshopProductivityCounter = entity.GetComponent<WorkshopProductivityCounter>();
	}

	public void ClearFragment()
	{
		_workshopProductivityCounter = null;
		UpdateFragment();
	}

	public void UpdateFragment()
	{
		if ((bool)(BaseComponent)(object)_workshopProductivityCounter && ((BaseComponent)(object)_workshopProductivityCounter).Enabled)
		{
			float param = _workshopProductivityCounter.CalculateProductivity();
			_text.text = _loc.T(_productivityPhrase, param);
			_root.ToggleDisplayStyle(visible: true);
		}
		else
		{
			_root.ToggleDisplayStyle(visible: false);
		}
	}
}
