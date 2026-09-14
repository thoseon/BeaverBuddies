using System.Collections.Frozen;
using System.Collections.Generic;
using Timberborn.BlueprintSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.Workshops;

public class RecipeSpecService : ILoadableSingleton
{
	private readonly ISpecService _specService;

	private FrozenDictionary<string, RecipeSpec> _recipeSpecs;

	public RecipeSpecService(ISpecService specService)
	{
		_specService = specService;
	}

	public void Load()
	{
		_recipeSpecs = _specService.GetSpecs<RecipeSpec>().ToFrozenDictionary((RecipeSpec spec) => spec.Id, (RecipeSpec spec) => spec);
	}

	public RecipeSpec GetRecipe(string recipeId)
	{
		return _recipeSpecs[recipeId];
	}

	public IEnumerable<RecipeSpec> GetRecipes()
	{
		return _recipeSpecs.Values;
	}
}
