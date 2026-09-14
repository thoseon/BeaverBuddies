using Timberborn.BaseComponentSystem;
using Timberborn.Carrying;
using Timberborn.GameDistricts;

namespace Timberborn.ResourceCountingSystem;

internal class GoodCarrierRegistrar : BaseComponent, IAwakableComponent
{
	private GoodCarrier _goodCarrier;

	private Citizen _citizen;

	public void Awake()
	{
		_goodCarrier = GetComponent<GoodCarrier>();
		_citizen = GetComponent<Citizen>();
		_citizen.ChangedAssignedDistrict += OnChangedAssignedDistrict;
	}

	private void OnChangedAssignedDistrict(object sender, ChangeAssignedDistrictEventArgs e)
	{
		if ((bool)e.PreviousDistrict)
		{
			e.PreviousDistrict.GetComponent<DistrictResourceCounter>().Remove(_goodCarrier);
		}
		if ((bool)e.CurrentDistrict)
		{
			e.CurrentDistrict.GetComponent<DistrictResourceCounter>().Add(_goodCarrier);
		}
	}
}
