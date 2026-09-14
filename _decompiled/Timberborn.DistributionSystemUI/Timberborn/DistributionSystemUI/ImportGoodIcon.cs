using Timberborn.CoreUI;
using Timberborn.DistributionSystem;
using UnityEngine.UIElements;

namespace Timberborn.DistributionSystemUI;

public class ImportGoodIcon
{
	private readonly string _goodId;

	private readonly VisualElement _importableIcon;

	private readonly VisualElement _nonImportableIcon;

	public DistrictDistributableGoodProvider DistrictDistributableGoodProvider { get; private set; }

	public ImportGoodIcon(string goodId, VisualElement importableIcon, VisualElement nonImportableIcon)
	{
		_goodId = goodId;
		_importableIcon = importableIcon;
		_nonImportableIcon = nonImportableIcon;
	}

	public void SetDistrictDistributableGoodProvider(DistrictDistributableGoodProvider districtDistributableGoodProvider)
	{
		DistrictDistributableGoodProvider = districtDistributableGoodProvider;
	}

	public void Update()
	{
		bool flag = DistrictDistributableGoodProvider.IsImportEnabled(_goodId);
		_importableIcon.ToggleDisplayStyle(flag);
		_nonImportableIcon.ToggleDisplayStyle(!flag);
	}

	public void Clear()
	{
		DistrictDistributableGoodProvider = null;
	}
}
