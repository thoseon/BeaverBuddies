using System.Collections.Generic;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Gathering;
using Timberborn.Growing;
using Timberborn.NaturalResources;
using UnityEngine;

namespace Timberborn.MapEditorNaturalResources;

public class NaturalResourceSpawner
{
	private readonly NaturalResourceFactory _naturalResourceFactory;

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private readonly IBlockService _blockService;

	public bool RandomizeYieldGrowth { get; set; } = true;

	public NaturalResourceSpawner(NaturalResourceFactory naturalResourceFactory, IRandomNumberGenerator randomNumberGenerator, IBlockService blockService)
	{
		_naturalResourceFactory = naturalResourceFactory;
		_randomNumberGenerator = randomNumberGenerator;
		_blockService = blockService;
	}

	public void Spawn(IEnumerable<SpawnableResource> spawnableResources, Vector3Int coordinates)
	{
		if (!_blockService.AnyObjectAt(coordinates))
		{
			SpawnableResource enumerableElement = _randomNumberGenerator.GetEnumerableElement(spawnableResources);
			NaturalResource naturalResource = _naturalResourceFactory.SpawnIgnoringConstraintsAndRandomizePosition(enumerableElement.Id, coordinates);
			if ((bool)naturalResource)
			{
				bool mature = !enumerableElement.IsSeedling;
				SetGrowStage(naturalResource, mature);
				SetGatherableYieldGrowStage(naturalResource, mature);
			}
		}
	}

	private void SetGrowStage(NaturalResource naturalResource, bool mature)
	{
		Growable component = naturalResource.GetComponent<Growable>();
		if (component != null)
		{
			float growthProgress = (mature ? 1f : _randomNumberGenerator.Range(0f, 0.8f));
			component.IncreaseGrowthProgress(growthProgress);
		}
	}

	private void SetGatherableYieldGrowStage(NaturalResource naturalResource, bool mature)
	{
		if (mature)
		{
			GatherableYieldGrower component = naturalResource.GetComponent<GatherableYieldGrower>();
			if (component != null && component.GetComponent<Gatherable>().UsableWithCurrentFeatureToggles)
			{
				int num = (RandomizeYieldGrowth ? _randomNumberGenerator.Range(1, 100) : 100);
				component.FastForwardGrowth((float)num / 100f);
			}
		}
	}
}
