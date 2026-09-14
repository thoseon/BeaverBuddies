using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Ruins;
using Timberborn.UndoSystem;
using UnityEngine.UIElements;

namespace Timberborn.RuinsModelShuffling;

internal class RuinModelShufflingFragment : IEntityPanelFragment
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly RuinReplacer _ruinReplacer;

	private readonly IUndoRegistry _undoRegistry;

	private Ruin _ruin;

	private Button _button;

	private VisualElement _root;

	public RuinModelShufflingFragment(VisualElementLoader visualElementLoader, RuinReplacer ruinReplacer, IUndoRegistry undoRegistry)
	{
		_visualElementLoader = visualElementLoader;
		_ruinReplacer = ruinReplacer;
		_undoRegistry = undoRegistry;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("MapEditor/EntityPanel/RuinModelShufflingFragment");
		_button = _root.Q<Button>("Button");
		_button.RegisterCallback<ClickEvent>(ShuffleModel);
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_ruin = entity.GetComponent<Ruin>();
		if ((bool)_ruin)
		{
			_root.ToggleDisplayStyle(visible: true);
		}
	}

	public void ClearFragment()
	{
		_ruin = null;
		_root.ToggleDisplayStyle(visible: false);
	}

	public void UpdateFragment()
	{
	}

	private void ShuffleModel(ClickEvent evt)
	{
		_ruinReplacer.Shuffle(_ruin);
		_undoRegistry.CommitStack();
	}
}
