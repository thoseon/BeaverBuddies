using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BlueprintSystem;
using Timberborn.FactionSystem;
using Timberborn.TemplateCollectionSystem;

namespace Timberborn.FactionValidators;

internal class FactionSpecTemplateValidator : IFactionSpecValidator
{
	private readonly ISpecService _specService;

	public FactionSpecTemplateValidator(ISpecService specService)
	{
		_specService = specService;
	}

	public bool IsValid(FactionSpec faction, out string errorMessage)
	{
		ImmutableArray<string> templateCollectionIds = faction.TemplateCollectionIds;
		List<string> list = (from @group in _specService.GetSpecs<TemplateCollectionSpec>()
			select @group.CollectionId).ToList();
		foreach (string item in templateCollectionIds)
		{
			if (!list.Contains(item))
			{
				errorMessage = "TemplateCollectionSpec with id " + item + " not found";
				return false;
			}
		}
		errorMessage = null;
		return true;
	}
}
