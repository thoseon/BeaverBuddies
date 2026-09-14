using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.SelectionSystem;
using Timberborn.SoundSystem;

namespace Timberborn.CoreSound;

public class BasicSelectionSound : BaseComponent, IAwakableComponent, ISelectionListener
{
	private readonly ISoundSystem _soundSystem;

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private BasicSelectionSoundSpec _basicSelectionSoundSpec;

	public BasicSelectionSound(ISoundSystem soundSystem, IRandomNumberGenerator randomNumberGenerator)
	{
		_soundSystem = soundSystem;
		_randomNumberGenerator = randomNumberGenerator;
	}

	public void Awake()
	{
		_basicSelectionSoundSpec = GetComponent<BasicSelectionSoundSpec>();
	}

	public void OnSelect()
	{
		_soundSystem.PlaySound2D(base.GameObject, "UI.BasicSelection." + GetSoundName(), 10);
	}

	public void OnUnselect()
	{
	}

	private string GetSoundName()
	{
		if (!string.IsNullOrEmpty(_basicSelectionSoundSpec.AlternativeSoundName) && _randomNumberGenerator.Range(0f, 1f) < 0.1f)
		{
			return _basicSelectionSoundSpec.AlternativeSoundName + ".AltSound";
		}
		return _basicSelectionSoundSpec.SoundName;
	}
}
