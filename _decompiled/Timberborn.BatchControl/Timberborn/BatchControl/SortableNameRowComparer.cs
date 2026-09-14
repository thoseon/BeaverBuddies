using System.Collections.Generic;
using Timberborn.EntityNaming;

namespace Timberborn.BatchControl;

internal class SortableNameRowComparer : IComparer<BatchControlRow>
{
	public int Compare(BatchControlRow x, BatchControlRow y)
	{
		if (x.Entity != y.Entity)
		{
			return x.Entity.GetComponent<NamedEntity>().SortingKey.CompareTo(y.Entity.GetComponent<NamedEntity>().SortingKey);
		}
		return 0;
	}
}
