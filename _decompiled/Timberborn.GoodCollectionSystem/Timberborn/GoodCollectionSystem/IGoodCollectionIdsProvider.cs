using System.Collections.Generic;

namespace Timberborn.GoodCollectionSystem;

public interface IGoodCollectionIdsProvider
{
	IEnumerable<string> GetGoodCollectionIds();
}
