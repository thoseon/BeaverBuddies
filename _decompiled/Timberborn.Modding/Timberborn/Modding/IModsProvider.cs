using System.Collections.Generic;

namespace Timberborn.Modding;

public interface IModsProvider
{
	IEnumerable<ModDirectory> GetModDirectories();
}
