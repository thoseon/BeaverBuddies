using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.CoreSound;
using Timberborn.SelectionSystem;
using Timberborn.SoundSystem;

namespace Timberborn.Buildings;

public class BuildingSelectionSound : BaseComponent, IAwakableComponent, ISelectionListener
{
	private readonly ISoundSystem _soundSystem;

	private BuildingSpec _buildingSpec;

	private BlockObject _blockObject;

	public BuildingSelectionSound(ISoundSystem soundSystem)
	{
		_soundSystem = soundSystem;
	}

	public void Awake()
	{
		_buildingSpec = GetComponent<BuildingSpec>();
		_blockObject = GetComponent<BlockObject>();
	}

	public void OnSelect()
	{
		string text = (_blockObject.IsFinished ? _buildingSpec.SelectionSoundName : "Default");
		_soundSystem.SetCustomMixer(base.GameObject, text, MixerNames.UIMixerNameKey);
		_soundSystem.PlaySound2D(base.GameObject, "UI.Buildings.Selected." + text, 10);
	}

	public void OnUnselect()
	{
	}
}
