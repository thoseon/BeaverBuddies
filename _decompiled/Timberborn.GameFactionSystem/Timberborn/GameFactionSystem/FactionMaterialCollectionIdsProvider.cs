using System.Collections.Generic;
using Timberborn.TimbermeshMaterials;

namespace Timberborn.GameFactionSystem;

internal class FactionMaterialCollectionIdsProvider : IMaterialCollectionIdsProvider
{
	private readonly FactionService _factionService;

	public FactionMaterialCollectionIdsProvider(FactionService factionService)
	{
		_factionService = factionService;
	}

	public IEnumerable<string> GetMaterialCollectionIds()
	{
		foreach (string materialCollectionId in _factionService.Current.MaterialCollectionIds)
		{
			yield return materialCollectionId;
		}
	}
}
