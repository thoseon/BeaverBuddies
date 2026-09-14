using System.Collections.Generic;
using Timberborn.BlockSystem;
using Timberborn.CoreUI;
using Timberborn.Goods;
using Timberborn.RecoverableGoodSystem;
using UnityEngine.UIElements;

namespace Timberborn.RecoverableGoodSystemUI;

public class RecoverableGoodElementFactory
{
	private static readonly string InBoxClass = "recoverable-good-content--in-box";

	private readonly IGoodService _goodService;

	private readonly RecoverableGoodItemFactory _recoverableGoodItemFactory;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly HashSet<BlockObject> _blockObjectsCache = new HashSet<BlockObject>();

	public RecoverableGoodElementFactory(IGoodService goodService, RecoverableGoodItemFactory recoverableGoodItemFactory, VisualElementLoader visualElementLoader)
	{
		_goodService = goodService;
		_recoverableGoodItemFactory = recoverableGoodItemFactory;
		_visualElementLoader = visualElementLoader;
	}

	public RecoverableGoodElement Create()
	{
		string elementName = "Game/RecoverableGood/RecoverableGoodContent";
		VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
		Label label = visualElement.Q<Label>("Label");
		VisualElement parent = visualElement.Q<VisualElement>("Items");
		IEnumerable<RecoverableGoodItem> recoverableGoodItems = CreateRecoverableGoodItems(parent);
		return new RecoverableGoodElement(visualElement, label, recoverableGoodItems);
	}

	public VisualElement Create(IEnumerable<BlockObject> blockObjects)
	{
		RecoverableGoodElement recoverableGoodElement = Create();
		recoverableGoodElement.Root.AddToClassList(InBoxClass);
		RecoverableGoodRegistry recoverableGoodRegistry = GetRecoverableGoodRegistry(blockObjects);
		recoverableGoodElement.Update(recoverableGoodRegistry);
		return recoverableGoodElement.Root;
	}

	private IEnumerable<RecoverableGoodItem> CreateRecoverableGoodItems(VisualElement parent)
	{
		foreach (string good in _goodService.Goods)
		{
			RecoverableGoodItem recoverableGoodItem = _recoverableGoodItemFactory.Create(good);
			parent.Add(recoverableGoodItem.Root);
			yield return recoverableGoodItem;
		}
	}

	private RecoverableGoodRegistry GetRecoverableGoodRegistry(IEnumerable<BlockObject> blockObjects)
	{
		RecoverableGoodRegistry recoverableGoodRegistry = new RecoverableGoodRegistry();
		FillBlockObjectsCache(blockObjects);
		foreach (BlockObject item in _blockObjectsCache)
		{
			AddFromRecoverableGoodProvider(item, recoverableGoodRegistry);
		}
		_blockObjectsCache.Clear();
		return recoverableGoodRegistry;
	}

	private void FillBlockObjectsCache(IEnumerable<BlockObject> blockObjects)
	{
		foreach (BlockObject blockObject in blockObjects)
		{
			_blockObjectsCache.Add(blockObject);
			IRecoverableObjectAdder component = blockObject.GetComponent<IRecoverableObjectAdder>();
			if (component != null)
			{
				BlockObject additionalObjectToRecover = component.GetAdditionalObjectToRecover();
				if ((bool)additionalObjectToRecover)
				{
					_blockObjectsCache.Add(additionalObjectToRecover);
				}
			}
		}
	}

	private static void AddFromRecoverableGoodProvider(BlockObject blockObject, RecoverableGoodRegistry recoverableGoodRegistry)
	{
		blockObject.GetComponent<RecoverableGoodProvider>()?.GetRecoverableGoods(recoverableGoodRegistry);
	}
}
