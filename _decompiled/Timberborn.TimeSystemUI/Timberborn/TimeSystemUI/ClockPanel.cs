using Timberborn.CoreUI;
using Timberborn.SingletonSystem;
using Timberborn.TimeSystem;
using Timberborn.UILayoutSystem;
using Timberborn.WorkSystem;
using UnityEngine.UIElements;

namespace Timberborn.TimeSystemUI;

public class ClockPanel : ILoadableSingleton, IUpdatableSingleton
{
	private static readonly float StartingAngleOffset = -60f;

	private readonly IDayNightCycle _dayNightCycle;

	private readonly WorkingHoursManager _workingHoursManager;

	private readonly UILayout _uiLayout;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly EventBus _eventBus;

	private RotatingBackground _needle;

	private RotatingBackground _workTimeEndMarker;

	private VisualElement _root;

	public ClockPanel(IDayNightCycle dayNightCycle, WorkingHoursManager workingHoursManager, UILayout uiLayout, VisualElementLoader visualElementLoader, EventBus eventBus)
	{
		_dayNightCycle = dayNightCycle;
		_workingHoursManager = workingHoursManager;
		_uiLayout = uiLayout;
		_visualElementLoader = visualElementLoader;
		_eventBus = eventBus;
	}

	public void Load()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/ClockPanel");
		_needle = _root.Q<RotatingBackground>("TimeNeedle");
		_workTimeEndMarker = _root.Q<RotatingBackground>("WorkingHoursNeedle");
		InitializeBorder();
		UpdateMovingParts();
		_eventBus.Register(this);
	}

	public void UpdateSingleton()
	{
		UpdateMovingParts();
	}

	[OnEvent]
	public void OnShowPrimaryUI(ShowPrimaryUIEvent showPrimaryUIEvent)
	{
		_uiLayout.AddTopRight(_root, int.MaxValue);
	}

	private void UpdateMovingParts()
	{
		_needle.SetRotation(NormalizeRotation(_dayNightCycle.DayProgress));
		_workTimeEndMarker.SetRotation(NormalizeRotation(_workingHoursManager.EndHours / 24f));
	}

	private void InitializeBorder()
	{
		float num = _dayNightCycle.DaytimeLengthInHours / 24f;
		float startingAngleOffset = StartingAngleOffset;
		float rotation = startingAngleOffset + num / 2f * 360f;
		float num2 = _dayNightCycle.NighttimeLengthInHours / 24f;
		float num3 = startingAngleOffset + num * 360f;
		float rotation2 = num3 + num2 / 2f * 360f;
		_root.Q<RotatingBackground>("DaytimeStart").SetRotation(startingAngleOffset);
		_root.Q<RotatingBackground>("Daytime").SetRotation(rotation);
		_root.Q<RotatingBackground>("NighttimeStart").SetRotation(num3);
		_root.Q<RotatingBackground>("Nighttime").SetRotation(rotation2);
	}

	private static float NormalizeRotation(float angle)
	{
		return angle * 360f + StartingAngleOffset;
	}
}
