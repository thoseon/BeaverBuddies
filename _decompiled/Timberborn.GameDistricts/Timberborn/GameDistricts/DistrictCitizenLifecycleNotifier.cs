using System;
using Timberborn.BaseComponentSystem;
using Timberborn.Beavers;

namespace Timberborn.GameDistricts;

public class DistrictCitizenLifecycleNotifier : BaseComponent
{
	public event EventHandler<Citizen> BeaverBorn;

	public event EventHandler<Citizen> BeaverDied;

	public void AddNewCitizen(Citizen citizen)
	{
		if ((bool)citizen.GetComponent<Child>())
		{
			BeaverBorn?.Invoke(this, citizen);
		}
	}

	public void RemoveDiedCitizen(Citizen citizen)
	{
		if (citizen.HasComponent<BeaverSpec>())
		{
			BeaverDied?.Invoke(this, citizen);
		}
	}
}
