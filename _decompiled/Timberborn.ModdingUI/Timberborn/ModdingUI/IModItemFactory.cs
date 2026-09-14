using System;
using Timberborn.Modding;

namespace Timberborn.ModdingUI;

public interface IModItemFactory
{
	ModItem CreateModItem(Mod mod, Action<Mod, bool> onPriorityIncreased, Action<Mod, bool> onPriorityDecreased);
}
