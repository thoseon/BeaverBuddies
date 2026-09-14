using System.Collections.Immutable;
using System.Linq;
using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.UnityEngineSpecs;

public record AnimationCurveSpec
{
	[Serialize]
	public ImmutableArray<AnimationKeyframeSpec> Keys { get; init; }

	[Serialize]
	public WrapMode PreWrapMode { get; init; }

	[Serialize]
	public WrapMode PostWrapMode { get; init; }

	public static AnimationCurveSpec FromAnimationCurve(AnimationCurve curve)
	{
		return new AnimationCurveSpec
		{
			Keys = curve.keys.Select(AnimationKeyframeSpec.FromKeyframe).ToImmutableArray(),
			PreWrapMode = curve.preWrapMode,
			PostWrapMode = curve.postWrapMode
		};
	}

	public AnimationCurve ToAnimationCurve()
	{
		return new AnimationCurve(Keys.Select((AnimationKeyframeSpec key) => key.ToKeyframe()).ToArray())
		{
			preWrapMode = PreWrapMode,
			postWrapMode = PostWrapMode
		};
	}
}
