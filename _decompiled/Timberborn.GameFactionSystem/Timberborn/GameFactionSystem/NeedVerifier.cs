using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BlueprintSystem;
using Timberborn.FactionSystem;
using Timberborn.NeedCollectionSystem;
using Timberborn.NeedSpecs;
using Timberborn.SingletonSystem;

namespace Timberborn.GameFactionSystem;

public class NeedVerifier : ILoadableSingleton
{
	private readonly FactionSpecService _factionSpecService;

	private readonly NeedGroupSpecService _needGroupSpecService;

	private readonly ISpecService _specService;

	private readonly CommonNeedCollectionIdsProvider _commonNeedCollectionIdsProvider;

	public NeedVerifier(FactionSpecService factionSpecService, NeedGroupSpecService needGroupSpecService, ISpecService specService, CommonNeedCollectionIdsProvider commonNeedCollectionIdsProvider)
	{
		_factionSpecService = factionSpecService;
		_needGroupSpecService = needGroupSpecService;
		_specService = specService;
		_commonNeedCollectionIdsProvider = commonNeedCollectionIdsProvider;
	}

	public void Load()
	{
		List<NeedSpec> needSpecs = _specService.GetSpecs<NeedSpec>().ToList();
		ImmutableArray<FactionSpec> factions = _factionSpecService.Factions;
		List<NeedCollectionSpec> needCollections = _specService.GetSpecs<NeedCollectionSpec>().ToList();
		VerifyAllNeedsAreUsed(needSpecs, needCollections);
		VerifyAllCollectionsAreUsed(needCollections, factions);
		VerifyAllNeedsExist(needSpecs, needCollections);
		VerifyGroupOfNeeds(needSpecs);
	}

	private static void VerifyAllNeedsAreUsed(IEnumerable<NeedSpec> needSpecs, ICollection<NeedCollectionSpec> needCollections)
	{
		foreach (NeedSpec needSpec in needSpecs)
		{
			if (!needCollections.Any((NeedCollectionSpec needCollection) => needCollection.Needs.Contains(needSpec.Id)))
			{
				throw new Exception("NeedSpec with id " + needSpec.Id + " is not used!");
			}
		}
	}

	private void VerifyAllCollectionsAreUsed(ICollection<NeedCollectionSpec> needCollections, ICollection<FactionSpec> allFactionSpecs)
	{
		string text = _commonNeedCollectionIdsProvider.GetNeedCollectionIds().Single();
		foreach (string needCollectionId in needCollections.Select((NeedCollectionSpec needCollection) => needCollection.CollectionId).Distinct())
		{
			if (needCollectionId != text && !allFactionSpecs.Any((FactionSpec faction) => faction.NeedCollectionIds.Contains(needCollectionId)))
			{
				throw new Exception("NeedCollectionSpec with id " + needCollectionId + " is not used!");
			}
		}
	}

	private static void VerifyAllNeedsExist(IReadOnlyCollection<NeedSpec> needSpecs, IEnumerable<NeedCollectionSpec> needCollections)
	{
		foreach (NeedCollectionSpec needCollection in needCollections)
		{
			foreach (string need in needCollection.Needs)
			{
				if (needSpecs.All((NeedSpec needSpec) => needSpec.Id != need))
				{
					throw new Exception("There is no NeedSpec with id " + need);
				}
			}
		}
	}

	private void VerifyGroupOfNeeds(IEnumerable<NeedSpec> needSpecs)
	{
		foreach (NeedSpec needSpec in needSpecs)
		{
			VerifyGroupOfNeed(needSpec);
		}
	}

	private void VerifyGroupOfNeed(NeedSpec needSpec)
	{
		string needGroupId = needSpec.NeedGroupId;
		if (!_needGroupSpecService.IsValidGroup(needGroupId))
		{
			throw new Exception("There is no NeedGroupSpec with id " + needGroupId);
		}
	}
}
