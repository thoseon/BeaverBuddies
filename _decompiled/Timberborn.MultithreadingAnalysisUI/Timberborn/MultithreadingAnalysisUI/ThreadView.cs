using UnityEngine.UIElements;

namespace Timberborn.MultithreadingAnalysisUI;

internal class ThreadView
{
	private readonly VisualElement _taskContainer;

	public VisualElement Root { get; }

	public ThreadView(VisualElement root, VisualElement taskContainer)
	{
		Root = root;
		_taskContainer = taskContainer;
	}

	public void AddTaskView(VisualElement taskViewRoot)
	{
		_taskContainer.Add(taskViewRoot);
	}

	public void SetScale(float pixelScale, long snapshotLength)
	{
		_taskContainer.style.width = new StyleLength(new Length(pixelScale * (float)snapshotLength, LengthUnit.Pixel));
	}
}
