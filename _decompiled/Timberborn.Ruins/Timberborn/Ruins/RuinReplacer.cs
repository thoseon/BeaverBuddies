using System;
using System.Collections.Immutable;
using Timberborn.BlockSystem;
using Timberborn.Coordinates;
using Timberborn.EntitySystem;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;
using Timberborn.TemplateSystem;

namespace Timberborn.Ruins;

public class RuinReplacer : ILoadableSingleton
{
	private readonly BlockObjectFactory _blockObjectFactory;

	private readonly TemplateService _templateService;

	private readonly EntityService _entityService;

	private readonly EntitySelectionService _entitySelectionService;

	private ImmutableArray<RuinSpec> _ruinTemplates;

	public RuinReplacer(BlockObjectFactory blockObjectFactory, TemplateService templateService, EntityService entityService, EntitySelectionService entitySelectionService)
	{
		_blockObjectFactory = blockObjectFactory;
		_templateService = templateService;
		_entityService = entityService;
		_entitySelectionService = entitySelectionService;
	}

	public void Load()
	{
		_ruinTemplates = _templateService.GetAll<RuinSpec>().ToImmutableArray();
	}

	public void Shuffle(Ruin originalRuin)
	{
		bool wasSelected = _entitySelectionService.IsSelected(originalRuin.GetComponent<SelectableObject>());
		_entityService.Delete(originalRuin);
		RuinSpec ruinForHeight = GetRuinForHeight(originalRuin.SpecifiedHeight);
		Instantiate(ruinForHeight, originalRuin, null, null, wasSelected);
	}

	public void Shrink(Ruin originalRuin)
	{
		bool wasSelected = _entitySelectionService.IsSelected(originalRuin.GetComponent<SelectableObject>());
		int amount = originalRuin.Yielder.Yield.Amount;
		_entityService.Delete(originalRuin);
		if (TryGetNextRuin(originalRuin, out var nextRuin))
		{
			Instantiate(nextRuin, originalRuin, amount, originalRuin.GetComponent<RuinModels>().VariantId, wasSelected);
		}
	}

	private bool TryGetNextRuin(Ruin originalRuin, out RuinSpec nextRuin)
	{
		int num = originalRuin.SpecifiedHeight - 1;
		if (num == 0)
		{
			nextRuin = null;
			return false;
		}
		nextRuin = GetRuinForHeight(num);
		return true;
	}

	private RuinSpec GetRuinForHeight(int nextHeight)
	{
		foreach (RuinSpec ruinTemplate in _ruinTemplates)
		{
			if (ruinTemplate.RuinHeight == nextHeight)
			{
				return ruinTemplate;
			}
		}
		throw new ArgumentException("No ruin template found for height " + nextHeight);
	}

	private void Instantiate(RuinSpec nextRuinTemplate, Ruin originalRuin, int? initialYield, string variantId, bool wasSelected)
	{
		RuinInit initComponent = new RuinInit(initialYield, variantId, wasSelected);
		EntitySetup.Builder entitySetupBuilder = new EntitySetup.Builder(nextRuinTemplate.Blueprint).AddInitComponent(initComponent);
		Placement placement = originalRuin.GetComponent<BlockObject>().Placement;
		_blockObjectFactory.CreateFinished(entitySetupBuilder, placement);
	}
}
