using System;
using System.Collections.Generic;
using Timberborn.CoreUI;
using Timberborn.NewGameConfigurationSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.MainMenuPanels;

public class CustomNewGameModeController
{
	private static readonly string InvalidInputClass = "new-game-mode-panel__setting-input--invalid";

	private readonly List<IntegerField> _integerFields = new List<IntegerField>();

	private Action _gameModeChangedCallback;

	private IntegerField _startingAdults;

	private IntegerField _minAdultAgeProgress;

	private IntegerField _maxAdultAgeProgress;

	private IntegerField _startingChildren;

	private IntegerField _minChildAgeProgress;

	private IntegerField _maxChildAgeProgress;

	private IntegerField _foodConsumption;

	private IntegerField _waterConsumption;

	private IntegerField _startingFood;

	private IntegerField _startingWater;

	private IntegerField _minTemperateWeatherDuration;

	private IntegerField _maxTemperateWeatherDuration;

	private Toggle _droughtEnabled;

	private VisualElement _droughtWrapper;

	private IntegerField _minDroughtDuration;

	private IntegerField _maxDroughtDuration;

	private IntegerField _droughtDurationHandicapMultiplier;

	private IntegerField _droughtDurationHandicapCycles;

	private Toggle _badtideEnabled;

	private VisualElement _badtideWrapper;

	private IntegerField _cyclesBeforeRandomizingBadtide;

	private IntegerField _chanceForBadtide;

	private IntegerField _minBadtideDuration;

	private IntegerField _maxBadtideDuration;

	private IntegerField _badtideDurationHandicapMultiplier;

	private IntegerField _badtideDurationHandicapCycles;

	private IntegerField _injuryChance;

	private IntegerField _demolishableRecoveryRate;

	private bool IsDroughtEnabled => _droughtEnabled.value;

	private bool IsBadtideEnabled => _badtideEnabled.value;

	public void Initialize(VisualElement root, GameModeSpec defaultGameMode, Action gameModeChangedCallback)
	{
		_gameModeChangedCallback = gameModeChangedCallback;
		_integerFields.Clear();
		_startingAdults = QInitializedIntField(root, "StartingAdults", defaultGameMode.StartingAdults, 1, 250);
		(IntegerField, IntegerField) tuple = QInitializedMinMaxPercentFields(root, "MinAdultAgeProgress", "MaxAdultAgeProgress", defaultGameMode.AdultAgeProgress);
		_minAdultAgeProgress = tuple.Item1;
		_maxAdultAgeProgress = tuple.Item2;
		_startingChildren = QInitializedIntField(root, "StartingChildren", defaultGameMode.StartingChildren, 0, 250);
		tuple = QInitializedMinMaxPercentFields(root, "MinChildAgeProgress", "MaxChildAgeProgress", defaultGameMode.ChildAgeProgress);
		_minChildAgeProgress = tuple.Item1;
		_maxChildAgeProgress = tuple.Item2;
		_foodConsumption = QInitializedPercentField(root, "FoodConsumption", defaultGameMode.FoodConsumption, 10, 1000);
		_waterConsumption = QInitializedPercentField(root, "WaterConsumption", defaultGameMode.WaterConsumption, 10, 1000);
		_startingFood = QInitializedIntField(root, "StartingFood", defaultGameMode.StartingFood, 0, 999999);
		_startingWater = QInitializedIntField(root, "StartingWater", defaultGameMode.StartingWater, 0, 999999);
		tuple = QInitializedMinMaxIntFields(root, "MinTemperateWeatherDuration", "MaxTemperateWeatherDuration", defaultGameMode.TemperateWeatherDuration, 3);
		_minTemperateWeatherDuration = tuple.Item1;
		_maxTemperateWeatherDuration = tuple.Item2;
		_droughtEnabled = QInitializedToggle(root, "DroughtEnabled", startingValue: true);
		_droughtWrapper = root.Q<VisualElement>("DroughtWrapper");
		tuple = QInitializedMinMaxIntFields(root, "MinDroughtDuration", "MaxDroughtDuration", defaultGameMode.DroughtDuration);
		_minDroughtDuration = tuple.Item1;
		_maxDroughtDuration = tuple.Item2;
		_droughtDurationHandicapMultiplier = QInitializedPercentField(root, "DroughtDurationHandicapMultiplier", defaultGameMode.DroughtDurationHandicapMultiplier);
		_droughtDurationHandicapCycles = QInitializedIntField(root, "DroughtDurationHandicapCycles", defaultGameMode.DroughtDurationHandicapCycles);
		_badtideEnabled = QInitializedToggle(root, "BadtideEnabled", startingValue: true);
		_badtideWrapper = root.Q<VisualElement>("BadtideWrapper");
		_cyclesBeforeRandomizingBadtide = QInitializedIntField(root, "CyclesBeforeRandomizingBadtide", defaultGameMode.CyclesBeforeRandomizingBadtide);
		_chanceForBadtide = QInitializedPercentField(root, "ChanceForBadtide", defaultGameMode.ChanceForBadtide);
		tuple = QInitializedMinMaxIntFields(root, "MinBadtideDuration", "MaxBadtideDuration", defaultGameMode.BadtideDuration);
		_minBadtideDuration = tuple.Item1;
		_maxBadtideDuration = tuple.Item2;
		_badtideDurationHandicapMultiplier = QInitializedPercentField(root, "BadtideDurationHandicapMultiplier", defaultGameMode.BadtideDurationHandicapMultiplier);
		_badtideDurationHandicapCycles = QInitializedIntField(root, "BadtideDurationHandicapCycles", defaultGameMode.BadtideDurationHandicapCycles);
		_injuryChance = QInitializedPercentField(root, "InjuryChance", defaultGameMode.InjuryChance, 0, 1000);
		_demolishableRecoveryRate = QInitializedPercentField(root, "DemolishableRecoveryRate", defaultGameMode.DemolishableRecoveryRate);
		root.Q<ScrollView>("CustomModeSettings").scrollOffset = Vector2.zero;
		UpdateFieldVisibility();
	}

