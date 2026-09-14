using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.DropdownSystem;
using Timberborn.EntitySystem;
using Timberborn.GoodsUI;
using Timberborn.Planting;
using UnityEngine;

namespace Timberborn.PlantingUI;

internal class PlantablePrioritizerDropdownProvider : BaseComponent, IAwakableComponent, IInitializableEntity, IExtendedDropdownProvider, IDropdownProvider
{
	private static readonly string NoPriorityItemLocKey = "Planting.NoPriorityOption";

	private readonly GoodDescriber _goodDescriber;

	private PlantablePrioritizer _plantablePrioritizer;

	private PlanterBuilding _planterBuilding;

	private readonly List<string> _items = new List<string>();

	public IReadOnlyList<string> Items => _items.AsReadOnlyList();

	public bool HasMultipleOptions => _planterBuilding.AllowedPlantables.Length > 1;

	public PlantablePrioritizerDropdownProvider(GoodDescriber goodDescriber)
	{
		_goodDescriber = goodDescriber;
	}

	public void Awake()
	{
		_plantablePrioritizer = GetComponent<PlantablePrioritizer>();
		_planterBuilding = GetComponent<PlanterBuilding>();
	}

	public void InitializeEntity()
	{
		ImmutableArray<PlantableSpec> allowedPlantables = _planterBuilding.AllowedPlantables;
		_items.Add(NoPriorityItemLocKey);
		_items.AddRange(allowedPlantables.Select(PlantableLocKey));
	}

	public string GetValue()
	{
		if (!(_plantablePrioritizer.PrioritizedPlantableSpec != null))
		{
			return NoPriorityItemLocKey;
		}
		return PlantableLocKey(_plantablePrioritizer.PrioritizedPlantableSpec);
	}

	public void SetValue(string value)
	{
		PlantableSpec prioritizedPlantable = GetPrioritizedPlantable(value);
		_plantablePrioritizer.PrioritizePlantable(prioritizedPlantable);
	}

	public string FormatDisplayText(string value, bool selected)
	{
		return value;
	}

	public Sprite GetIcon(string value)
	{
		PlantableSpec prioritizedPlantable = GetPrioritizedPlantable(value);
		if (prioritizedPlantable != null)
		{
			return GetPlantableIcon(prioritizedPlantable);
		}
		return null;
	}

	public ImmutableArray<string> GetItemClasses(string value)
	{
		return ImmutableArray<string>.Empty;
	}

	private static string PlantableLocKey(PlantableSpec plantableSpec)
	{
		return plantableSpec.GetSpec<LabeledEntitySpec>().DisplayNameLocKey;
	}

	private PlantableSpec GetPrioritizedPlantable(string value)
	{
		return _planterBuilding.AllowedPlantables.SingleOrDefault((PlantableSpec plantable) => plantable.GetSpec<LabeledEntitySpec>().DisplayNameLocKey == value);
	}

	private Sprite GetPlantableIcon(PlantableSpec plantableSpec)
	{
		IPlantableGoodIdProvider spec = plantableSpec.GetSpec<IPlantableGoodIdProvider>();
		if (spec != null)
		{
			string goodId = spec.GetGoodId();
			return _goodDescriber.GetIcon(goodId);
		}
		return null;
	}
}
