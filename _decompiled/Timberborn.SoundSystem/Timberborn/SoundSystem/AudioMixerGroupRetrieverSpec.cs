using Timberborn.BlueprintSystem;
using UnityEngine.Audio;

namespace Timberborn.SoundSystem;

internal record AudioMixerGroupRetrieverSpec : ComponentSpec
{
	[Serialize]
	public AssetRef<AudioMixer> AudioMixer { get; init; }
}
