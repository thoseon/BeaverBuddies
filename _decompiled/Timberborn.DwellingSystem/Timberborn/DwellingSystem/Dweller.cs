using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.Beavers;
using Timberborn.Characters;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.Navigation;
using Timberborn.Persistence;
using Timberborn.RelationSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.DwellingSystem;

public class Dweller : BaseComponent, IAwakableComponent, IPersistentEntity, IDeletableEntity, IInitializableEntity, IRelationOwner
{
	private static readonly ComponentKey DwellerKey = new ComponentKey("Dweller");

	private static readonly PropertyKey<Dwelling> HomeKey = new PropertyKey<Dwelling>("Home");

	private readonly ReferenceSerializer _referenceSerializer;

	private bool _isAdult;

	private bool _loaded;

	public Dwelling Home { get; private set; }

	public bool HasHome => Home;

	public Vector3? HomeAccess => Home.GetEnabledComponent<Accessible>().UnblockedSingleAccess;

	private bool HomeIsOverpopulated
	{
		get
		{
			if (!_isAdult)
			{
				return Home.OverpopulatedByChildren;
			}
			return Home.OverpopulatedByAdults;
		}
	}

	private bool HomeIsUnderpopulated
	{
		get
		{
			if (_isAdult && Home.FreeAdultSlots >= 1)
			{
				return Home.NumberOfDwellers == 1;
			}
			return false;
		}
	}

	public event EventHandler RelationsChanged;

	public Dweller(ReferenceSerializer referenceSerializer)
	{
		_referenceSerializer = referenceSerializer;
	}

	public void Awake()
	{
		_isAdult = HasComponent<AdultSpec>();
		GetComponent<Character>().Died += delegate
		{
			UnassignFromHome();
		};
	}

	public void InitializeEntity()
	{
		if (_loaded)
		{
			AssignToDwellingAfterLoad();
		}
	}

	public void DeleteEntity()
	{
		UnassignFromHome();
	}

	public void AssignToHome(Dwelling home)
	{
		if (Home != home)
		{
			UnassignFromHome();
			Home = home;
			RelationsChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	public void UnassignFromHome()
	{
		if ((bool)Home)
		{
			Dwelling home = Home;
			Home = null;
			home.UnassignDweller(this);
			RelationsChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	public bool IsLookingForBetterHome()
	{
		if (HasHome && !HomeIsOverpopulated)
		{
			return HomeIsUnderpopulated;
		}
		return true;
	}

	public void Save(IEntitySaver entitySaver)
	{
		if ((bool)Home)
		{
			entitySaver.GetComponent(DwellerKey).Set(HomeKey, Home, _referenceSerializer.Of<Dwelling>());
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(DwellerKey, out var objectLoader) && objectLoader.GetObsoletable(HomeKey, _referenceSerializer.Of<Dwelling>(), out var value))
		{
			Home = value;
			_loaded = true;
		}
	}

	public IEnumerable<BaseComponent> GetRelations()
	{
		if (!HasHome)
		{
			return Enumerable.Empty<BaseComponent>();
		}
		return Enumerables.One(Home);
	}

	private void AssignToDwellingAfterLoad()
	{
		if ((bool)Home)
		{
			if (Home.HasFreeSlots)
			{
				Home.AssignDweller(this);
				return;
			}
			string firstName = GetComponent<Character>().FirstName;
			Debug.LogWarning("After loading " + firstName + " couldn't get assigned to their old home, " + Home.Name);
			Home = null;
		}
	}
}
