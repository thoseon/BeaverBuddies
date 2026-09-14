using System.Collections.Generic;
using Timberborn.FactionSystem;
using Timberborn.SingletonSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.WellbeingUI;

public class PopulationWellbeingGoals : IUpdatableSingleton
{
	private static readonly float BlinkInterval = 0.5f;

	private static readonly int BlinkCount = 5;

	private static readonly string BlinkClass = "blink";

	private readonly GoalRowFactory _goalRowFactory;

	private readonly FactionSpecService _factionSpecService;

	private VisualElement _goalsWrapper;

	private readonly Dictionary<string, VisualElement> _goals = new Dictionary<string, VisualElement>();

	private VisualElement _blinkingElement;

	private int _remainingBlinks;

	private float _timeToBlink;

	public PopulationWellbeingGoals(GoalRowFactory goalRowFactory, FactionSpecService factionSpecService)
	{
		_goalRowFactory = goalRowFactory;
		_factionSpecService = factionSpecService;
	}

	public void Initialize(VisualElement root)
	{
		_goalsWrapper = root.Q<VisualElement>("GoalsWrapper");
	}

	public void StartBlinking(FactionSpec factionSpec)
	{
		_blinkingElement = _goals[factionSpec.Id];
		_remainingBlinks = BlinkCount * 2;
		_timeToBlink = BlinkInterval;
	}

	public void UpdateSingleton()
	{
		if (_blinkingElement != null)
		{
			_timeToBlink -= Time.unscaledDeltaTime;
			if (_timeToBlink <= 0f)
			{
				Blink();
			}
		}
	}

	public void AddGoals()
	{
		foreach (UnlockableFactionSpec unlockableFaction in _factionSpecService.UnlockableFactions)
		{
			VisualElement visualElement = _goalRowFactory.CreateRow(unlockableFaction);
			_goalsWrapper.Add(visualElement);
			_goals[unlockableFaction.GetSpec<FactionSpec>().Id] = visualElement;
		}
	}

	public void Clear()
	{
		_goalsWrapper.Clear();
		_goals.Clear();
		_blinkingElement = null;
	}

	private void Blink()
	{
		_timeToBlink = BlinkInterval;
		_remainingBlinks--;
		if (_remainingBlinks > 0)
		{
			_blinkingElement.ToggleInClassList(BlinkClass);
			return;
		}
		_blinkingElement.RemoveFromClassList(BlinkClass);
		_blinkingElement = null;
	}
}
