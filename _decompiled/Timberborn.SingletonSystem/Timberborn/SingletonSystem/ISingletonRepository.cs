using System.Collections.Generic;

namespace Timberborn.SingletonSystem;

public interface ISingletonRepository
{
	IEnumerable<T> GetSingletons<T>();
}
