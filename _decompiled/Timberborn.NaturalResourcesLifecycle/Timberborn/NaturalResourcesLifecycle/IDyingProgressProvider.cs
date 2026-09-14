using System;

namespace Timberborn.NaturalResourcesLifecycle;

public interface IDyingProgressProvider
{
	DyingProgress DyingProgress { get; }

	event EventHandler StartedDying;

	event EventHandler StoppedDying;
}
