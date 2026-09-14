using System.Collections.Generic;
using Timberborn.Common;

namespace Timberborn.GoodStackSystem;

public class GoodStackService<T> : IGoodStackService
{
	private readonly List<GoodStack> _goodStacks = new List<GoodStack>();

	public ReadOnlyList<GoodStack> GoodStacks => _goodStacks.AsReadOnlyList();

	public void Add(GoodStack goodStack)
	{
		_goodStacks.Add(goodStack);
	}

	public void Remove(GoodStack goodStack)
	{
		_goodStacks.Remove(goodStack);
	}
}
