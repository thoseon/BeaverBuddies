using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Goods;
using Timberborn.Ruins;
using UnityEngine.UIElements;

namespace Timberborn.RuinsUI;

internal class RuinFragment : IEntityPanelFragment
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly IGoodService _goodService;

	private Ruin _ruin;

	private VisualElement _root;

	private Label _goodRemaining;

	private Label _goodName;

	private Image _goodIcon;

	public RuinFragment(VisualElementLoader visualElementLoader, IGoodService goodService)
	{
		_visualElementLoader = visualElementLoader;
		_goodService = goodService;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/RuinFragment");
		_goodRemaining = _root.Q<Label>("GoodRemaining");
		_goodName = _root.Q<Label>("GoodName");
		_goodIcon = _root.Q<Image>("GoodIcon");
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_ruin = entity.GetComponent<Ruin>();
		if ((bool)_ruin)
		{
			string id = _ruin.YielderSpec.Yield.Id;
			GoodSpec good = _goodService.GetGood(id);
			_goodName.text = good.PluralDisplayName.Value;
			_goodIcon.sprite = good.IconSmall.Value;
			_root.ToggleDisplayStyle(visible: true);
		}
	}

	public void ClearFragment()
	{
		_ruin = null;
		_root.ToggleDisplayStyle(visible: false);
	}

	public void UpdateFragment()
	{
		if ((bool)_ruin)
		{
			_goodRemaining.text = _ruin.Yielder.Yield.Amount.ToString();
		}
	}
}
