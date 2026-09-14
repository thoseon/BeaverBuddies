using Timberborn.Stockpiles;
using UnityEngine.UIElements;

namespace Timberborn.StockpilesUI;

public interface IGoodSelectionController
{
	void Initialize(VisualElement root);

	void Update();

	void SetStockpile(Stockpile stockpile);

	void ShowGoodSelectionBox();

	void Clear();
}
