using System.Collections.Generic;
using System.Linq;

namespace Timberborn.SingletonSystem;

internal class SingletonRepository : ISingletonRepository
{
	private readonly SingletonListener _singletonListener;

	public SingletonRepository(SingletonListener singletonListener)
	{
		_singletonListener = singletonListener;
	}

	public IEnumerable<T> GetSingletons<T>()
	{
		return _singletonListener.Collect().OfType<T>();
	}
}
