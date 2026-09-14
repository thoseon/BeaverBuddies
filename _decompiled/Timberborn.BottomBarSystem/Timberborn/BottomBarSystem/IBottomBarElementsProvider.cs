using System.Collections.Generic;

namespace Timberborn.BottomBarSystem;

public interface IBottomBarElementsProvider
{
	IEnumerable<BottomBarElement> GetElements();
}
