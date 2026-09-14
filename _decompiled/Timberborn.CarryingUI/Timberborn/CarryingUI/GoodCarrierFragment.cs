using Timberborn.BaseComponentSystem;
using Timberborn.BeaverContaminationSystem;
using Timberborn.Carrying;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Goods;
using Timberborn.GoodsUI;
using Timberborn.Localization;
using Timberborn.UIFormatters;
using UnityEngine.UIElements;

namespace Timberborn.CarryingUI;

internal class GoodCarrierFragment : IEntityPanelFragment
{
	private static readonly string NothingLocKey = "Carrying.Nothing";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly GoodDescriber _goodDescriber;

	private readonly ILoc _loc;

	private readonly IGoodService _goodService;

	private GoodCarrier _goodCarrier;

	private Contaminable _contaminable;

	private Label _carryText;

	private VisualElement _root;

	private string _nothingText;

	private readonly Phrase _carryingPhrase = Phrase.New("Carrying.Carry");

	private readonly Phrase _goodPhrase = Phrase.New("Carrying.Good").Format((string value) => value.ToString()).Format((int value) => value.ToString())
		.FormatKilogram<int>();

	public GoodCarrierFragment(VisualElementLoader visualElementLoader, GoodDescriber goodDescriber, ILoc loc, IGoodService goodService)
	{
		_visualElementLoader = visualElementLoader;
		_goodDescriber = goodDescriber;
		_loc = loc;
		_goodService = goodService;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/GoodCarrierFragment");
		_carryText = _root.Q<Label>("GoodCarrierFragment");
		_root.ToggleDisplayStyle(visible: false);
		_nothingText = _loc.T(NothingLocKey);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_goodCarrier = entity.GetComponent<GoodCarrier>();
		_contaminable = entity.GetComponent<Contaminable>();
	}

	public void ClearFragment()
	{
		_root.ToggleDisplayStyle(visible: false);
		_goodCarrier = null;
	}

	public void UpdateFragment()
	{
		if (!_goodCarrier)
		{
			return;
		}
		GoodAmount goodAmount = _goodCarrier.CarriedGood.GoodAmount;
		int liftingCapacity = _goodCarrier.LiftingCapacity;
		_root.ToggleDisplayStyle(visible: true);
		if (goodAmount.Amount > 0)
		{
			int weight = _goodService.GetGood(goodAmount.GoodId).Weight;
			int param = goodAmount.Amount * weight;
			string param2 = _goodDescriber.Describe(goodAmount);
			string param3 = _loc.T(_goodPhrase, param2, param, liftingCapacity);
			_carryText.text = _loc.T(_carryingPhrase, param3);
		}
		else
		{
			Contaminable contaminable = _contaminable;
			if (contaminable != null && contaminable.IsContaminated)
			{
				_root.ToggleDisplayStyle(visible: false);
			}
			else
			{
				_carryText.text = _loc.T(_carryingPhrase, _nothingText);
			}
		}
	}
}
