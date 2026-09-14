using System;
using Timberborn.BaseComponentSystem;
using Timberborn.Characters;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;

namespace Timberborn.GameDistricts;

public class Citizen : BaseComponent, IAwakableComponent, IInitializableEntity, IPersistentEntity, IChildhoodInfluenced, IDeletableEntity
{
	private static readonly ComponentKey CitizenKey = new ComponentKey("Citizen");

	private static readonly PropertyKey<DistrictCenter> AssignedDistrictKey = new PropertyKey<DistrictCenter>("AssignedDistrict");

	private readonly UnassignedCitizenRegistry _unassignedCitizenRegistry;

	private readonly ReferenceSerializer _referenceSerializer;

	private readonly CitizenUnstucker _citizenUnstucker;

	private Character _character;

	public DistrictCenter AssignedDistrict { get; private set; }

	public bool HasAssignedDistrict => AssignedDistrict;

	public event EventHandler<ChangeAssignedDistrictEventArgs> ChangedAssignedDistrict;

	internal Citizen(UnassignedCitizenRegistry unassignedCitizenRegistry, ReferenceSerializer referenceSerializer, CitizenUnstucker citizenUnstucker)
	{
		_unassignedCitizenRegistry = unassignedCitizenRegistry;
		_referenceSerializer = referenceSerializer;
		_citizenUnstucker = citizenUnstucker;
	}

	public void Awake()
	{
		_character = GetComponent<Character>();
		_character.Died += OnDied;
	}

	public void InitializeEntity()
	{
		if (_character.Alive && !AssignedDistrict)
		{
			_unassignedCitizenRegistry.Add(this);
		}
	}

	public void DeleteEntity()
	{
		RemoveFromDistrictsAssignment();
	}

	public void AssignInitialDistrict(DistrictCenter districtCenter)
	{
		Asserts.FieldIsNull(this, AssignedDistrict, "AssignedDistrict");
		AssignDistrict(districtCenter);
		districtCenter.GetComponent<DistrictCitizenLifecycleNotifier>().AddNewCitizen(this);
	}

	public void AssignDistrict(DistrictCenter districtCenter)
	{
		UnassignDistrict();
		AssignedDistrict = districtCenter;
		_unassignedCitizenRegistry.Remove(this);
		districtCenter.DistrictPopulation.AssignCitizen(this);
		ChangedAssignedDistrict?.Invoke(this, new ChangeAssignedDistrictEventArgs(null, AssignedDistrict));
	}

	public void UnassignDistrictIfCutOff()
	{
		if (AssignedDistrict == null)
		{
			return;
		}
		if ((bool)AssignedDistrict)
		{
			if (!AssignedDistrict.IsGloballyReachableFromCitizen(this) && !_citizenUnstucker.TryUnstuckAndKeepDistrict(this, AssignedDistrict))
			{
				UnassignDistrict();
			}
		}
		else
		{
			UnassignDistrict();
		}
	}

	public void Save(IEntitySaver entitySaver)
	{
		if ((bool)AssignedDistrict)
		{
			entitySaver.GetComponent(CitizenKey).Set(AssignedDistrictKey, AssignedDistrict, _referenceSerializer.Of<DistrictCenter>());
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(CitizenKey, out var objectLoader) && objectLoader.GetObsoletable(AssignedDistrictKey, _referenceSerializer.Of<DistrictCenter>(), out var value))
		{
			AssignDistrict(value);
		}
	}

	public void InfluenceByChildhood(Character child)
	{
		Citizen component = child.GetComponent<Citizen>();
		if (component.HasAssignedDistrict)
		{
			AssignDistrict(component.AssignedDistrict);
		}
	}

	private void UnassignDistrict()
	{
		DistrictCenter assignedDistrict = AssignedDistrict;
		if (HasAssignedDistrict)
		{
			AssignedDistrict.DistrictPopulation.UnassignCitizen(this);
		}
		_unassignedCitizenRegistry.Add(this);
		AssignedDistrict = null;
		ChangedAssignedDistrict?.Invoke(this, new ChangeAssignedDistrictEventArgs(assignedDistrict, AssignedDistrict));
	}

	private void OnDied(object sender, EventArgs e)
	{
		if ((bool)AssignedDistrict)
		{
			AssignedDistrict.GetComponent<DistrictCitizenLifecycleNotifier>().RemoveDiedCitizen(this);
		}
		RemoveFromDistrictsAssignment();
	}

	private void RemoveFromDistrictsAssignment()
	{
		UnassignDistrict();
		_unassignedCitizenRegistry.Remove(this);
	}
}
