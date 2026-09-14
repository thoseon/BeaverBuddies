using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BlueprintSystem;
using Timberborn.FactionSystem;
using Timberborn.GoodCollectionSystem;

namespace Timberborn.FactionValidators;

internal class FactionSpecGoodsValidator : IFactionSpecValidator
{
	private readonly ISpecService _specService;

	public FactionSpecGoodsValidator(ISpecService specService)
	{
		_specService = specService;
	}

	public bool IsValid(FactionSpec faction, out string errorMessage)
	{
		ImmutableArray<string> goodCollectionIds = faction.GoodCollectionIds;
		List<string> list = (from @group in _specService.GetSpecs<GoodCollectionSpec>()
			select @group.CollectionId).ToList();
		foreach (string item in goodCollectionIds)
		{
			if (!list.Contains(item))
			{
				errorMessage = "GoodCollectionSpec with id  " + item + " not found";
				return false;
			}
		}
		errorMessage = null;
		return true;
	}
}
