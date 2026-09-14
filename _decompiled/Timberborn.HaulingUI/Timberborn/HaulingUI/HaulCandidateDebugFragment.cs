using System.Collections.Generic;
using System.Linq;
using System.Text;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Hauling;
using UnityEngine.UIElements;

namespace Timberborn.HaulingUI;

internal class HaulCandidateDebugFragment : IEntityPanelFragment
{
	private readonly DebugFragmentFactory _debugFragmentFactory;

	private readonly StringBuilder _description = new StringBuilder();

	private HaulCandidate _haulCandidate;

	private Label _text;

	private VisualElement _root;

	private readonly List<WeightedBehavior> _weightedBehaviors = new List<WeightedBehavior>();

	public HaulCandidateDebugFragment(DebugFragmentFactory debugFragmentFactory)
	{
		_debugFragmentFactory = debugFragmentFactory;
	}

	public VisualElement InitializeFragment()
	{
		_root = _debugFragmentFactory.Create("HaulCandidate");
		_text = _root.Q<Label>("Text");
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_haulCandidate = entity.GetComponent<HaulCandidate>();
	}

	public void ClearFragment()
	{
		_haulCandidate = null;
		UpdateContent();
	}

	public void UpdateFragment()
	{
		UpdateContent();
	}

	private void UpdateContent()
	{
		_description.Clear();
		if ((bool)_haulCandidate && _haulCandidate.Enabled)
		{
			UpdateDescription();
		}
		_root.ToggleDisplayStyle(_description.Length > 0);
	}

	private void UpdateDescription()
	{
		_haulCandidate.GetWeightedBehaviors(_weightedBehaviors);
		foreach (WeightedBehavior weightedBehavior in _weightedBehaviors)
		{
			_description.AppendLine($"{weightedBehavior.Weight:F2} {GetBehaviorName(weightedBehavior)}");
		}
		_weightedBehaviors.Clear();
		_text.text = _description.ToStringWithoutNewLineEnd();
	}

	private static string GetBehaviorName(WeightedBehavior weightedBehavior)
	{
		string text = ((object)weightedBehavior.WorkplaceBehavior).ToString().Split('.').Last();
		return text.Remove(text.Length - 1);
	}
}
