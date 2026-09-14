using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using Timberborn.SelectionSystem;
using Timberborn.TerrainSystem;

namespace Timberborn.Explosions;

internal class UnstableCoreVisualisation : BaseComponent, IDeletableEntity, ISelectionListener, IPostPlacementChangeListener, IAwakableComponent
{
	private readonly ExplosionVisualizerService _explosionVisualizerService;

	private readonly ITerrainService _terrainService;

	private UnstableCore _unstableCore;

	public UnstableCoreVisualisation(ExplosionVisualizerService explosionVisualizerService, ITerrainService terrainService)
	{
		_explosionVisualizerService = explosionVisualizerService;
		_terrainService = terrainService;
	}

	public void Awake()
	{
		_unstableCore = GetComponent<UnstableCore>();
	}

	public void DeleteEntity()
	{
		_explosionVisualizerService.ClearSelected(_unstableCore);
	}

	public void OnPostPlacementChanged()
	{
		_explosionVisualizerService.UpdateHighlight(_unstableCore);
	}

	public void OnSelect()
	{
		_explosionVisualizerService.UpdateHighlight(_unstableCore);
		_terrainService.TerrainHeightChanged += OnTerrainChanged;
	}

	public void OnUnselect()
	{
		_terrainService.TerrainHeightChanged -= OnTerrainChanged;
		_explosionVisualizerService.ClearSelected(_unstableCore);
	}

	private void OnTerrainChanged(object sender, TerrainHeightChangeEventArgs terrainChangedEvent)
	{
		_explosionVisualizerService.UpdateHighlight(_unstableCore);
	}
}
