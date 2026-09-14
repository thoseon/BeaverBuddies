using System.Collections.Immutable;
using Timberborn.BaseComponentSystem;
using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.DistributionSystem;
using Timberborn.EntityPanelSystem;
using Timberborn.GameDistricts;
using UnityEngine.UIElements;

namespace Timberborn.DistributionSystemUI;

internal class DistrictCrossingFragment : IEntityPanelFragment
{
	private readonly IBatchControlBox _batchControlBox;

	private readonly BatchControlDistrict _batchControlDistrict;

	private readonly ImportGoodIconFactory _importGoodIconFactory;

	private readonly VisualElementLoader _visualElementLoader;

	private ImmutableArray<ImportGoodIcon> _importGoodIcons;

	private DistrictCrossing _districtCrossing;

	private VisualElement _root;

	public DistrictCrossingFragment(IBatchControlBox batchControlBox, BatchControlDistrict batchControlDistrict, ImportGoodIconFactory importGoodIconFactory, VisualElementLoader visualElementLoader)
	{
		_batchControlBox = batchControlBox;
		_batchControlDistrict = batchControlDistrict;
		_importGoodIconFactory = importGoodIconFactory;
		_visualElementLoader = visualElementLoader;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/DistrictCrossingFragment");
		_root.ToggleDisplayStyle(visible: false);
		_root.Q<Button>("DistributionButton").RegisterCallback<ClickEvent>(OnDistributionButtonClicked);
		VisualElement parent = _root.Q<VisualElement>("ImportGoodsWrapper");
		_importGoodIcons = _importGoodIconFactory.CreateImportGoods(parent).ToImmutableArray();
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_districtCrossing = entity.GetComponent<DistrictCrossing>();
		UpdateRootAndIcons();
	}

	public void ClearFragment()
	{
		_districtCrossing = null;
		_root.ToggleDisplayStyle(visible: false);
		foreach (ImportGoodIcon importGoodIcon in _importGoodIcons)
		{
			importGoodIcon.Clear();
		}
	}

	public void UpdateFragment()
	{
		UpdateRootAndIcons();
	}

	private void UpdateRootAndIcons()
	{
		if ((bool)_districtCrossing && (bool)_districtCrossing.DistrictDistributableGoodProvider)
		{
			_root.ToggleDisplayStyle(visible: true);
			foreach (ImportGoodIcon importGoodIcon in _importGoodIcons)
			{
				importGoodIcon.SetDistrictDistributableGoodProvider(_districtCrossing.DistrictDistributableGoodProvider);
				importGoodIcon.Update();
			}
		}
		else
		{
			_root.ToggleDisplayStyle(visible: false);
		}
	}

	private void OnDistributionButtonClicked(ClickEvent evt)
	{
		DistrictCenter district = _districtCrossing.GetComponent<DistrictBuilding>().District;
		_batchControlDistrict.SetDistrict(district);
		_batchControlBox.OpenDistributionTab();
	}
}
