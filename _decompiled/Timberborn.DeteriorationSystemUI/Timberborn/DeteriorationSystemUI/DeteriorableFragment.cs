using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.DeteriorationSystem;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;
using Timberborn.UIFormatters;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.DeteriorationSystemUI;

internal class DeteriorableFragment : IEntityPanelFragment
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly ILoc _loc;

	private Deteriorable _deteriorable;

	private VisualElement _root;

	private Timberborn.CoreUI.ProgressBar _progressBar;

	private Label _durabilityLabel;

	private readonly Phrase _progressPhrase = Phrase.New("Bot.Durability").FormatPercentFloored();

	public DeteriorableFragment(VisualElementLoader visualElementLoader, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_loc = loc;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/DeteriorableFragment");
		_progressBar = _root.Q<Timberborn.CoreUI.ProgressBar>("ProgressBar");
		_durabilityLabel = _root.Q<Label>("Durability");
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_deteriorable = entity.GetComponent<Deteriorable>();
		if ((bool)(BaseComponent)(object)_deteriorable)
		{
			_root.ToggleDisplayStyle(visible: true);
		}
	}

	public void ClearFragment()
	{
		_root.ToggleDisplayStyle(visible: false);
		_deteriorable = null;
	}

	public void UpdateFragment()
	{
		if ((bool)(BaseComponent)(object)_deteriorable)
		{
			float num = Mathf.Clamp01(_deteriorable.DeteriorationProgress);
			_progressBar.SetProgress(num);
			_durabilityLabel.text = _loc.T(_progressPhrase, num);
		}
	}
}
