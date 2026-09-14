using System.Collections.Generic;
using Timberborn.TemplateCollectionSystem;

namespace Timberborn.GameFactionSystem;

internal class FactionTemplateCollectionIdProvider : ITemplateCollectionIdProvider
{
	private readonly FactionService _factionService;

	public FactionTemplateCollectionIdProvider(FactionService factionService)
	{
		_factionService = factionService;
	}

	public IEnumerable<string> GetTemplateCollectionIds()
	{
		return _factionService.Current.TemplateCollectionIds;
	}
}
