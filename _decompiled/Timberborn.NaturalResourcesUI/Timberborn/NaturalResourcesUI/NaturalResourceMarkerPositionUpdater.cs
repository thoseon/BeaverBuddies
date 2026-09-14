using System;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.NaturalResources;
using Timberborn.NaturalResourcesModelSystem;
using Timberborn.Rendering;

namespace Timberborn.NaturalResourcesUI;

internal class NaturalResourceMarkerPositionUpdater : BaseComponent, IAwakableComponent
{
	private CoordinatesOffsetter _coordinatesOffsetter;

	private MarkerPosition _markerPosition;

	private EntityComponent _entityComponent;

	public void Awake()
	{
		_coordinatesOffsetter = GetComponent<CoordinatesOffsetter>();
		_markerPosition = GetComponent<MarkerPosition>();
		_entityComponent = GetComponent<EntityComponent>();
		GetComponent<NaturalResourceModel>().ModelChanged += OnModelChanged;
	}

	private void OnModelChanged(object sender, EventArgs e)
	{
		if (!_entityComponent.Deleted)
		{
			_markerPosition.UpdatePosition(_coordinatesOffsetter.CoordinatesOffset.XYZ());
		}
	}
}
