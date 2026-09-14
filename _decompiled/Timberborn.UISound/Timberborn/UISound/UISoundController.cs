using Timberborn.RootProviders;
using Timberborn.SingletonSystem;
using Timberborn.SoundSystem;
using UnityEngine;

namespace Timberborn.UISound;

public class UISoundController : ILoadableSingleton
{
	private static readonly string Click = "UI.Click";

	private static readonly string Cancel = "UI.Cancel";

	private static readonly string CantDo = "UI.CantDo";

	private readonly ISoundSystem _soundSystem;

	private readonly RootObjectProvider _rootObjectProvider;

	private GameObject _parent;

	public UISoundController(ISoundSystem soundSystem, RootObjectProvider rootObjectProvider)
	{
		_soundSystem = soundSystem;
		_rootObjectProvider = rootObjectProvider;
	}

	public void Load()
	{
		_parent = _rootObjectProvider.CreateRootObject("UISoundController");
	}

	public void PlaySound(string sound)
	{
		_soundSystem.PlaySound2D(_parent, sound, 10);
	}

	public void PlayClickSound()
	{
		PlaySound(Click);
	}

	public void PlayCancelSound()
	{
		PlaySound(Cancel);
	}

	public void PlayCantDoSound()
	{
		PlaySound(CantDo);
	}
}
