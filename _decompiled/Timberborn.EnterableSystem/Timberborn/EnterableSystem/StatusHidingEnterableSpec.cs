using JetBrains.Annotations;
using Timberborn.BlueprintSystem;

namespace Timberborn.EnterableSystem;

[UsedImplicitly]
internal record StatusHidingEnterableSpec : ComponentSpec, IStatusHider;
