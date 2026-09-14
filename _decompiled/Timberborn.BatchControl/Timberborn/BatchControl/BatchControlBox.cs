using Timberborn.CameraSystem;
using Timberborn.CoreUI;
using Timberborn.InputSystem;
using Timberborn.SingletonSystem;
using Timberborn.UILayoutSystem;
using UnityEngine.UIElements;

namespace Timberborn.BatchControl;

internal class BatchControlBox : ILoadableSingleton, IPanelController, IInputProcessor, IBatchControlBox
{
	private static readonly string ToggleBatchControlBoxKey = "ToggleBatchControlBox";

	private static readonly float CameraOffset = -0.2f;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly UILayout _uiLayout;

	private readonly PanelStack _panelStack;

	private readonly CameraHorizontalShifter _cameraHorizontalShifter;

	private readonly InputService _inputService;

	private readonly EventBus _eventBus;

	private readonly BatchControlBoxTabController _batchControlBoxTabController;

	private readonly BatchControlBoxDistrictController _batchControlBoxDistrictController;

	private readonly IHideableByBatchControl _hideableByBatchControl;

	private VisualElement _root;

	public BatchControlBox(VisualElementLoader visualElementLoader, UILayout uiLayout, PanelStack panelStack, CameraHorizontalShifter cameraHorizontalShifter, InputService inputService, EventBus eventBus, BatchControlBoxTabController batchControlBoxTabController, BatchControlBoxDistrictController batchControlBoxDistrictController, IHideableByBatchControl hideableByBatchControl)
	{
		_visualElementLoader = visualElementLoader;
		_uiLayout = uiLayout;
		_panelStack = panelStack;
		_cameraHorizontalShifter = cameraHorizontalShifter;
		_inputService = inputService;
		_eventBus = eventBus;
		_batchControlBoxTabController = batchControlBoxTabController;
		_batchControlBoxDistrictController = batchControlBoxDistrictController;
		_hideableByBatchControl = hideableByBatchControl;
	}

	public void Load()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/BatchControl/BatchControlBox");
		_batchControlBoxTabController.Initialize(_root);
		_batchControlBoxDistrictController.Initialize(_root);
		_root.Q<Button>("CancelButton").RegisterCallback<ClickEvent>(delegate
		{
			Close();
		});
		_root.Q<Button>("SettlementButton").RegisterCallback<ClickEvent>(delegate
		{
			Close();
		});
		_eventBus.Register(this);
	}

	public VisualElement GetPanel()
	{
		_uiLayout.HideLeftAndCenterItems();
		_hideableByBatchControl.Hide();
		_cameraHorizontalShifter.EnableHorizontalCameraShift(CameraOffset);
		_batchControlBoxTabController.UpdateEntities();
		_inputService.AddInputProcessor(this);
		return _root;
	}

	public bool OnUIConfirmed()
	{
		return false;
	}

	public void OnUICancelled()
	{
		Close();
	}

	public bool ProcessInput()
	{
		if (_inputService.IsKeyDown(ToggleBatchControlBoxKey))
		{
			Close();
			return true;
		}
		return false;
	}

	public void OpenBatchControlBox()
	{
		OpenTab(_batchControlBoxTabController.LastOpenedTabIndex);
	}

	public void OpenCharactersTab()
	{
		OpenTab(0);
	}

	public void OpenHousingTab()
	{
		OpenTab(1);
	}

	public void OpenWorkplacesTab()
	{
		OpenTab(2);
	}

	public void OpenMigrationTab()
	{
		OpenTab(6);
	}

	public void OpenDistributionTab()
	{
		OpenTab(7);
	}

	public void OpenTab(int index)
	{
		if (_batchControlBoxTabController.CurrentTab != null)
		{
			_batchControlBoxTabController.ShowTab(index);
			return;
		}
		_panelStack.HideAndPushWithoutPause(this);
		_batchControlBoxDistrictController.Show();
		_batchControlBoxTabController.ShowTab(index);
		_eventBus.Post(new BatchControlBoxShownEvent());
	}

	private void Close()
	{
		_batchControlBoxTabController.Clear();
		_batchControlBoxDistrictController.Clear();
		_uiLayout.ShowLeftAndCenterItems();
		_hideableByBatchControl.Show();
		_panelStack.Pop(this);
		_cameraHorizontalShifter.DisableCameraShift();
		_inputService.RemoveInputProcessor(this);
		_eventBus.Post(new BatchControlBoxHiddenEvent());
	}
}
