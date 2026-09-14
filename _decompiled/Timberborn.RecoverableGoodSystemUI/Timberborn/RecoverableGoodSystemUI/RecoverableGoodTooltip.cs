using System.Collections.Generic;
using Timberborn.BlockSystem;
using Timberborn.CoreUI;
using Timberborn.RecoverableGoodSystem;
using Timberborn.SingletonSystem;
using Timberborn.TooltipSystem;
using UnityEngine.UIElements;

namespace Timberborn.RecoverableGoodSystemUI;

public class RecoverableGoodTooltip : ILoadableSingleton, IUpdatableSingleton
{
	private readonly RecoverableGoodElementFactory _recoverableGoodElementFactory;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly VisualElementLoader _visualElementLoader;

	private RecoverableGoodElement _recoverableGoodElement;

	private VisualElement _tooltip;

	private readonly RecoverableGoodRegistry _recoverableGoodRegistry = new RecoverableGoodRegistry();

	private bool _enabled;

	public RecoverableGoodTooltip(RecoverableGoodElementFactory recoverableGoodElementFactory, ITooltipRegistrar tooltipRegistrar, VisualElementLoader visualElementLoader)
	{
		_recoverableGoodElementFactory = recoverableGoodElementFactory;
		_tooltipRegistrar = tooltipRegistrar;
		_visualElementLoader = visualElementLoader;
	}

	public void Load()
	{
		string elementName = "Game/RecoverableGood/RecoverableGoodTooltip";
		_tooltip = _visualElementLoader.LoadVisualElement(elementName);
		_recoverableGoodElement = _recoverableGoodElementFactory.Create();
		_tooltip.Add(_recoverableGoodElement.Root);
	}

	public void Enable()
	{
		_enabled = true;
	}

	public void Disable()
	{
		_enabled = false;
		Hide();
	}

	public void SetRecoverableGoods(IEnumerable<BlockObject> blockObjects)
	{
		_recoverableGoodRegistry.Clear();
		foreach (BlockObject blockObject in blockObjects)
		{
			blockObject.GetComponent<RecoverableGoodProvider>()?.GetRecoverableGoods(_recoverableGoodRegistry);
		}
	}

	public void UpdateSingleton()
	{
		if (_enabled)
		{
			Update();
		}
	}

	private void Update()
	{
		if (_recoverableGoodRegistry.TotalAmount > 0)
		{
			_tooltipRegistrar.ShowPriority(_tooltip);
			_recoverableGoodElement.Update(_recoverableGoodRegistry);
			_recoverableGoodRegistry.Clear();
		}
		else
		{
			Hide();
		}
	}

	private void Hide()
	{
		_tooltipRegistrar.HidePriority();
	}
}
