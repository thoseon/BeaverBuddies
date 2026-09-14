using System;
using System.Linq;
using Timberborn.CoreUI;
using Timberborn.InputSystem;
using Timberborn.MultithreadingAnalysis;
using Timberborn.RootProviders;
using Timberborn.SingletonSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.MultithreadingAnalysisUI;

internal class TaskSnapshotPanel : ILoadableSingleton, ILateUpdatableSingleton, IInputProcessor
{
	private static readonly int MinScale = 100;

	private static readonly int MaxScale = 50000;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly RootVisualElementProvider _rootVisualElementProvider;

	private readonly SnapshotCollector _snapshotCollector;

	private readonly InputService _inputService;

	private readonly SnapshotTimeline _snapshotTimeline;

	private VisualElement _root;

	private VisualElement _panel;

	private ScrollView _scrollView;

	private Slider _scaleSlider;

	private TextField _scaleValue;

	private Toggle _showMarkers;

	private Label _taskCount;

	private Label _totalTaskTime;

	private Label _totalIdleTime;

	private Label _minTime;

	private Label _maxTime;

	private Snapshot _snapshot;

	private bool _shouldResetScale;

	private int _scale;

	private long _referenceTimestamp;

	private long _snapshotLength;

	private int _threadCount;

	private bool _isOpened;

	public TaskSnapshotPanel(VisualElementLoader visualElementLoader, RootVisualElementProvider rootVisualElementProvider, SnapshotCollector snapshotCollector, InputService inputService, SnapshotTimeline snapshotTimeline)
	{
		_visualElementLoader = visualElementLoader;
		_rootVisualElementProvider = rootVisualElementProvider;
		_snapshotCollector = snapshotCollector;
		_inputService = inputService;
		_snapshotTimeline = snapshotTimeline;
	}

	public void Load()
	{
		_root = _rootVisualElementProvider.Create("TaskSnapshotPanel", "Common/MultithreadingAnalysis/TaskSnapshotContainer", 2);
		_panel = _visualElementLoader.LoadVisualElement("Common/MultithreadingAnalysis/TaskSnapshotPanel");
		_root.Q<VisualElement>("TaskSnapshotContainer").Add(_panel);
		_minTime = _panel.Q<Label>("MinTime");
		_maxTime = _panel.Q<Label>("MaxTime");
		_scrollView = _panel.Q<ScrollView>("ScrollView");
		_scrollView.horizontalScrollerVisibility = ScrollerVisibility.Auto;
		_scrollView.verticalScrollerVisibility = ScrollerVisibility.Auto;
		_scrollView.mode = ScrollViewMode.VerticalAndHorizontal;
		_scaleSlider = _panel.Q<Slider>("ScaleSlider");
		_scaleSlider.lowValue = MinScale;
		_scaleSlider.highValue = MaxScale;
		_scaleSlider.RegisterValueChangedCallback(delegate(ChangeEvent<float> evt)
		{
			SetScale(evt.newValue);
		});
		_scaleValue = _panel.Q<TextField>("ScaleValue");
		_scaleValue.RegisterCallback<FocusOutEvent>(delegate
		{
			if (int.TryParse(_scaleValue.value, out var result))
			{
				SetScale(result);
			}
		});
		_showMarkers = _panel.Q<Toggle>("ShowMarkers");
		_showMarkers.RegisterValueChangedCallback(OnShowMarkersChanged);
		_taskCount = _panel.Q<Label>("TaskCount");
		_totalTaskTime = _panel.Q<Label>("TotalTaskTime");
		_totalIdleTime = _panel.Q<Label>("TotalIdleTime");
		_panel.Q<Button>("FitButton").RegisterCallback<ClickEvent>(delegate
		{
			SetScale(MinScale);
		});
		_panel.Q<Button>("TakeSnapshot").RegisterCallback<ClickEvent>(delegate
		{
			TakeNextSnapshot();
		});
		_panel.Q<Button>("CloseButton").RegisterCallback<ClickEvent>(delegate
		{
			Close();
		});
		_root.ToggleDisplayStyle(visible: false);
		_snapshotTimeline.Initialize(_panel);
		_snapshotCollector.SnapshotCollected += OnSnapshotsCollected;
	}

