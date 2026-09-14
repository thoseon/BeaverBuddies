using System.Collections.Generic;

namespace Timberborn.TimbermeshMaterials;

internal class CommonMaterialCollectionIdsProvider : IMaterialCollectionIdsProvider
{
	private static readonly string CommonCollectionId = "Common";

	public IEnumerable<string> GetMaterialCollectionIds()
	{
		yield return CommonCollectionId;
	}
}
