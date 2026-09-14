using System;
using Timberborn.BaseComponentSystem;
using Timberborn.CharacterModelSystem;
using Timberborn.Planting;

namespace Timberborn.PlantingEffects;

internal class PlantingAnimationController : BaseComponent, IAwakableComponent
{
	private static readonly string PlantingAnimation = "Planting";

	private CharacterAnimator _characterAnimator;

	public void Awake()
	{
		_characterAnimator = GetComponent<CharacterAnimator>();
		PlantExecutor component = GetComponent<PlantExecutor>();
		component.PlantingStarted += OnPlantingStarted;
		component.PlantingFinished += OnPlantingFinished;
	}

	private void OnPlantingStarted(object sender, EventArgs e)
	{
		_characterAnimator.SetBool(PlantingAnimation, value: true);
	}

	private void OnPlantingFinished(object sender, EventArgs e)
	{
		_characterAnimator.SetBool(PlantingAnimation, value: false);
	}
}
