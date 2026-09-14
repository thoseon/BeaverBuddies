using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.UnityEngineSpecs;

public record AnimationKeyframeSpec
{
	[Serialize]
	public float Time { get; init; }

	[Serialize]
	public float Value { get; init; }

	[Serialize]
	public float InTangent { get; init; }

	[Serialize]
	public float OutTangent { get; init; }

	[Serialize]
	public int WeightedMode { get; init; }

	[Serialize]
	public float InWeight { get; init; }

	[Serialize]
	public float OutWeight { get; init; }

	public static AnimationKeyframeSpec FromKeyframe(Keyframe keyframe)
	{
		return new AnimationKeyframeSpec
		{
			Time = keyframe.time,
			Value = keyframe.value,
			InTangent = keyframe.inTangent,
			OutTangent = keyframe.outTangent,
			WeightedMode = (int)keyframe.weightedMode,
			InWeight = keyframe.inWeight,
			OutWeight = keyframe.outWeight
		};
	}

	public Keyframe ToKeyframe()
	{
		return new Keyframe
		{
			time = Time,
			value = Value,
			inTangent = InTangent,
			outTangent = OutTangent,
			weightedMode = (WeightedMode)WeightedMode,
			inWeight = InWeight,
			outWeight = OutWeight
		};
	}
}
