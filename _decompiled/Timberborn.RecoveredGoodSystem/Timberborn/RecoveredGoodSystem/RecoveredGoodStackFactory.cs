using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BlockSystem;
using Timberborn.BlueprintSystem;
using Timberborn.Coordinates;
using Timberborn.EntitySystem;
using Timberborn.Goods;
using Timberborn.SingletonSystem;
using Timberborn.TemplateSystem;
using UnityEngine;

namespace Timberborn.RecoveredGoodSystem;

internal class RecoveredGoodStackFactory : ILoadableSingleton
{
	private readonly BlockObjectFactory _blockObjectFactory;

	private readonly TemplateService _templateService;

	private Blueprint _recoveredGoodStackTemplate;

	public BlockSpec GoodStackBlockSpec { get; private set; }

	public RecoveredGoodStackFactory(BlockObjectFactory blockObjectFactory, TemplateService templateService)
	{
		_blockObjectFactory = blockObjectFactory;
		_templateService = templateService;
	}

	public void Load()
	{
		_recoveredGoodStackTemplate = _templateService.GetSingle<RecoveredGoodStackSpec>().Blueprint;
		GoodStackBlockSpec = _recoveredGoodStackTemplate.GetSpec<BlockObjectSpec>().Blocks.Single();
	}

	public void Create(Vector3Int coordinate, IEnumerable<GoodAmount> recoveredGoods)
	{
		EntitySetup.Builder entitySetupBuilder = new EntitySetup.Builder(_recoveredGoodStackTemplate).AddInitComponent(new RecoveredGoodStackInit(recoveredGoods.ToImmutableArray()));
		_blockObjectFactory.CreateFinished(entitySetupBuilder, new Placement(coordinate));
	}
}
