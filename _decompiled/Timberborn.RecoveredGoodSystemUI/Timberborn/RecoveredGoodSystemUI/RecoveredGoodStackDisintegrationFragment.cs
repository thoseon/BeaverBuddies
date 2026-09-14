using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;
using Timberborn.RecoveredGoodSystem;
using Timberborn.UIFormatters;
using UnityEngine.UIElements;

namespace Timberborn.RecoveredGoodSystemUI;

internal class RecoveredGoodStackDisintegrationFragment : IEntityPanelFragment
{
	private static readonly string DisintegrationCountdownLocKey = "RecoveredGoodStack.DisintegrationCountdown";

	private readonly ILoc _loc;

	private readonly VisualElementLoader _visualElementLoader;

	private RecoveredGoodStackDisintegration _recoveredGoodStackDisintegration;

	private VisualElement _root;

	private Timberborn.CoreUI.ProgressBar _progressBar;

	private Label _text;

	public RecoveredGoodStackDisintegrationFragment(ILoc loc, VisualElementLoader visualElementLoader)
	{
		_loc = loc;
		_visualElementLoader = visualElementLoader;
	}

	public VisualElement InitializeFragment()
	{
		string elementName = "Game/EntityPanel/RecoveredGoodStackDisintegrationFragment";
		_root = _visualElementLoader.LoadVisualElement(elementName);
		_progressBar = _root.Q<Timberborn.CoreUI.ProgressBar>("ProgressBar");
		_text = _root.Q<Label>("Text");
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_recoveredGoodStackDisintegration = entity.GetComponent<RecoveredGoodStackDisintegration>();
		if ((bool)_recoveredGoodStackDisintegration)
		{
			_root.ToggleDisplayStyle(visible: true);
			UpdateFragment();
		}
	}

	public void ClearFragment()
	{
		_root.ToggleDisplayStyle(visible: false);
		_recoveredGoodStackDisintegration = null;
	}

	public void UpdateFragment()
	{
		if ((bool)_recoveredGoodStackDisintegration)
		{
			_progressBar.SetProgress(_recoveredGoodStackDisintegration.Progress);
			string param = NumberFormatter.CeilToTenthsPlace(_recoveredGoodStackDisintegration.DaysToDisintegration);
			_text.text = _loc.T(DisintegrationCountdownLocKey, param);
		}
	}
}
