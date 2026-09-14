using Timberborn.CoreUI;
using Timberborn.SingletonSystem;
using Timberborn.UILayoutSystem;
using UnityEngine.UIElements;

namespace Timberborn.Benchmarking;

internal class BenchmarkPanel : ILoadableSingleton, IUpdatableSingleton
{
	private readonly UILayout _uiLayout;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly Benchmarker _benchmarker;

	private readonly EventBus _eventBus;

	private Label _info;

	private bool _benchmarkStarted;

	public BenchmarkPanel(UILayout uiLayout, VisualElementLoader visualElementLoader, Benchmarker benchmarker, EventBus eventBus)
	{
		_uiLayout = uiLayout;
		_visualElementLoader = visualElementLoader;
		_benchmarker = benchmarker;
		_eventBus = eventBus;
	}

	public void Load()
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/BenchmarkPanel");
		_info = visualElement.Q<Label>("Info");
		_info.ToggleDisplayStyle(visible: false);
		_uiLayout.AddAbsoluteItem(visualElement);
		_eventBus.Register(this);
	}

	public void UpdateSingleton()
	{
		if (_benchmarkStarted)
		{
			_info.text = $"Benchmark time left: {_benchmarker.GetTimeLeft():0.0}s" + "\nStage: " + (_benchmarker.WarmUpInProgress ? "warm up" : "sampling");
		}
	}

	[OnEvent]
	public void OnBenchmarkStarted(BenchmarkStartedEvent benchmarkStartedEvent)
	{
		_benchmarkStarted = true;
		_info.ToggleDisplayStyle(visible: true);
	}
}
