using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.Beavers;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.NeedSpecs;
using Timberborn.RelationSystem;
using UnityEngine;

namespace Timberborn.DwellingSystem;

public class Dwelling : BaseComponent, IAwakableComponent, IFinishedStateListener, IRegisteredComponent, IRelationOwner
{
	private DwellingSpec _dwellingSpec;

	private readonly HashSet<Dweller> _adults = new HashSet<Dweller>();

	private readonly HashSet<Dweller> _children = new HashSet<Dweller>();

	public int ChildSlots { get; private set; }

	public int AdultSlots { get; private set; }

	public int MaxBeavers => _dwellingSpec.MaxBeavers;

	public IReadOnlyCollection<ContinuousEffectSpec> SleepEffects => _dwellingSpec.SleepEffects;

	public IEnumerable<Dweller> AdultDwellers => _adults.AsReadOnlyEnumerable();

	public IEnumerable<Dweller> ChildDwellers => _children.AsReadOnlyEnumerable();

	public int NumberOfDwellers => NumberOfAdultDwellers + NumberOfChildDwellers;

	public int NumberOfAdultDwellers => _adults.Count;

	public int NumberOfChildDwellers => _children.Count;

	public bool HasFreeSlots => NumberOfAdultDwellers + NumberOfChildDwellers < MaxBeavers;

	public bool IsEmpty
	{
		get
		{
			if (NumberOfAdultDwellers == 0)
			{
				return NumberOfChildDwellers == 0;
			}
			return false;
		}
	}

	public bool OverpopulatedByAdults => NumberOfAdultDwellers > AdultSlots;

	public int FreeAdultSlots => AdultSlots - NumberOfAdultDwellers;

	public bool OverpopulatedByChildren => DesiredNumberOfChildren < NumberOfChildDwellers;

	public bool UnderpopulatedByChildren
	{
		get
		{
			if (DesiredNumberOfChildren > NumberOfChildDwellers)
			{
				return ChildSlots > 0;
			}
			return false;
		}
	}

	public bool HasFreeChildSlots
	{
		get
		{
			if (HasFreeSlots)
			{
				return ChildSlots > NumberOfChildDwellers;
			}
			return false;
		}
	}

	public int TotalSlots => AdultSlots + ChildSlots;

	private int DesiredNumberOfChildren => NumberOfAdultDwellers / 2;

	public event EventHandler RelationsChanged;

	public event EventHandler NumberOfDwellersChanged;

	public void Awake()
	{
		_dwellingSpec = GetComponent<DwellingSpec>();
		ChildSlots = Mathf.FloorToInt((float)MaxBeavers / 3f);
		AdultSlots = MaxBeavers - ChildSlots;
		DisableComponent();
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
	}

	public void OnExitFinishedState()
	{
		UnassignAllDwellers();
		DisableComponent();
	}

	public void AssignDweller(Dweller dweller)
	{
		if (!HasFreeSlots)
		{
			throw new InvalidOperationException("Tried to assign a dweller to a full dwelling!");
		}
		dweller.AssignToHome(this);
		if ((bool)dweller.GetComponent<Child>())
		{
			_children.Add(dweller);
		}
		else
		{
			_adults.Add(dweller);
		}
		RelationsChanged?.Invoke(this, EventArgs.Empty);
		NumberOfDwellersChanged?.Invoke(this, EventArgs.Empty);
	}

	public void UnassignDweller(Dweller dweller)
	{
		_adults.Remove(dweller);
		_children.Remove(dweller);
		dweller.UnassignFromHome();
		RelationsChanged?.Invoke(this, EventArgs.Empty);
		NumberOfDwellersChanged?.Invoke(this, EventArgs.Empty);
	}

	public void UnassignAllDwellers()
	{
		foreach (Dweller item in _adults.Concat(_children).ToList())
		{
			UnassignDweller(item);
		}
	}

	public IEnumerable<BaseComponent> GetRelations()
	{
		foreach (Dweller child in _children)
		{
			yield return child;
		}
		foreach (Dweller adult in _adults)
		{
			yield return adult;
		}
	}
}
