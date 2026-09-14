using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;

namespace Timberborn.TerrainLevelValidation;

public class BottomTerrainLevelValidationConstraint : BaseComponent, IAwakableComponent
{
	private IBottomLevelProvider _bottomLevelProvider;

	private BlockObject _blockObject;

	public int BottomLevel => _bottomLevelProvider?.BottomLevel ?? _blockObject.CoordinatesAtBaseZ.z;

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_bottomLevelProvider = _blockObject.GetComponent<IBottomLevelProvider>();
	}
}
