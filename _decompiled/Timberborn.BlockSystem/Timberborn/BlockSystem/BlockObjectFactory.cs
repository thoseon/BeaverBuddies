using Timberborn.BaseComponentSystem;
using Timberborn.Coordinates;
using Timberborn.EntitySystem;
using Timberborn.TemplateInstantiation;
using UnityEngine;

namespace Timberborn.BlockSystem;

public class BlockObjectFactory
{
	private readonly EntityService _entityService;

	private readonly TemplateInstantiator _templateInstantiator;

	public BlockObjectFactory(EntityService entityService, TemplateInstantiator templateInstantiator)
	{
		_entityService = entityService;
		_templateInstantiator = templateInstantiator;
	}

	public BlockObject CreateUnfinished(EntitySetup.Builder entitySetupBuilder, Placement placement)
	{
		return Create(entitySetupBuilder, placement, markFinished: false);
	}

	public BlockObject CreateFinished(EntitySetup.Builder entitySetupBuilder, Placement placement)
	{
		return Create(entitySetupBuilder, placement, markFinished: true);
	}

	public BlockObject CreateAsPreview(BlockObjectSpec template, Transform parent, Placement placement)
	{
		BlockObject componentSlow = _templateInstantiator.Instantiate(template.Blueprint, parent).GetComponentSlow<BlockObject>();
		componentSlow.MarkAsPreview();
		componentSlow.Reposition(placement);
		return componentSlow;
	}

	private BlockObject Create(EntitySetup.Builder entitySetupBuilder, Placement placement, bool markFinished)
	{
		entitySetupBuilder.AddInitComponent(new BlockObjectInit(placement, markFinished));
		return _entityService.Instantiate(entitySetupBuilder).GetComponent<BlockObject>();
	}
}
