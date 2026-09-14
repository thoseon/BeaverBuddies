using System.Collections.Generic;
using Timberborn.NeedCollectionSystem;

namespace Timberborn.GameFactionSystem;

internal class FactionNeedCollectionIdsProvider : INeedCollectionIdsProvider
{
	private readonly FactionService _factionService;

	public FactionNeedCollectionIdsProvider(FactionService factionService)
	{
		_factionService = factionService;
	}

	public IEnumerable<string> GetNeedCollectionIds()
	{
		foreach (string needCollectionId in _factionService.Current.NeedCollectionIds)
		{
			yield return needCollectionId;
		}
	}
}
