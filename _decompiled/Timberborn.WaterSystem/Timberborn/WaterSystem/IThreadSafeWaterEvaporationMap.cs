using Timberborn.Common;

namespace Timberborn.WaterSystem;

public interface IThreadSafeWaterEvaporationMap
{
	ReadOnlyArray<float> EvaporationModifiers { get; }
}
