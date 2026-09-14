using System;
using Timberborn.BaseComponentSystem;
using Timberborn.WalkingSystem;

namespace Timberborn.ZiplineMovementSystem;

internal class ZiplineSwimmingBlocker : BaseComponent, IAwakableComponent
{
	private SwimmingAnimator _swimmingAnimator;

	public void Awake()
	{
		_swimmingAnimator = GetComponent<SwimmingAnimator>();
		ZiplineVisitor component = GetComponent<ZiplineVisitor>();
		component.EnteredZipline += OnEnteredZipline;
		component.ExitedZipline += OnExitedZipline;
	}

	private void OnEnteredZipline(object sender, EventArgs e)
	{
		_swimmingAnimator.BlockSwimmingMovement();
	}

	private void OnExitedZipline(object sender, EventArgs e)
	{
		_swimmingAnimator.UnblockSwimmingMovement();
	}
}
