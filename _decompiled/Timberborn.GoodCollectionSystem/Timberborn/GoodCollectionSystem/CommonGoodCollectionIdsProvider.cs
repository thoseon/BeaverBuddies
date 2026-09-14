using System.Collections.Generic;

namespace Timberborn.GoodCollectionSystem;

public class CommonGoodCollectionIdsProvider : IGoodCollectionIdsProvider
{
	private static readonly string CommonCollectionId = "Common";

	public IEnumerable<string> GetGoodCollectionIds()
	{
		yield return CommonCollectionId;
	}
}
