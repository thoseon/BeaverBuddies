using System.Collections.Generic;
using System.Linq;

namespace Timberborn.Common;

public static class Enumerables
{
	public static IEnumerable<T> One<T>(T item)
	{
		return Enumerable.Repeat(item, 1);
	}
}