	public bool TryGetValidatedGameMode(out GameModeSpec gameMode)
	{
		GameModeSpec gameMode2 = GetGameMode();
		if (ValidateGameMode(gameMode2))
		{
			gameMode = gameMode2;
			return true;
		}
		gameMode = null;
		return false;
	}

	private (IntegerField min, IntegerField max) QInitializedMinMaxPercentFields(VisualElement root, string minName, string maxName, MinMaxSpec<float> startingValue)
	{
		return (min: QInitializedPercentField(root, minName, startingValue.Min), max: QInitializedPercentField(root, maxName, startingValue.Max));
	}

	private (IntegerField min, IntegerField max) QInitializedMinMaxIntFields(VisualElement root, string minName, string maxName, MinMaxSpec<int> startingValue, int minValue = 0)
	{
		return (min: QInitializedIntField(root, minName, startingValue.Min, minValue), max: QInitializedIntField(root, maxName, startingValue.Max, minValue));
	}

	private IntegerField QInitializedPercentField(VisualElement root, string name, float startingValue, int minValue = 0, int maxValue = 100)
	{
		return QInitializedIntField(root, name, FloatToPercent(startingValue), minValue, maxValue);
	}

	private IntegerField QInitializedIntField(VisualElement root, string name, int startingValue, int minValue = 0, int maxValue = int.MaxValue)
	{
		IntegerField integerField = root.Q<IntegerField>(name);
		TextFields.InitializeIntegerField(integerField, startingValue, minValue, maxValue, delegate
		{
			OnGameModeChanged();
		});
		_integerFields.Add(integerField);
		return integerField;
	}

	private Toggle QInitializedToggle(VisualElement root, string name, bool startingValue)
	{
		Toggle toggle = root.Q<Toggle>(name);
		toggle.SetValueWithoutNotify(startingValue);
		toggle.RegisterCallback<ClickEvent>(delegate
		{
			OnGameModeChanged();
		});
		return toggle;
	}

	private void OnGameModeChanged()
	{
		ValidateGameMode(GetGameMode());
		UpdateFieldVisibility();
		_gameModeChangedCallback();
	}

