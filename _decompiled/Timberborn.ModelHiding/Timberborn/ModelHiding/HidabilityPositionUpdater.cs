using Timberborn.BaseComponentSystem;
using Timberborn.BlockObjectModelSystem;
using Timberborn.BlockSystem;

namespace Timberborn.ModelHiding;

public class HidabilityPositionUpdater : BaseComponent, IAwakableComponent, IPrePlacementChangeListener, IPostPlacementChangeListener
{
	private readonly IModelAdder _modelAdder;

	private BlockObjectModelController _blockObjectModelController;

	public HidabilityPositionUpdater(IModelAdder modelAdder)
	{
		_modelAdder = modelAdder;
	}

	public void Awake()
	{
		_blockObjectModelController = GetComponent<BlockObjectModelController>();
	}

	public void OnPrePlacementChanged()
	{
		if (!_blockObjectModelController.BlockObject.IsPreview)
		{
			_modelAdder.RemoveModel(_blockObjectModelController);
		}
	}

	public void OnPostPlacementChanged()
	{
		if (!_blockObjectModelController.BlockObject.IsPreview)
		{
			_modelAdder.AddModel(_blockObjectModelController);
		}
	}
}
