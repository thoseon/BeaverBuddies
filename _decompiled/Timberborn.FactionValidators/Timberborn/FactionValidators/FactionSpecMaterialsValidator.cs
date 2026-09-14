using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BlueprintSystem;
using Timberborn.FactionSystem;
using Timberborn.TimbermeshMaterials;

namespace Timberborn.FactionValidators;

internal class FactionSpecMaterialsValidator : IFactionSpecValidator
{
	private readonly ISpecService _specService;

	public FactionSpecMaterialsValidator(ISpecService specService)
	{
		_specService = specService;
	}

	public bool IsValid(FactionSpec faction, out string errorMessage)
	{
		ImmutableArray<string> materialCollectionIds = faction.MaterialCollectionIds;
		List<string> list = (from @group in _specService.GetSpecs<MaterialCollectionSpec>()
			select @group.CollectionId).ToList();
		foreach (string item in materialCollectionIds)
		{
			if (!list.Contains(item))
			{
				errorMessage = "MaterialCollectionSpec with id " + item + " not found";
				return false;
			}
		}
		errorMessage = null;
		return true;
	}
}
