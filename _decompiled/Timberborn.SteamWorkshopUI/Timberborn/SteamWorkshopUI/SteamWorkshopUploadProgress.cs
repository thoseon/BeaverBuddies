using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.SingletonSystem;
using Timberborn.SteamWorkshop;
using Timberborn.UIFormatters;
using UnityEngine.UIElements;

namespace Timberborn.SteamWorkshopUI;

public class SteamWorkshopUploadProgress : ILoadableSingleton, IUpdatableSingleton, IPanelController
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly PanelStack _panelStack;

	private readonly ILoc _loc;

	private VisualElement _root;

	private Timberborn.CoreUI.ProgressBar _progressBar;

	private Label _progressLabel;

	private float _progress;

	private SteamWorkshopUpdateHandle _steamWorkshopUpdateHandle;

	private readonly Phrase _uploadProgressPhrase = Phrase.New("SteamWorkshop.UploadProgress").FormatPercentFloored();

	public SteamWorkshopUploadProgress(VisualElementLoader visualElementLoader, PanelStack panelStack, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_panelStack = panelStack;
		_loc = loc;
	}

	public void Load()
	{
		string elementName = "Common/SteamWorkshop/SteamWorkshopUploadProgress";
		_root = _visualElementLoader.LoadVisualElement(elementName);
		_progressBar = _root.Q<Timberborn.CoreUI.ProgressBar>("ProgressBar");
		_progressLabel = _root.Q<Label>("ProgressLabel");
	}

	public void Open()
	{
		_panelStack.PushOverlay(this);
		_progress = 0f;
		UpdateProgressBar();
	}

	public void Close()
	{
		_panelStack.Pop(this);
		_steamWorkshopUpdateHandle = null;
	}

	public void SetUpdateHandle(SteamWorkshopUpdateHandle steamWorkshopUpdateHandle)
	{
		_steamWorkshopUpdateHandle = steamWorkshopUpdateHandle;
	}

	public void UpdateSingleton()
	{
		if (_steamWorkshopUpdateHandle != null)
		{
			float progress = _steamWorkshopUpdateHandle.GetProgress();
			if (progress > _progress)
			{
				_progress = progress;
				UpdateProgressBar();
			}
		}
	}

	public VisualElement GetPanel()
	{
		return _root;
	}

	public bool OnUIConfirmed()
	{
		return false;
	}

	public void OnUICancelled()
	{
	}

	private void UpdateProgressBar()
	{
		_progressBar.SetProgress(_progress);
		_progressLabel.text = _loc.T(_uploadProgressPhrase, _progress);
	}
}
