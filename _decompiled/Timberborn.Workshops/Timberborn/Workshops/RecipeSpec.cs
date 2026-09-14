using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using Timberborn.Common;
using Timberborn.Goods;
using Timberborn.SpriteOperations;
using UnityEngine;

namespace Timberborn.Workshops;

public record RecipeSpec : ComponentSpec
{
	[Serialize]
	public string Id { get; init; }

	[Serialize]
	public ImmutableArray<string> BackwardCompatibleIds { get; init; }

	[Serialize]
	public string DisplayLocKey { get; init; }

	[Serialize]
	public float CycleDurationInHours { get; init; }

	[Serialize]
	public ImmutableArray<GoodAmountSpec> Ingredients { get; init; }

	[Serialize]
	public ImmutableArray<GoodAmountSpec> Products { get; init; }

	[Serialize]
	public int ProducedSciencePoints { get; init; }

	[Serialize]
	public string Fuel { get; init; }

	[Serialize]
	public int CyclesFuelLasts { get; init; }

	[Serialize]
	public int FuelCapacity { get; init; }

	[Serialize("Icon")]
	public UISprite UIIcon { get; init; }

	[Serialize]
	private int CyclesCapacity { get; init; }

	[Serialize]
	private AssetRef<Sprite> Icon { get; init; }

	public bool ProducesProducts => !Products.IsEmpty();

	public bool ProducesSciencePoints => ProducedSciencePoints > 0;

	public bool ConsumesIngredients => !Ingredients.IsEmpty();

	public bool ConsumesFuel => FuelCapacity > 0;

	public int GetCapacity(GoodAmount goodAmount)
	{
		return goodAmount.Amount * CyclesCapacity;
	}
}
