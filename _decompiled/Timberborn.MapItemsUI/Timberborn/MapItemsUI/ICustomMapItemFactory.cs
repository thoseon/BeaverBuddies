using System.Collections.Generic;

namespace Timberborn.MapItemsUI;

public interface ICustomMapItemFactory
{
	IEnumerable<MapItem> Create();
}
