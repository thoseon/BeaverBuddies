using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.GameDistrictsMigration;
using UnityEngine.UIElements;

namespace Timberborn.GameDistrictsMigrationBatchControl;

internal class PopulationDistributorBatchControlRowItem : IBatchControlRowItem, IUpdatableBatchControlRowItem
{
	private readonly IntegerField _minimum;

	private readonly AlternateClickable _decreaseMinimum;

	private readonly AlternateClickable _increaseMinimum;

	private readonly VisualElement _needingIcon;

	private readonly PopulationDistributor _populationDistributor;

	public VisualElement Root { get; }

	public PopulationDistributorBatchControlRowItem(VisualElement root, IntegerField minimum, AlternateClickable decreaseMinimum, AlternateClickable increaseMinimum, VisualElement needingIcon, PopulationDistributor populationDistributor)
	{
		Root = root;
		_minimum = minimum;
		_decreaseMinimum = decreaseMinimum;
		_increaseMinimum = increaseMinimum;
		_needingIcon = needingIcon;
		_populationDistributor = populationDistributor;
	}

	public void UpdateRowItem()
	{
		if (!_minimum.IsFocused())
		{
			_minimum.SetValueWithoutNotify(_populationDistributor.Minimum);
		}
		_decreaseMinimum.Root.SetEnabled(_populationDistributor.Minimum > 0);
		_decreaseMinimum.Update();
		_increaseMinimum.Update();
		_needingIcon.ToggleDisplayStyle(_populationDistributor.CanImmigrate);
	}
}