	private GameModeSpec GetGameMode()
	{
		int startingAdults = GetInt(_startingAdults);
		MinMaxSpec<float> floatsFromPercents = GetFloatsFromPercents(_minAdultAgeProgress, _maxAdultAgeProgress);
		int startingChildren = GetInt(_startingChildren);
		MinMaxSpec<float> floatsFromPercents2 = GetFloatsFromPercents(_minChildAgeProgress, _maxChildAgeProgress);
		float floatFromPercent = GetFloatFromPercent(_foodConsumption);
		float floatFromPercent2 = GetFloatFromPercent(_waterConsumption);
		int startingFood = GetInt(_startingFood);
		int startingWater = GetInt(_startingWater);
		MinMaxSpec<int> ints = GetInts(_minTemperateWeatherDuration, _maxTemperateWeatherDuration);
		MinMaxSpec<int> droughtDuration = (IsDroughtEnabled ? GetInts(_minDroughtDuration, _maxDroughtDuration) : new MinMaxSpec<int>());
		float droughtDurationHandicapMultiplier = (IsDroughtEnabled ? GetFloatFromPercent(_droughtDurationHandicapMultiplier) : 0f);
		int droughtDurationHandicapCycles = (IsDroughtEnabled ? GetInt(_droughtDurationHandicapCycles) : 0);
		int cyclesBeforeRandomizingBadtide = (IsBadtideEnabled ? GetInt(_cyclesBeforeRandomizingBadtide) : 0);
		float chanceForBadtide = GetFloatFromPercent(_chanceForBadtide);
		if (!IsBadtideEnabled)
		{
			chanceForBadtide = 0f;
		}
		else if (!IsDroughtEnabled)
		{
			chanceForBadtide = 1f;
		}
		MinMaxSpec<int> badtideDuration = (IsBadtideEnabled ? GetInts(_minBadtideDuration, _maxBadtideDuration) : new MinMaxSpec<int>());
		float badtideDurationHandicapMultiplier = (IsBadtideEnabled ? GetFloatFromPercent(_badtideDurationHandicapMultiplier) : 0f);
		int badtideDurationHandicapCycles = (IsBadtideEnabled ? GetInt(_badtideDurationHandicapCycles) : 0);
		float floatFromPercent3 = GetFloatFromPercent(_injuryChance);
		float floatFromPercent4 = GetFloatFromPercent(_demolishableRecoveryRate);
		return new GameModeSpec
		{
			StartingAdults = startingAdults,
			AdultAgeProgress = floatsFromPercents,
			StartingChildren = startingChildren,
			ChildAgeProgress = floatsFromPercents2,
			FoodConsumption = floatFromPercent,
			WaterConsumption = floatFromPercent2,
			StartingFood = startingFood,
			StartingWater = startingWater,
			TemperateWeatherDuration = ints,
			DroughtDuration = droughtDuration,
			DroughtDurationHandicapMultiplier = droughtDurationHandicapMultiplier,
			DroughtDurationHandicapCycles = droughtDurationHandicapCycles,
			CyclesBeforeRandomizingBadtide = cyclesBeforeRandomizingBadtide,
			ChanceForBadtide = chanceForBadtide,
			BadtideDuration = badtideDuration,
			BadtideDurationHandicapMultiplier = badtideDurationHandicapMultiplier,
			BadtideDurationHandicapCycles = badtideDurationHandicapCycles,
			InjuryChance = floatFromPercent3,
			DemolishableRecoveryRate = floatFromPercent4
		};
	}

	private bool ValidateGameMode(GameModeSpec gameMode)
	{
		foreach (IntegerField integerField in _integerFields)
		{
			integerField.RemoveFromClassList(InvalidInputClass);
		}
		bool flag = false;
		MinMaxSpec<float> adultAgeProgress = gameMode.AdultAgeProgress;
		if (adultAgeProgress.Min > adultAgeProgress.Max)
		{
			_minAdultAgeProgress.AddToClassList(InvalidInputClass);
			_maxAdultAgeProgress.AddToClassList(InvalidInputClass);
			flag = true;
		}
		MinMaxSpec<float> childAgeProgress = gameMode.ChildAgeProgress;
		if (childAgeProgress.Min > childAgeProgress.Max)
		{
			_minChildAgeProgress.AddToClassList(InvalidInputClass);
			_maxChildAgeProgress.AddToClassList(InvalidInputClass);
			flag = true;
		}
		MinMaxSpec<int> temperateWeatherDuration = gameMode.TemperateWeatherDuration;
		if (temperateWeatherDuration.Min > temperateWeatherDuration.Max)
		{
			_minTemperateWeatherDuration.AddToClassList(InvalidInputClass);
			_maxTemperateWeatherDuration.AddToClassList(InvalidInputClass);
			flag = true;
		}
		if (IsDroughtEnabled)
		{
			MinMaxSpec<int> droughtDuration = gameMode.DroughtDuration;
			if (droughtDuration.Min > droughtDuration.Max)
			{
				_minDroughtDuration.AddToClassList(InvalidInputClass);
				_maxDroughtDuration.AddToClassList(InvalidInputClass);
				flag = true;
			}
		}
		if (IsBadtideEnabled)
		{
			MinMaxSpec<int> badtideDuration = gameMode.BadtideDuration;
			if (badtideDuration.Min > badtideDuration.Max)
			{
				_minBadtideDuration.AddToClassList(InvalidInputClass);
				_maxBadtideDuration.AddToClassList(InvalidInputClass);
				flag = true;
			}
		}
		return !flag;
	}

	private void UpdateFieldVisibility()
	{
		_droughtWrapper.ToggleDisplayStyle(IsDroughtEnabled);
		_badtideWrapper.ToggleDisplayStyle(IsBadtideEnabled);
	}

	private static MinMaxSpec<int> GetInts(IntegerField minIntegerField, IntegerField maxIntegerField)
	{
		return new MinMaxSpec<int>
		{
			Min = GetInt(minIntegerField),
			Max = GetInt(maxIntegerField)
		};
	}

	private static int GetInt(IntegerField integerField)
	{
		return integerField.value;
	}

	private static MinMaxSpec<float> GetFloatsFromPercents(IntegerField minIntegerField, IntegerField maxIntegerField)
	{
		return new MinMaxSpec<float>
		{
			Min = GetFloatFromPercent(minIntegerField),
			Max = GetFloatFromPercent(maxIntegerField)
		};
	}

	private static float GetFloatFromPercent(IntegerField integerField)
	{
		return (float)GetInt(integerField) / 100f;
	}

	private static int FloatToPercent(float value)
	{
		return Mathf.RoundToInt(value * 100f);
	}
}
