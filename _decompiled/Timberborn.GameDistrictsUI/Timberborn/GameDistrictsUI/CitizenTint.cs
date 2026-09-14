using Timberborn.BaseComponentSystem;
using Timberborn.Characters;
using Timberborn.CoreUI;
using Timberborn.EntityNaming;
using Timberborn.GameDistricts;

namespace Timberborn.GameDistrictsUI;

internal class CitizenTint : BaseComponent, IAwakableComponent
{
	private CharacterTint _characterTint;

	private Citizen _citizen;

	public void Awake()
	{
		_characterTint = GetComponent<CharacterTint>();
		_citizen = GetComponent<Citizen>();
	}

	public void UpdateTint()
	{
		string entityName = _citizen.GetComponent<NamedEntity>().EntityName;
		string text = (_citizen.HasAssignedDistrict ? _citizen.AssignedDistrict.DistrictName : string.Empty);
		if (ColorParser.TryGetColorFromText(entityName, out var color) || ColorParser.TryGetColorFromText(text, out color))
		{
			_characterTint.SetTint(color);
		}
		else
		{
			_characterTint.DisableTint();
		}
	}
}
