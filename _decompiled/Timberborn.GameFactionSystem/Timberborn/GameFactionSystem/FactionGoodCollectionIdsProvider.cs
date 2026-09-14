using System.Collections.Generic;
using Timberborn.GoodCollectionSystem;

namespace Timberborn.GameFactionSystem;

internal class FactionGoodCollectionIdsProvider : IGoodCollectionIdsProvider
{
	private readonly FactionService _factionService;

	public FactionGoodCollectionIdsProvider(FactionService factionService)
	{
		_factionService = factionService;
	}

	public IEnumerable<string> GetGoodCollectionIds()
	{
		foreach (string goodCollectionId in _factionService.Current.GoodCollectionIds)
		{
			yield return goodCollectionId;
		}
	}
}
