using Timberborn.CoreUI;
using UnityEngine.UIElements;

namespace Timberborn.InventorySystemUI;

public class InventoryFragmentBuilderFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly InformationalRowsFactory _informationalRowsFactory;

	public InventoryFragmentBuilderFactory(VisualElementLoader visualElementLoader, InformationalRowsFactory informationalRowsFactory)
	{
		_visualElementLoader = visualElementLoader;
		_informationalRowsFactory = informationalRowsFactory;
	}

	public InventoryFragment.Builder CreateBuilder(VisualElement root)
	{
		return new InventoryFragment.Builder(_visualElementLoader, _informationalRowsFactory, root);
	}
}
