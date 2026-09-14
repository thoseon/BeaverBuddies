using System.Collections.Generic;
using Timberborn.BlueprintSystem;
using Timberborn.Common;
using Timberborn.SingletonSystem;

namespace Timberborn.AutomationBuildings;

internal class SpeakerBuiltinSounds : ILoadableSingleton
{
	private readonly ISpecService _specService;

	private readonly List<string> _soundIds = new List<string>();

	private readonly Dictionary<string, string> _soundNames = new Dictionary<string, string>();

	public ReadOnlyList<string> SoundIds => _soundIds.AsReadOnlyList();

	public SpeakerBuiltinSounds(ISpecService specService)
	{
		_specService = specService;
	}

	public void Load()
	{
		foreach (SpeakerSoundSpec spec in _specService.GetSpecs<SpeakerSoundSpec>())
		{
			_soundIds.Add(spec.SoundId);
			_soundNames[spec.SoundId] = spec.DisplayName.Value;
		}
	}

	public string GetSoundDisplayName(string soundId)
	{
		return _soundNames[soundId];
	}
}
