using System;
using Timberborn.BaseComponentSystem;
using Timberborn.Forestry;
using Timberborn.WalkingSystem;

namespace Timberborn.ForestryEffects;

internal class TreeCutterSwimmingBlocker : BaseComponent, IAwakableComponent
{
	private SwimmingAnimator _swimmingAnimator;

	public void Awake()
	{
		_swimmingAnimator = GetComponent<SwimmingAnimator>();
		TreeCutter component = GetComponent<TreeCutter>();
		component.CuttingStarted += OnCuttingStarted;
		component.CuttingStopped += OnCuttingStopped;
	}

	private void OnCuttingStarted(object sender, EventArgs e)
	{
		_swimmingAnimator.BlockSwimmingMovementAndResetPosition();
	}

	private void OnCuttingStopped(object sender, EventArgs e)
	{
		_swimmingAnimator.UnblockSwimmingMovement();
	}
}
