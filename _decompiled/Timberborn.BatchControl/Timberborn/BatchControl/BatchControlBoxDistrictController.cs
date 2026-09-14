using Timberborn.CoreUI;
using Timberborn.DropdownSystem;
using Timberborn.EntityNaming;
using Timberborn.GameDistricts;
using Timberborn.SingletonSystem;
using UnityEngine.UIElements;

namespace Timberborn.BatchControl;

internal class BatchControlBoxDistrictController
{
	private readonly BatchControlBoxTabController _batchControlBoxTabController;

	private readonly BatchControlDistrict _batchControlDistrict;

	private readonly DistrictCenterRegistry _districtCenterRegistry;

	private readonly DistrictContextService _districtContextService;

	private readonly DistrictDropdownProvider _districtDropdownProvider;

	private readonly DropdownItemsSetter _dropdownItemsSetter;

	private readonly EventBus _eventBus;

	private VisualElement _root;

	private Dropdown _dropdown;

	public BatchControlBoxDistrictController(BatchControlBoxTabController batchControlBoxTabController, BatchControlDistrict batchControlDistrict, DistrictCenterRegistry districtCenterRegistry, DistrictContextService districtContextService, DistrictDropdownProvider districtDropdownProvider, DropdownItemsSetter dropdownItemsSetter, EventBus eventBus)
	{
		_batchControlBoxTabController = batchControlBoxTabController;
		_batchControlDistrict = batchControlDistrict;
		_districtCenterRegistry = districtCenterRegistry;
		_districtContextService = districtContextService;
		_districtDropdownProvider = districtDropdownProvider;
		_dropdownItemsSetter = dropdownItemsSetter;
		_eventBus = eventBus;
	}

	public void Initialize(VisualElement root)
	{
		_root = root;
		_dropdown = _root.Q<Dropdown>("DistrictDropdown");
	}

	public void Show()
	{
		_batchControlDistrict.SetDistrict(_districtContextService.SelectedDistrict);
		UpdateDropdown();
		_eventBus.Register(this);
	}

	public void Clear()
	{
		_dropdown.ClearItems();
		_eventBus.Unregister(this);
	}

	[OnEvent]
	public void OnBatchControlTabShown(BatchControlTabShownEvent batchControlTabShownEvent)
	{
		bool visible = !batchControlTabShownEvent.BatchControlTab.IgnoreDistrictSelection;
		_dropdown.ToggleDisplayStyle(visible);
	}

	[OnEvent]
	public void OnDistrictCenterRegistryChanged(DistrictCenterRegistryChangedEvent districtCenterRegistryChangedEvent)
	{
		if ((bool)_batchControlDistrict.SelectedDistrict && !_districtCenterRegistry.FinishedDistrictCenters.Contains(_batchControlDistrict.SelectedDistrict))
		{
			_batchControlDistrict.SetDistrict(null);
		}
		UpdateDropdown();
	}

	[OnEvent]
	public void OnBatchControlDistrictChanged(BatchControlDistrictChangedEvent batchControlDistrictChangedEvent)
	{
		_batchControlBoxTabController.CurrentTab?.UpdateRowsVisibility();
		_dropdown.UpdateSelectedValue();
	}

	[OnEvent]
	public void OnEntityNameChanged(EntityNameChangedEvent entityNameChangedEvent)
	{
		if ((bool)entityNameChangedEvent.Entity.GetComponent<DistrictCenter>())
		{
			UpdateDropdown();
		}
	}

	private void UpdateDropdown()
	{
		_districtDropdownProvider.UpdateDistrictsList();
		_dropdownItemsSetter.SetItems(_dropdown, _districtDropdownProvider);
	}
}
