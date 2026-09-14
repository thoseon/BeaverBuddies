using System.Linq;
using System.Text;
using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using UnityEngine.UIElements;

namespace Timberborn.BehaviorSystemUI;

internal class BehaviorManagerDebugFragment : IEntityPanelFragment
{
	private readonly DebugFragmentFactory _debugFragmentFactory;

	private BehaviorManager _behaviorManager;

	private Label _text;

	private VisualElement _root;

	public BehaviorManagerDebugFragment(DebugFragmentFactory debugFragmentFactory)
	{
		_debugFragmentFactory = debugFragmentFactory;
	}

	public VisualElement InitializeFragment()
	{
		_root = _debugFragmentFactory.Create("BehaviorManager");
		_text = _root.Q<Label>("Text");
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_behaviorManager = entity.GetComponent<BehaviorManager>();
	}

	public void ClearFragment()
	{
		_behaviorManager = null;
		UpdateDescriptions();
	}

	public void UpdateFragment()
	{
		UpdateDescriptions();
	}

	private void UpdateDescriptions()
	{
		if ((bool)(BaseComponent)(object)_behaviorManager && ((BaseComponent)(object)_behaviorManager).Enabled)
		{
			StringBuilder stringBuilder = new StringBuilder();
			BehaviorInfo runningBehavior = _behaviorManager.RunningBehavior;
			ExecutorInfo runningExecutor = _behaviorManager.RunningExecutor;
			stringBuilder.AppendLine("Active behavior: " + runningBehavior.Name);
			stringBuilder.AppendLine($"Active executor: {runningExecutor.Name} {runningExecutor.ElapsedTime:0.0}s");
			stringBuilder.AppendLine("Behavior log:");
			foreach (string item in _behaviorManager.TimestampedBehaviorLog.Reverse())
			{
				stringBuilder.AppendLine(item ?? "");
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
