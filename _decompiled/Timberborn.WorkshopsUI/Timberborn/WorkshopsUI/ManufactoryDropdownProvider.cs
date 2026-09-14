using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.DropdownSystem;
using Timberborn.EntitySystem;
using Timberborn.StatusSystemUI;
using Timberborn.Workshops;
using UnityEngine;

namespace Timberborn.WorkshopsUI;

internal class ManufactoryDropdownProvider : BaseComponent, IAwakableComponent, IInitializableEntity, IExtendedDropdownProvider, IDropdownProvider
{
	private static readonly string NoRecipeItemLocKey = "Manufactory.NoRecipeOption";

	private readonly StatusListFragment _statusListFragment;

	private Manufactory _manufactory;

	private readonly List<string> _items = new List<string>();

	public IReadOnlyList<string> Items => _items.AsReadOnlyList();

	public ManufactoryDropdownProvider(StatusListFragment statusListFragment)
	{
		_statusListFragment = statusListFragment;
	}

	public void Awake()
	{
		_manufactory = GetComponent<Manufactory>();
	}

	public void InitializeEntity()
	{
		ImmutableArray<RecipeSpec> productionRecipes = _manufactory.ProductionRecipes;
		_items.Add(NoRecipeItemLocKey);
		_items.AddRange(productionRecipes.Select((RecipeSpec recipe) => recipe.DisplayLocKey));
	}

	public string GetValue()
	{
		return _manufactory.CurrentRecipe?.DisplayLocKey ?? NoRecipeItemLocKey;
	}

	public void SetValue(string value)
	{
		RecipeSpec recipeSpec = GetRecipeSpec(value);
		_manufactory.SetRecipe(recipeSpec);
		_statusListFragment.UpdateFragment();
	}

	public string FormatDisplayText(string value, bool selected)
	{
		return value;
	}

	public Sprite GetIcon(string value)
	{
		return GetRecipeSpec(value)?.UIIcon.Value;
	}

	public ImmutableArray<string> GetItemClasses(string value)
	{
		return ImmutableArray<string>.Empty;
	}

	private RecipeSpec GetRecipeSpec(string value)
	{
		return _manufactory.ProductionRecipes.SingleOrDefault((RecipeSpec recipe) => recipe.DisplayLocKey == value);
	}
}
