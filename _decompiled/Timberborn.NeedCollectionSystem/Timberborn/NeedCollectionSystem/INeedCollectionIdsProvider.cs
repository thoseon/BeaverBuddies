using System.Collections.Generic;

namespace Timberborn.NeedCollectionSystem;

public interface INeedCollectionIdsProvider
{
	IEnumerable<string> GetNeedCollectionIds();
}
