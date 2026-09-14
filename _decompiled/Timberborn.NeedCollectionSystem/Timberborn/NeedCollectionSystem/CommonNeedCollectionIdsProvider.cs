using System.Collections.Generic;

namespace Timberborn.NeedCollectionSystem;

public class CommonNeedCollectionIdsProvider : INeedCollectionIdsProvider
{
	private static readonly string CommonCollectionId = "Common";

	public IEnumerable<string> GetNeedCollectionIds()
	{
		yield return CommonCollectionId;
	}
}
