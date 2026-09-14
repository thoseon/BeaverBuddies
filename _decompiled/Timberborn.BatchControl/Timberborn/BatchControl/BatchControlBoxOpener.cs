using System.Collections.Generic;
using Timberborn.InputSystem;
using Timberborn.SingletonSystem;
using Timberborn.UILayoutSystem;

namespace Timberborn.BatchControl;

internal class BatchControlBoxOpener : ILoadableSingleton, IInputProcessor
{
	private readonly IBatchControlBox _batchControlBox;

	private readonly BatchControlBoxTabController _batchControlBoxTabController;

	private readonly InputService _inputService;

	private readonly EventBus _eventBus;

	private readonly List<BatchControlTab> _batchControlTabs = new List<BatchControlTab>();

	public BatchControlBoxOpener(IBatchControlBox batchControlBox, BatchControlBoxTabController batchControlBoxTabController, InputService inputService, EventBus eventBus)
	{
		_batchControlBox = batchControlBox;
		_batchControlBoxTabController = batchControlBoxTabController;
		_inputService = inputService;
		_eventBus = eventBus;
	}

	public void Load()
	{
		_batchControlTabs.AddRange(_batchControlBoxTabController.Tabs);
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnShowPrimaryUI(ShowPrimaryUIEvent showPrimaryUIEvent)
	{
		_inputService.AddInputProcessor(this);
	}

	public bool ProcessInput()
	{
		foreach (BatchControlTab batchControlTab in _batchControlTabs)
		{
			if (_inputService.IsKeyDown(batchControlTab.BindingKey))
			{
				OpenTab(batchControlTab);
				return true;
			}
		}
		return false;
	}

	private void OpenTab(BatchControlTab batchControlTab)
	{
		int tabIndex = _batchControlBoxTabController.GetTabIndex(batchControlTab);
		_batchControlBox.OpenTab(tabIndex);
	}
}
