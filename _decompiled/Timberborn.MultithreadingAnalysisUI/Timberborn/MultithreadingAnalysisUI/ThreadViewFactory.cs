using Timberborn.CoreUI;
using UnityEngine.UIElements;

namespace Timberborn.MultithreadingAnalysisUI;

internal class ThreadViewFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	public ThreadViewFactory(VisualElementLoader visualElementLoader)
	{
		_visualElementLoader = visualElementLoader;
	}

	public ThreadView CreateThreadView()
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Common/MultithreadingAnalysis/ThreadView");
		return new ThreadView(visualElement, visualElement.Q<VisualElement>("TaskContainer"));
	}
}
