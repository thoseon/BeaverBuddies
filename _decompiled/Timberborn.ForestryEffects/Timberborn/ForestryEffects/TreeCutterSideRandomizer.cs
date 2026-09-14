using System;
using Timberborn.BaseComponentSystem;
using Timberborn.CharacterModelSystem;
using Timberborn.Common;
using Timberborn.Forestry;

namespace Timberborn.ForestryEffects;

internal class TreeCutterSideRandomizer : BaseComponent, IAwakableComponent
{
	private static readonly string FlipAnimationParameter = "CuttingFlipped";

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private CharacterAnimator _characterAnimator;

	public TreeCutterSideRandomizer(IRandomNumberGenerator randomNumberGenerator)
	{
		_randomNumberGenerator = randomNumberGenerator;
	}

	public void Awake()
	{
		_characterAnimator = GetComponent<CharacterAnimator>();
		TreeCutter component = GetComponent<TreeCutter>();
		component.CuttingStarted += RandomizeCuttingSide;
		component.CuttingStopped += ClearMirroredCutting;
	}

	private void RandomizeCuttingSide(object sender, EventArgs e)
	{
		_characterAnimator.SetBool(FlipAnimationParameter, _randomNumberGenerator.CheckProbability(0.5f));
	}

	private void ClearMirroredCutting(object sender, EventArgs e)
	{
		_characterAnimator.SetBool(FlipAnimationParameter, value: false);
	}
}
