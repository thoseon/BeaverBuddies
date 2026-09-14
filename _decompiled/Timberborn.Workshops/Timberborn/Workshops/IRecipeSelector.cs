using System;

namespace Timberborn.Workshops;

public interface IRecipeSelector
{
	bool HasCurrentRecipe { get; }

	event EventHandler RecipeChanged;
}
