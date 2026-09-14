using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BlueprintSystem;
using Timberborn.Common;
using Timberborn.LocalizationSerialization;
using Timberborn.NeedCollectionSystem;
using Timberborn.NeedSpecs;
using Timberborn.SingletonSystem;

namespace Timberborn.GameFactionSystem;

public class FactionNeedService : ILoadableSingleton
{
	private static readonly NeedSpec NullNeedSpec = new NeedSpec
	{
		DisplayName = new LocalizedText("NullNeed"),
		MinimumValue = 0f,
		MaximumValue = 1f
	};

	private readonly NeedModificationService _needModificationService;

	private readonly ISpecService _specService;

	private readonly ImmutableArray<INeedCollectionIdsProvider> _needCollectionIdsProviders;

	private readonly List<NeedSpec> _needs = new List<NeedSpec>();

	public ReadOnlyList<NeedSpec> Needs => _needs.AsReadOnlyList();

	public NeedSpec NullNeed => NullNeedSpec;

	public FactionNeedService(NeedModificationService needModificationService, ISpecService specService, IEnumerable<INeedCollectionIdsProvider> needCollectionIdsProviders)
	{
		_needModificationService = needModificationService;
		_specService = specService;
		_needCollectionIdsProviders = needCollectionIdsProviders.ToImmutableArray();
	}

	public void Load()
	{
		HashSet<string> hashSet = _needCollectionIdsProviders.SelectMany((INeedCollectionIdsProvider provider) => provider.GetNeedCollectionIds()).ToHashSet();
		HashSet<string> hashSet2 = new HashSet<string>();
		foreach (NeedCollectionSpec spec in _specService.GetSpecs<NeedCollectionSpec>())
		{
			if (hashSet.Contains(spec.CollectionId))
			{
				hashSet2.AddRange(spec.Needs);
			}
		}
		foreach (NeedSpec item in from needSpec in _specService.GetSpecs<NeedSpec>()
			orderby needSpec.Order
			select needSpec)
		{
			if (hashSet2.Contains(item.Id) && item.Blueprint.IsAllowedByFeatureToggles())
			{
				_needs.Add(_needModificationService.ModifyIfEligible(item));
			}
		}
	}

	public bool IsCurrentFactionNeed(string id)
	{
		return _needs.Any((NeedSpec need) => need.GetSpec<NeedSpec>().Id == id);
	}

	public NeedSpec GetBeaverOrBotNeedById(string id)
	{
		NeedSpec needSpec = GetBeaverNeeds().SingleOrDefault((NeedSpec need) => need.Id == id) ?? GetBotNeeds().SingleOrDefault((NeedSpec need) => need.Id == id);
		if (needSpec != null)
		{
			return needSpec;
		}
		throw new InvalidOperationException("Need with id " + id + " not found or multiple needs found");
	}

	public IEnumerable<NeedSpec> GetBeaverNeeds()
	{
		return _needs.Where((NeedSpec need) => need.CharacterType == "Beaver");
	}

	public IEnumerable<NeedSpec> GetBotNeeds()
	{
		return _needs.Where((NeedSpec need) => need.CharacterType == "Bot");
	}
}
