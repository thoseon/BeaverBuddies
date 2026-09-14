using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.InputSystem;
using Timberborn.MultithreadingAnalysis;
using UnityEngine.UIElements;

namespace Timberborn.MultithreadingAnalysisUI;

internal class SnapshotTimeline : IInputProcessor
{
	private static readonly string ThreadLabelClass = "thread-label";

	private readonly InputService _inputService;

	private readonly ThreadViewFactory _threadViewFactory;

	private readonly TaskViewFactory _taskViewFactory;

	private readonly MarkerViewFactory _markerViewFactory;

	private readonly TaskColorProvider _taskColorProvider;

	private readonly List<ThreadView> _threadViews = new List<ThreadView>();

	private readonly List<TaskView> _taskViews = new List<TaskView>();

	private readonly List<MarkerView> _markerViews = new List<MarkerView>();

	private VisualElement _threadLabels;

	private VisualElement _timeline;

	private bool _transparencyEnabled;

	public SnapshotTimeline(InputService inputService, ThreadViewFactory threadViewFactory, TaskViewFactory taskViewFactory, MarkerViewFactory markerViewFactory, TaskColorProvider taskColorProvider)
	{
		_inputService = inputService;
		_threadViewFactory = threadViewFactory;
		_taskViewFactory = taskViewFactory;
		_markerViewFactory = markerViewFactory;
		_taskColorProvider = taskColorProvider;
	}

	public void Initialize(VisualElement root)
	{
		_threadLabels = root.Q<VisualElement>("ThreadLabels");
		_timeline = root.Q<VisualElement>("Timeline");
	}

	public bool ProcessInput()
	{
		if (_transparencyEnabled && _inputService.Cancel)
		{
			ResetTransparency();
			return true;
		}
		return false;
	}

	public void Open(Snapshot snapshot)
	{
		_taskColorProvider.InitializeFromSamples(snapshot.TaskSamples);
		foreach (IGrouping<Thread, TaskSample> item in from sample in snapshot.TaskSamples
			group sample by sample.Thread into grouping
			orderby grouping.Key.DisplayName()
			select grouping)
		{
			CreateTaskViews(item.Key, item);
		}
		CreateMarkerViews(snapshot.Markers);
		_inputService.AddInputProcessor(this);
	}

	public void Close()
	{
		foreach (TaskView taskView in _taskViews)
		{
			taskView.TaskViewClicked = (EventHandler)Delegate.Remove(taskView.TaskViewClicked, new EventHandler(OnTaskViewClicked));
		}
		_threadViews.Clear();
		_taskViews.Clear();
		_markerViews.Clear();
		_threadLabels.Clear();
		_timeline.Clear();
		_inputService.RemoveInputProcessor(this);
	}

	public void SetScale(float pixelScale, long referenceTimestamp, long snapshotLength)
	{
		foreach (ThreadView threadView in _threadViews)
		{
			threadView.SetScale(pixelScale, snapshotLength);
		}
		foreach (TaskView taskView in _taskViews)
		{
			taskView.SetScale(pixelScale, referenceTimestamp);
		}
		foreach (MarkerView markerView in _markerViews)
		{
			markerView.SetScale(pixelScale, referenceTimestamp);
		}
	}

	public void SetMarkerVisibility(bool isVisible)
	{
		foreach (MarkerView markerView in _markerViews)
		{
			markerView.Root.ToggleDisplayStyle(isVisible);
		}
	}

	private void CreateTaskViews(Thread thread, IEnumerable<TaskSample> taskSamples)
	{
		ThreadView threadView = _threadViewFactory.CreateThreadView();
		_timeline.Add(threadView.Root);
		_threadViews.Add(threadView);
		_threadLabels.Add(CreateThreadLabel(thread));
		foreach (TaskSample taskSample in taskSamples)
		{
			TaskView taskView = _taskViewFactory.CreateTask(taskSample);
			taskView.TaskViewClicked = (EventHandler)Delegate.Combine(taskView.TaskViewClicked, new EventHandler(OnTaskViewClicked));
			threadView.AddTaskView(taskView.Root);
			_taskViews.Add(taskView);
		}
	}

	private void CreateMarkerViews(IEnumerable<Marker> markers)
	{
		foreach (Marker marker in markers)
		{
			MarkerView markerView = _markerViewFactory.CreateMarker(marker);
			markerView.Root.ToggleDisplayStyle(visible: false);
			_timeline.Add(markerView.Root);
			_markerViews.Add(markerView);
		}
	}

	private static VisualElement CreateThreadLabel(Thread thread)
	{
		Label label = new Label(thread.DisplayName());
		label.AddToClassList(ThreadLabelClass);
		return label;
	}

	private void OnTaskViewClicked(object sender, EventArgs e)
	{
		Type genericType = ((TaskView)sender).TaskSample.GenericType;
		foreach (TaskView taskView in _taskViews)
		{
			if (taskView.TaskSample.GenericType == genericType)
			{
				taskView.UnsetTransparent();
			}
			else
			{
				taskView.SetTransparent();
			}
		}
		_transparencyEnabled = true;
	}

	private void ResetTransparency()
	{
		foreach (TaskView taskView in _taskViews)
		{
			taskView.UnsetTransparent();
		}
		_transparencyEnabled = false;
	}
}
