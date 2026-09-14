using Timberborn.BaseComponentSystem;
using Timberborn.EntityNaming;
using Timberborn.EntitySystem;
using Timberborn.GameDistricts;

namespace Timberborn.GameDistrictsUI;

internal class CitizenDistrictTintChanger : BaseComponent, IAwakableComponent, IPostInitializableEntity
{
	private DistrictPopulation _districtPopulation;

	public void Awake()
	{
		_districtPopulation = GetComponent<DistrictPopulation>();
		GetComponent<NamedEntity>().EntityNameChanged += delegate
		{
			UpdatePopulationTint();
		};
	}

	public void PostInitializeEntity()
	{
		_districtPopulation.CitizenAssigned += delegate(object _, CitizenAssignedEventArgs e)
		{
			UpdateCitizenTint(e.Citizen);
		};
		_districtPopulation.CitizenUnassigned += delegate(object _, CitizenUnassignedEventArgs e)
		{
			UpdateCitizenTint(e.Citizen);
		};
		UpdatePopulationTint();
	}

	private void UpdatePopulationTint()
	{
		for (int i = 0; i < _districtPopulation.Beavers.Count; i++)
		{
			UpdateCitizenTint(_districtPopulation.Beavers[i]);
		}
		for (int j = 0; j < _districtPopulation.Bots.Count; j++)
		{
			UpdateCitizenTint(_districtPopulation.Bots[j]);
		}
	}

	private static void UpdateCitizenTint(BaseComponent citizen)
	{
		CitizenTint component = citizen.GetComponent<CitizenTint>();
		if ((bool)component)
		{
			component.UpdateTint();
		}
	}
}
