using Timberborn.InventorySystem;
using Timberborn.Stockpiles;
using UnityEngine.UIElements;

namespace Timberborn.StockpilesUI;

internal class GoodSelectionController : IGoodSelectionController
{
	private static readonly string NoIconClass = "icon--hidden";

	private readonly StockpileGoodSelectionBoxFactory _stockpileGoodSelectionBoxFactory;

	private readonly StockpileOptionsService _stockpileOptionsService;

	private StockpileGoodSelectionBox _stockpileGoodSelectionBox;

	private Image _selectedGoodIcon;

	private Label _selectedGoodText;

	private Button _goodSelectionButton;

	private SingleGoodAllower _singleGoodAllower;

	private Stockpile _stockpile;

	public GoodSelectionController(StockpileGoodSelectionBoxFactory stockpileGoodSelectionBoxFactory, StockpileOptionsService stockpileOptionsService)
	{
		_stockpileGoodSelectionBoxFactory = stockpileGoodSelectionBoxFactory;
		_stockpileOptionsService = stockpileOptionsService;
	}

	public void Initialize(VisualElement root)
	{
		_stockpileGoodSelectionBox = _stockpileGoodSelectionBoxFactory.Create();
		root.Add(_stockpileGoodSelectionBox.Root);
		_selectedGoodIcon = root.Q<Image>("GoodIcon");
		_selectedGoodText = root.Q<Label>("SelectionItem");
		_goodSelectionButton = root.Q<Button>("Selection");
		_goodSelectionButton.RegisterCallback<ClickEvent>(delegate
		{
			ShowGoodSelectionBox();
		});
		_goodSelectionButton.RegisterCallback<MouseEnterEvent>(delegate
		{
			_stockpileGoodSelectionBox.DisableInput();
		});
		_goodSelectionButton.RegisterCallback<MouseLeaveEvent>(delegate
		{
			_stockpileGoodSelectionBox.EnableInput();
		});
	}

	public void Update()
	{
		_stockpileGoodSelectionBox.Update();
	}

	public void SetStockpile(Stockpile stockpile)
	{
		_stockpile = stockpile;
		_singleGoodAllower = stockpile.GetComponent<SingleGoodAllower>();
		_singleGoodAllower.DisallowedGoodsChanged += OnDisallowedGoodsChanged;
		UpdateSelectedGood();
	}

	public void ShowGoodSelectionBox()
	{
		_stockpileGoodSelectionBox.ToggleGoodSelection(_stockpile);
	}

	public void Clear()
	{
		_stockpile = null;
		if ((bool)_singleGoodAllower)
		{
			_singleGoodAllower.DisallowedGoodsChanged -= OnDisallowedGoodsChanged;
		}
		_singleGoodAllower = null;
	}

	private void OnDisallowedGoodsChanged(object sender, DisallowedGoodsChangedEventArgs e)
	{
		UpdateSelectedGood();
	}

	private void UpdateSelectedGood()
	{
		string key = _singleGoodAllower.AllowedGood ?? StockpileOptionsService.NothingSelectedLocKey;
		_stockpileOptionsService.UpdateItem(_selectedGoodText, _selectedGoodIcon, key);
		_selectedGoodIcon.EnableInClassList(NoIconClass, _singleGoodAllower.HasAllowedGood);
	}
}
