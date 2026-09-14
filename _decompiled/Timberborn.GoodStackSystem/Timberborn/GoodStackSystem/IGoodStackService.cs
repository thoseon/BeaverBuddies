using Timberborn.Common;

namespace Timberborn.GoodStackSystem;

public interface IGoodStackService
{
	ReadOnlyList<GoodStack> GoodStacks { get; }
}
