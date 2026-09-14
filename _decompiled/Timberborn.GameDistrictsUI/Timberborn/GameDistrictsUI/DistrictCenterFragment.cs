using Timberborn.BaseComponentSystem;
using Timberborn.BatchControl;
using Timberborn.Bots;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.GameDistricts;
using Timberborn.GameDistrictsMigration;
using Timberborn.SliderToggleSystem;
using Timberborn.TooltipSystem;
using Timberborn.WorkSystem;
using Timberborn.WorkerTypesUI;
using UnityEngine.UIElements;

namespace Timberborn.GameDistrictsUI;

internal class DistrictCenterFragment : IEntityPanelFragment
{
	private static readonly string BeaverClass = "worker-type-toggle__icon--beaver";

	private static readonly string BotClass = "worker-type-toggle__icon--bot";

	private static readonly string WorkerTypeLocKey = "Work.DefaultAllowedWorker.Tooltip";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly IBatchControlBox _batchControlBox;

	private readonly ManualMigrationDistrictSetter _manualMigrationDistrictSetter;

	private readonly BotPopulation _botPopulation;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly WorkerTypeHelper _workerTypeHelper;

	private readonly SliderToggleFactory _sliderToggleFactory;

	private DistrictCenter _districtCenter;

	private DistrictDefaultWorkerType _districtDefaultWorkerType;

	private VisualElement _root;

	private VisualElement _workerTypeRoot;

	private SliderToggle _sliderToggle;

	public DistrictCenterFragment(VisualElementLoader visualElementLoader, IBatchControlBox batchControlBox, ManualMigrationDistrictSetter manualMigrationDistrictSetter, BotPopulation botPopulation, ITooltipRegistrar tooltipRegistrar, WorkerTypeHelper workerTypeHelper, SliderToggleFactory sliderToggleFactory)
	{
		_visualElementLoader = visualElementLoader;
		_batchControlBox = batchControlBox;
		_manualMigrationDistrictSetter = manualMigrationDistrictSetter;
		_botPopulation = botPopulation;
		_tooltipRegistrar = tooltipRegistrar;
		_workerTypeHelper = workerTypeHelper;
		_sliderToggleFactory = sliderToggleFactory;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/DistrictCenterFragment");
		_root.Q<Button>("MigrateButtonLeft").RegisterCallback<ClickEvent>(OpenMigrationTabAsLeft);
		_root.Q<Button>("MigrateButtonRight").RegisterCallback<ClickEvent>(OpenMigrationTabAsRight);
		_root.ToggleDisplayStyle(visible: false);
		_workerTypeRoot = _root.Q<VisualElement>("WorkerType");
		SliderToggleItem sliderToggleItem = SliderToggleItem.Create(GetBeaverWorkerTooltip, BeaverClass, SetBeaverWorkerType, IsBeaverWorkerType);
		SliderToggleItem sliderToggleItem2 = SliderToggleItem.Create(GetBotWorkerTooltip, BotClass, SetBotWorkerType, IsBotWorkerType);
		_sliderToggle = _sliderToggleFactory.Create(_workerTypeRoot, sliderToggleItem, sliderToggleItem2);
		_tooltipRegistrar.RegisterLocalizable(_workerTypeRoot, WorkerTypeLocKey);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_districtCenter = entity.GetComponent<DistrictCenter>();
		if ((bool)_districtCenter)
		{
			_districtDefaultWorkerType = _districtCenter.GetComponent<DistrictDefaultWorkerType>();
		}
	}

	public void ClearFragment()
	{
		_districtCenter = null;
		_districtDefaultWorkerType = null;
		_root.ToggleDisplayStyle(visible: false);
	}

	public void UpdateFragment()
	{
		if ((bool)_districtCenter && _districtCenter.Enabled)
		{
			_root.ToggleDisplayStyle(visible: true);
			_sliderToggle.Update();
			_workerTypeRoot.ToggleDisplayStyle(_botPopulation.BotCreated);
		}
	}

	private void OpenMigrationTabAsLeft(ClickEvent evt)
	{
		_manualMigrationDistrictSetter.SetLeftDistrictWithHighlight(_districtCenter);
		_batchControlBox.OpenMigrationTab();
	}

	private void OpenMigrationTabAsRight(ClickEvent evt)
	{
		_manualMigrationDistrictSetter.SetRightDistrictWithHighlight(_districtCenter);
		_batchControlBox.OpenMigrationTab();
	}

	private void SetBeaverWorkerType()
	{
		_districtDefaultWorkerType.SetWorkerType(WorkerTypeHelper.BeaverWorkerType);
	}

	private void SetBotWorkerType()
	{
		_districtDefaultWorkerType.SetWorkerType(WorkerTypeHelper.BotWorkerType);
	}

	private string GetBeaverWorkerTooltip()
	{
		return _workerTypeHelper.GetBeaverWorkerTypeDisplayText();
	}

	private string GetBotWorkerTooltip()
	{
		return _workerTypeHelper.GetBotWorkerTypeDisplayText();
	}

	private bool IsBeaverWorkerType()
	{
		return _workerTypeHelper.IsBeaverWorkerType(_districtDefaultWorkerType.WorkerType);
	}

	private bool IsBotWorkerType()
	{
		return _workerTypeHelper.IsBotWorkerType(_districtDefaultWorkerType.WorkerType);
	}
}
