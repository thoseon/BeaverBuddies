using Timberborn.DeconstructionSystem;
using Timberborn.SingletonSystem;
using Timberborn.UISound;

namespace Timberborn.DeconstructionSystemUI;

internal class DeconstructionSoundPlayer : ILoadableSingleton, IUpdatableSingleton
{
	private static readonly string DeconstructionSoundName = "UI.Buildings.Deconstruction";

	private readonly EventBus _eventBus;

	private readonly UISoundController _uiSoundController;

	private bool _shouldPlaySound;

	public DeconstructionSoundPlayer(EventBus eventBus, UISoundController uiSoundController)
	{
		_eventBus = eventBus;
		_uiSoundController = uiSoundController;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	public void UpdateSingleton()
	{
		if (_shouldPlaySound)
		{
			_shouldPlaySound = false;
			_uiSoundController.PlaySound(DeconstructionSoundName);
		}
	}

	[OnEvent]
	public void OnBuildingDeconstructed(BuildingDeconstructedEvent buildingDeconstructedEvent)
	{
		_shouldPlaySound = true;
	}
}
