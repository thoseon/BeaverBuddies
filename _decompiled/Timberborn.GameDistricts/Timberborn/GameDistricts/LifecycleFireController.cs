using Timberborn.BaseComponentSystem;
using Timberborn.Buildings;

namespace Timberborn.GameDistricts;

internal class LifecycleFireController : BaseComponent, IAwakableComponent
{
	public void Awake()
	{
		FireIntensityController fireIntensityController = GetComponent<FireIntensityController>();
		DistrictCitizenLifecycleNotifier component = GetComponent<DistrictCitizenLifecycleNotifier>();
		component.BeaverBorn += delegate
		{
			fireIntensityController.Strengthen();
		};
		component.BeaverDied += delegate
		{
			fireIntensityController.Dampen();
		};
	}
}
