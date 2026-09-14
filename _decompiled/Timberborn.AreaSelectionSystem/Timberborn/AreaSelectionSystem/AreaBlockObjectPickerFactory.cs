using Timberborn.BlockObjectPickingSystem;

namespace Timberborn.AreaSelectionSystem;

public class AreaBlockObjectPickerFactory
{
	private readonly AreaSelectionController _areaSelectionController;

	private readonly AreaSelector _areaSelector;

	private readonly BlockObjectPicker _blockObjectPicker;

	public AreaBlockObjectPickerFactory(AreaSelectionController areaSelectionController, AreaSelector areaSelector, BlockObjectPicker blockObjectPicker)
	{
		_areaSelectionController = areaSelectionController;
		_areaSelector = areaSelector;
		_blockObjectPicker = blockObjectPicker;
	}

	public AreaBlockObjectPicker CreatePickingUpwards()
	{
		return Create(BlockObjectPickingMode.UpwardStack);
	}

	public AreaBlockObjectPicker CreatePickingDownwards()
	{
		return Create(BlockObjectPickingMode.DownwardStack);
	}

	private AreaBlockObjectPicker Create(BlockObjectPickingMode pickingMode)
	{
		return new AreaBlockObjectPicker(_areaSelectionController, _areaSelector, _blockObjectPicker, pickingMode);
	}
}
