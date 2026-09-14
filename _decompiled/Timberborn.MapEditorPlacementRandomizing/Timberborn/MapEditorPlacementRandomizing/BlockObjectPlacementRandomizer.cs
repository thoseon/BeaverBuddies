using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.EntitySystem;
using Timberborn.WorldPersistence;

namespace Timberborn.MapEditorPlacementRandomizing;

internal class BlockObjectPlacementRandomizer : BaseComponent, IPersistentEntity, IPostInitializableEntity
{
	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private readonly BlockObjectPlacementRandomizingService _blockObjectPlacementRandomizingService;

	private bool _wasLoaded;

	public BlockObjectPlacementRandomizer(IRandomNumberGenerator randomNumberGenerator, BlockObjectPlacementRandomizingService blockObjectPlacementRandomizingService)
	{
		_randomNumberGenerator = randomNumberGenerator;
		_blockObjectPlacementRandomizingService = blockObjectPlacementRandomizingService;
	}

	public void PostInitializeEntity()
	{
		if (!_wasLoaded && _blockObjectPlacementRandomizingService.Randomize)
		{
			BlockObject component = GetComponent<BlockObject>();
			component.Reposition(new Placement(component.Placement.Coordinates, GetRandomOrientation(), GetRandomFlipMode()));
		}
	}

	public void Save(IEntitySaver entitySaver)
	{
	}

	public void Load(IEntityLoader entityLoader)
	{
		_wasLoaded = true;
	}

	private Orientation GetRandomOrientation()
	{
		return (Orientation)_randomNumberGenerator.Range(0, 4);
	}

	private FlipMode GetRandomFlipMode()
	{
		return new FlipMode(_randomNumberGenerator.Range(0f, 1f) > 0.5f);
	}
}
