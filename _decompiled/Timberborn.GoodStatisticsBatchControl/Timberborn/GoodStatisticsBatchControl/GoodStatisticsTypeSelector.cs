using Timberborn.CoreUI;
using Timberborn.GoodStatisticsUI;
using Timberborn.SingletonSystem;
using UnityEngine.UIElements;

namespace Timberborn.GoodStatisticsBatchControl;

internal class GoodStatisticsTypeSelector
{
	private static readonly string GoodStatisticsTypeLocKeyPrefix = "BatchControl.GoodStatisticsType.";

	private readonly RadioToggleFactory _radioToggleFactory;

	private readonly EventBus _eventBus;

	private RadioToggle _goodStatisticsTypeToggle;

	public GoodStatisticType SelectedType { get; private set; }

	public GoodStatisticsTypeSelector(RadioToggleFactory radioToggleFactory, EventBus eventBus)
	{
		_radioToggleFactory = radioToggleFactory;
		_eventBus = eventBus;
	}

	public void Initialize(VisualElement root)
	{
		_goodStatisticsTypeToggle = _radioToggleFactory.CreateLocalizable<GoodStatisticType>(GoodStatisticsTypeLocKeyPrefix, root);
		_goodStatisticsTypeToggle.RadioButtonSelected += OnGoodStatisticsTypeChanged;
		_goodStatisticsTypeToggle.Update((int)SelectedType);
	}

	private void OnGoodStatisticsTypeChanged(object sender, int index)
	{
		SelectedType = (GoodStatisticType)index;
		_goodStatisticsTypeToggle.Update(index);
		_eventBus.Post(new GoodStatisticsTypeChangedEvent());
	}
}
