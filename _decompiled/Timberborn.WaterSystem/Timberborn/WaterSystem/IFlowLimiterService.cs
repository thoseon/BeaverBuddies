using System;
using Timberborn.Common;

namespace Timberborn.WaterSystem;

public interface IFlowLimiterService
{
	ReadOnlyArray<int> LimitedDirections { get; }

	ReadOnlyArray<float> HeightLimits { get; }

	event EventHandler<int> HeightLimitValueChanged;
}