	public void LateUpdateSingleton()
	{
		if (_isOpened)
		{
			if (_shouldResetScale)
			{
				_shouldResetScale = false;
				SetScale(MinScale);
			}
			UpdateMinMaxLabels();
		}
	}

	public bool ProcessInput()
	{
		if (_inputService.UICancel)
		{
			Close();
			return true;
		}
		return false;
	}

	private void Close()
	{
		if (_isOpened)
		{
			_snapshot = null;
			_snapshotTimeline.Close();
			_inputService.RemoveInputProcessor(this);
			_root.ToggleDisplayStyle(visible: false);
			_isOpened = false;
		}
	}

	private void Open(Snapshot snapshot)
	{
		_snapshot = snapshot;
		_inputService.AddInputProcessor(this);
		_showMarkers.SetValueWithoutNotify(newValue: false);
		_threadCount = (from s in _snapshot.TaskSamples
			group s by s.Thread).Count();
		CalculateTime();
		UpdateStats();
		_snapshotTimeline.Open(snapshot);
		_root.ToggleDisplayStyle(visible: true);
		_shouldResetScale = true;
		_isOpened = true;
	}

	private void CalculateTime()
	{
		long num = _snapshot.TaskSamples.Select((TaskSample t) => t.StartTime).Min();
		long num2 = _snapshot.TaskSamples.Select((TaskSample t) => t.EndTime).Max();
		if (_showMarkers.value)
		{
			num = Math.Min(_snapshot.Markers.Select((Marker t) => t.Timestamp).Min(), num);
			num2 = Math.Max(_snapshot.Markers.Select((Marker t) => t.Timestamp).Max(), num2);
		}
		_snapshotLength = num2 - num;
		_referenceTimestamp = num;
	}

	private void UpdateStats()
	{
		_taskCount.text = $"Tasks: {_snapshot.TaskSamples.Count}";
		double num = _snapshot.TaskSamples.Select((TaskSample sample) => sample.EndTime - sample.StartTime).Sum((Func<long, double>)TaskSampleCalculator.TicksToMs);
		_totalTaskTime.text = $"Total task time: {num:0.000}ms";
		double num2 = TaskSampleCalculator.TicksToMs(_snapshotLength * _threadCount) - num;
		_totalIdleTime.text = $"Total idle time: {num2:0.000}ms";
	}

	private void SetScale(float scale)
	{
		Vector2 vector = _scrollView.scrollOffset / _scale;
		_scale = Mathf.Clamp((int)scale, MinScale, MaxScale);
		_snapshotTimeline.SetScale(GetPixelScale(), _referenceTimestamp, _snapshotLength);
		_scrollView.scrollOffset = vector * _scale;
		_scaleSlider.SetValueWithoutNotify(_scale);
		_scaleValue.SetValueWithoutNotify(_scale.ToString());
	}

	private float GetPixelScale()
	{
		return _scrollView.resolvedStyle.width / (float)_snapshotLength * (float)_scale / 100f;
	}

	private void UpdateMinMaxLabels()
	{
		float x = _scrollView.scrollOffset.x;
		long ticks = (long)(Mathf.Max(0f, x) / GetPixelScale());
		float width = _scrollView.contentContainer.resolvedStyle.width;
		float width2 = _scrollView.contentViewport.layout.width;
		float num = Math.Max(1f, width - width2);
		float t = Mathf.Clamp01(x / num);
		long ticks2 = (long)Mathf.Lerp((float)_snapshotLength / ((float)_scale / 100f), _snapshotLength, t);
		_minTime.text = $"{TaskSampleCalculator.TicksToMs(ticks):0.000}ms";
		_maxTime.text = $"{TaskSampleCalculator.TicksToMs(ticks2):0.000}ms";
	}

	private void OnSnapshotsCollected(object sender, Snapshot snapshot)
	{
		Close();
		Open(snapshot);
	}

	private void TakeNextSnapshot()
	{
		_snapshotCollector.ScheduleCollection(_snapshot.Ticks);
	}

	private void OnShowMarkersChanged(ChangeEvent<bool> showMarkers)
	{
		CalculateTime();
		UpdateStats();
		UpdateMinMaxLabels();
		_snapshotTimeline.SetScale(GetPixelScale(), _referenceTimestamp, _snapshotLength);
		_snapshotTimeline.SetMarkerVisibility(showMarkers.newValue);
	}
}
