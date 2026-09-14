using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using UnityEngine;

namespace Timberborn.WaterBuildings;

internal class WaterInputFixedCoordinates : BaseComponent, IAwakableComponent, IPostInitializableEntity, IWaterInputCoordinates
{
	private BlockObject _blockObject;

	private WaterInputSpec _waterInputSpec;

	public Vector3Int Coordinates { get; private set; }

	public bool IsBlocked => false;

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_waterInputSpec = GetComponent<WaterInputSpec>();
	}

	public void PostInitializeEntity()
	{
		Coordinates = _blockObject.TransformCoordinates(_waterInputSpec.WaterInputCoordinates);
	}
}
