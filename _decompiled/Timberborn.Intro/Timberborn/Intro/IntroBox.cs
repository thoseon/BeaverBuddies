using System;
using System.IO;
using Bindito.Unity;
using Timberborn.AssetSystem;
using Timberborn.CoreUI;
using Timberborn.InputSystem;
using Timberborn.IntroSettingsSystem;
using Timberborn.RootProviders;
using Timberborn.SingletonSystem;
using Timberborn.TitleScreenUI;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;

namespace Timberborn.Intro;

public class IntroBox : IPanelController, IUnloadableSingleton
{
	private static readonly string IntroPath = Path.Combine(Application.streamingAssetsPath, "Intro", "Timberborn_Intro.mp4");

	private readonly PanelStack _panelStack;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly RootObjectProvider _rootObjectProvider;

	private readonly IAssetLoader _assetLoader;

	private readonly IInstantiator _instantiator;

	private readonly TitleScreen _titleScreen;

	private readonly IntroSettings _introSettings;

	private readonly MouseController _mouseController;

	private VisualElement _root;

	private GameObject _rootObject;

	private Action _onStart;

	public IntroBox(PanelStack panelStack, VisualElementLoader visualElementLoader, RootObjectProvider rootObjectProvider, IAssetLoader assetLoader, IInstantiator instantiator, TitleScreen titleScreen, IntroSettings introSettings, MouseController mouseController)
	{
		_panelStack = panelStack;
		_visualElementLoader = visualElementLoader;
		_rootObjectProvider = rootObjectProvider;
		_assetLoader = assetLoader;
		_instantiator = instantiator;
		_titleScreen = titleScreen;
		_introSettings = introSettings;
		_mouseController = mouseController;
	}

	public VisualElement GetPanel()
	{
		return _root;
	}

	public bool OnUIConfirmed()
	{
		Start();
		return false;
	}

	public void OnUICancelled()
	{
		Start();
	}

	public void Show(Action onStart)
	{
		if (_introSettings.DisableIntro || !File.Exists(IntroPath))
		{
			onStart?.Invoke();
			return;
		}
		_root = _visualElementLoader.LoadVisualElement("MainMenu/IntroBox");
		_rootObject = _rootObjectProvider.CreateRootObject("IntroBox");
		_titleScreen.HideBackground();
		_mouseController.HideCursor();
		InitializeVideoPlayer();
		_onStart = onStart;
		_panelStack.Push(this);
	}

	public void Unload()
	{
		_assetLoader.Load<RenderTexture>("Intro/Intro").Release();
	}

	private void Start()
	{
		_root = null;
		UnityEngine.Object.Destroy(_rootObject);
		if (_panelStack.IsPanelOnTop(this))
		{
			_panelStack.Pop(this);
		}
		_titleScreen.ShowBackground();
		_mouseController.ShowCursor();
		_onStart?.Invoke();
	}

	private void InitializeVideoPlayer()
	{
		GameObject prefab = _assetLoader.Load<GameObject>("Intro/Intro");
		VideoPlayer component = _instantiator.Instantiate(prefab, _rootObject.transform).GetComponent<VideoPlayer>();
		component.source = VideoSource.Url;
		component.url = IntroPath;
		component.Prepare();
		component.prepareCompleted += delegate(VideoPlayer player)
		{
			player.Play();
		};
		component.loopPointReached += delegate
		{
			Start();
		};
		ClearRenderTexture(component);
	}

	private static void ClearRenderTexture(VideoPlayer videoPlayer)
	{
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = videoPlayer.targetTexture;
		GL.Clear(clearDepth: true, clearColor: true, Color.black);
		RenderTexture.active = active;
	}
}
