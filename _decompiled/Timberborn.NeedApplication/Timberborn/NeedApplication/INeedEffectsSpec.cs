using System.Collections.Immutable;

namespace Timberborn.NeedApplication;

public interface INeedEffectsSpec
{
	ImmutableArray<NeedApplierEffectSpec> Effects { get; }
}
