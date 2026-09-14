using System.Collections.Immutable;
using Timberborn.Goods;

namespace Timberborn.RecoveredGoodSystem;

internal record RecoveredGoodStackInit(ImmutableArray<GoodAmount> GoodAmounts);
