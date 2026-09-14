using Timberborn.CoreUI;
using Timberborn.MultithreadingAnalysis;
using Timberborn.TooltipSystem;

namespace Timberborn.MultithreadingAnalysisUI;

internal class TaskViewFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly TaskColorProvider _taskColorProvider;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	public TaskViewFactory(VisualElementLoader visualElementLoader, TaskColorProvider taskColorProvider, ITooltipRegistrar tooltipRegistrar)
	{
		_visualElementLoader = visualElementLoader;
		_taskColorProvider = taskColorProvider;
		_tooltipRegistrar = tooltipRegistrar;
	}

	public TaskView CreateTask(TaskSample task)
	{
		TaskView taskView = new TaskView(_visualElementLoader.LoadVisualElement("Common/MultithreadingAnalysis/TaskView"), color: _taskColorProvider.GetColor(task.GenericType), taskSample: task);
		taskView.Initialize();
		_tooltipRegistrar.Register(taskView.Root, taskView.GetTooltipText());
		return taskView;
	}
}
