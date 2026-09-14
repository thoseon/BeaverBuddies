using System.Collections.Generic;
using System.Text;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Effects;
using Timberborn.EntityPanelSystem;
using Timberborn.Goods;
using Timberborn.GoodsUI;
using Timberborn.Localization;
using Timberborn.UIFormatters;
using Timberborn.WorkSystem;
using Timberborn.Workshops;
using UnityEngine.UIElements;

namespace Timberborn.WorkshopsUI;

public class ManufactoryDescriber : BaseComponent, IAwakableComponent, IEntityDescriber
{
	private static readonly string ScienceClass = "described-amount--science";

	private static readonly string SciencePointsLocKey = "Science.SciencePoints";

	private readonly GoodEffectDescriber _goodEffectDescriber;

	private readonly ILoc _loc;

	private readonly DescribedAmountFactory _describedAmountFactory;

	private readonly ProductionItemFactory _productionItemFactory;

	private readonly GoodDescriber _goodDescriber;

	private Manufactory _manufactory;

	private Workplace _workplace;

	private BlockObject _blockObject;

	private readonly Phrase _craftingTimePhrase = Phrase.New().FormatHours<float>(GetCraftingTimeFormat);

	public ManufactoryDescriber(GoodEffectDescriber goodEffectDescriber, ILoc loc, DescribedAmountFactory describedAmountFactory, ProductionItemFactory productionItemFactory, GoodDescriber goodService)
	{
		_goodEffectDescriber = goodEffectDescriber;
		_loc = loc;
		_describedAmountFactory = describedAmountFactory;
		_productionItemFactory = productionItemFactory;
		_goodDescriber = goodService;
	}

	public void Awake()
	{
		_manufactory = GetComponent<Manufactory>();
		_workplace = GetComponent<Workplace>();
		_blockObject = GetComponent<BlockObject>();
	}

	public IEnumerable<EntityDescription> DescribeEntity()
	{
		if (!_blockObject.IsPreview)
		{
			yield break;
		}
		foreach (EntityDescription item in DescribeStatistics())
		{
			yield return item;
		}
	}

	public (VisualElement input, VisualElement output) DescribeRecipe(RecipeSpec productionRecipe)
	{
		return (input: _productionItemFactory.CreateInput(GetInputs(productionRecipe)), output: _productionItemFactory.CreateOutput(GetOutputs(productionRecipe)));
	}

	public string GetCraftingTime(RecipeSpec productionRecipe, float workers)
	{
		float param = productionRecipe.CycleDurationInHours / workers;
		return _loc.T(_craftingTimePhrase, param);
	}

	private static string GetCraftingTimeFormat(float craftingTime)
	{
		if (!(craftingTime < 1f))
		{
			if (craftingTime < 10f)
			{
				return "0.#";
			}
			return "F0";
		}
		return "0.##";
	}

	private IEnumerable<EntityDescription> DescribeStatistics()
	{
		for (int i = 0; i < _manufactory.ProductionRecipes.Length; i++)
		{
			RecipeSpec productionRecipe = _manufactory.ProductionRecipes[i];
			int num = ((!_workplace) ? 1 : _workplace.MaxWorkers);
			VisualElement content = DescribeRecipe(productionRecipe, num);
			yield return EntityDescription.CreateInputOutputSection(content, i);
		}
	}

	private VisualElement DescribeRecipe(RecipeSpec productionRecipe, float workers)
	{
		return _productionItemFactory.CreateInputOutput(GetInputs(productionRecipe), GetOutputs(productionRecipe), GetCraftingTime(productionRecipe, workers));
	}

	private IEnumerable<VisualElement> GetInputs(RecipeSpec productionRecipe)
	{
		for (int i = 0; i < productionRecipe.Ingredients.Length; i++)
		{
			GoodAmountSpec goodAmountSpec = productionRecipe.Ingredients[i];
			DescribedGood describedGood = _goodDescriber.GetDescribedGood(goodAmountSpec.Id);
			string tooltip = GetTooltip(describedGood);
			yield return CreateElement(describedGood, goodAmountSpec.Amount, tooltip);
		}
		if (productionRecipe.ConsumesFuel)
		{
			float num = 1f / (float)productionRecipe.CyclesFuelLasts;
			DescribedGood describedGood2 = _goodDescriber.GetDescribedGood(productionRecipe.Fuel);
			string tooltip2 = GetTooltip(describedGood2);
			yield return CreateElement(describedGood2, num.ToString("0.#"), tooltip2);
		}
	}

	private IEnumerable<VisualElement> GetOutputs(RecipeSpec productionRecipe)
	{
		for (int i = 0; i < productionRecipe.Products.Length; i++)
		{
			GoodAmountSpec goodAmountSpec = productionRecipe.Products[i];
			DescribedGood describedGood = _goodDescriber.GetDescribedGood(goodAmountSpec.Id);
			string tooltipWithEffects = GetTooltipWithEffects(goodAmountSpec.Id, describedGood);
			yield return CreateElement(describedGood, goodAmountSpec.Amount, tooltipWithEffects);
		}
		if (productionRecipe.ProducesSciencePoints)
		{
			string amount = productionRecipe.ProducedSciencePoints.ToString();
			string tooltip = _loc.T(SciencePointsLocKey);
			yield return _describedAmountFactory.CreatePlain(ScienceClass, amount, tooltip);
		}
	}

	private static string GetTooltip(DescribedGood good)
	{
		return good.DisplayName;
	}

	private string GetTooltipWithEffects(string goodId, DescribedGood good)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(GetTooltip(good));
		_goodEffectDescriber.DescribeEffects(goodId, stringBuilder);
		return stringBuilder.ToString().TrimEnd();
	}

	private VisualElement CreateElement(DescribedGood good, int amount, string tooltip)
	{
		return CreateElement(good, amount.ToString(), tooltip);
	}

	private VisualElement CreateElement(DescribedGood good, string amount, string tooltip)
	{
		return _describedAmountFactory.CreatePlain("", amount, good.Icon, tooltip);
	}
}
