using JetBrains.Annotations;
using Steamworks;
using Timberborn.CoreUI;
using Timberborn.InputSystem;
using Timberborn.SingletonSystem;
using Timberborn.SteamStoreSystem;
using UnityEngine.UIElements;

namespace Timberborn.SteamOverlaySystem;

internal class SteamOverlayInputBlocker : IPostLoadableSingleton, IUnloadableSingleton, IPanelController
{
	private readonly SteamManager _steamManager;

	private readonly IInputStateResetter _inputStateResetter;

	private readonly PanelStack _panelStack;

	[UsedImplicitly]
	private Callback<GameOverlayActivated_t> _steamOverlayCallback;

	public SteamOverlayInputBlocker(SteamManager steamManager, IInputStateResetter inputStateResetter, PanelStack panelStack)
	{
		_steamManager = steamManager;
		_inputStateResetter = inputStateResetter;
		_panelStack = panelStack;
	}

	public void PostLoad()
	{
		if (_steamManager.Initialized)
		{
			_steamOverlayCallback = Callback<GameOverlayActivated_t>.Create(SteamOverlayActivated);
		}
	}

	public void Unload()
	{
		_steamOverlayCallback?.Dispose();
		_steamOverlayCallback = null;
	}

	public VisualElement GetPanel()
	{
		return new VisualElement();
	}

	public bool OnUIConfirmed()
	{
		return false;
	}

	public void OnUICancelled()
	{
	}

	private void SteamOverlayActivated(GameOverlayActivated_t callback)
	{
		if (callback.m_nAppID == SteamAppId.AppId)
		{
			bool flag = _panelStack.IsPanelOnTop(this);
			if (callback.m_bActive == 1 && !flag)
			{
				_panelStack.PushOverlay(this);
			}
			else if (flag)
			{
				_panelStack.Pop(this);
				_inputStateResetter.ResetInputState();
			}
		}
	}
}
