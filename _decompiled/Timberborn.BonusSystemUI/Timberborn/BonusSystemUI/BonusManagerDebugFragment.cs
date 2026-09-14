using System.Text;
using Timberborn.BaseComponentSystem;
using Timberborn.BonusSystem;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using UnityEngine.UIElements;

namespace Timberborn.BonusSystemUI;

public class BonusManagerDebugFragment : IEntityPanelFragment
{
	private readonly DebugFragmentFactory _debugFragmentFactory;

	private readonly BonusTypeSpecService _bonusTypeSpecService;

	private BonusManager _bonusManager;

	private Label _text;

	private VisualElement _root;

	public BonusManagerDebugFragment(DebugFragmentFactory debugFragmentFactory, BonusTypeSpecService bonusTypeSpecService)
	{
		_debugFragmentFactory = debugFragmentFactory;
		_bonusTypeSpecService = bonusTypeSpecService;
	}

	public VisualElement InitializeFragment()
	{
		_root = _debugFragmentFactory.Create("BonusManager");
		_text = _root.Q<Label>("Text");
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_bonusManager = entity.GetComponent<BonusManager>();
	}

	public void ClearFragment()
	{
		_bonusManager = null;
	}

	public void UpdateFragment()
	{
		if ((bool)_bonusManager)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string bonusId in _bonusTypeSpecService.BonusIds)
			{
				float num = _bonusManager.Multiplier(bonusId);
				stringBuilder.AppendLine($"{bonusId}: {num}");
			}
			_text.text = stringBuilder.ToStringWithoutNewLineEnd();
			_root.ToggleDisplayStyle(visible: true);
		}
		else
		{
			_root.ToggleDisplayStyle(visible: false);
		}
	}
}
