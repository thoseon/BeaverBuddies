using System.Collections.Generic;

namespace Timberborn.TimbermeshMaterials;

public interface IMaterialCollectionIdsProvider
{
	IEnumerable<string> GetMaterialCollectionIds();
}
