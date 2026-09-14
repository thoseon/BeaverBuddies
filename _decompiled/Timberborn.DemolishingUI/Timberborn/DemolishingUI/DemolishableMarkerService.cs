using System;
using System.Collections.Generic;
using Timberborn.BlockObjectModelSystem;
using Timberborn.BlueprintSystem;
using Timberborn.CameraSystem;
using Timberborn.Demolishing;
using Timberborn.Rendering;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.DemolishingUI;

internal class DemolishableMarkerService : ILoadableSingleton, IUpdatableSingleton
{
	private static readonly Vector3 IconScale = new Vector3(0.3f, 0.3f, 0.3f);

	private readonly EventBus _eventBus;

	private readonly MeshDrawerFactory _meshDrawerFactory;

	private readonly CameraService _cameraService;

	private readonly ISpecService _specService;

	private MeshDrawer _meshDrawer;

	private readonly List<MarkerPosition> _marked = new List<MarkerPosition>();

	public DemolishableMarkerService(EventBus eventBus, MeshDrawerFactory meshDrawerFactory, CameraService cameraService, ISpecService specService)
	{
		_eventBus = eventBus;
		_meshDrawerFactory = meshDrawerFactory;
		_cameraService = cameraService;
		_specService = specService;
	}

	public void Load()
	{
		_eventBus.Register(this);
		DemolishableMarkerServiceSpec singleSpec = _specService.GetSingleSpec<DemolishableMarkerServiceSpec>();
		_meshDrawer = _meshDrawerFactory.Create(singleSpec.Mesh.Asset, singleSpec.Material.Asset);
	}

	public void UpdateSingleton()
	{
		DrawMarkers();
	}

	[OnEvent]
	public void OnDemolishableMarked(DemolishableMarkedEvent demolishableMarkedEvent)
	{
		Demolishable demolishable = demolishableMarkedEvent.Demolishable;
		_marked.Add(demolishable.GetComponent<MarkerPosition>());
		BlockObjectModelController component = demolishable.GetComponent<BlockObjectModelController>();
		if (component != null)
		{
			component.ModelsUpdated += OnModelsUpdated;
			if (!component.IsAnyModelShown || component.IsUncoveredModelShown)
			{
				_marked.Remove(demolishable.GetComponent<MarkerPosition>());
			}
		}
	}

	[OnEvent]
	public void OnDemolishableUnmarked(DemolishableUnmarkedEvent demolishableUnmarkedEvent)
	{
		Demolishable demolishable = demolishableUnmarkedEvent.Demolishable;
		_marked.Remove(demolishable.GetComponent<MarkerPosition>());
		BlockObjectModelController component = demolishable.GetComponent<BlockObjectModelController>();
		if (component != null)
		{
			component.ModelsUpdated -= OnModelsUpdated;
		}
	}

	private void DrawMarkers()
	{
		Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, _cameraService.FacingCamera, IconScale);
		for (int i = 0; i < _marked.Count; i++)
		{
			Vector3 position = _marked[i].Position;
			matrix.SetColumn(3, new Vector4(position.x, position.y, position.z, 1f));
			_meshDrawer.Draw(matrix);
		}
	}

	private void OnModelsUpdated(object sender, EventArgs args)
	{
		BlockObjectModelController blockObjectModelController = (BlockObjectModelController)sender;
		MarkerPosition component = blockObjectModelController.GetComponent<MarkerPosition>();
		bool flag = blockObjectModelController.IsAnyModelShown && !blockObjectModelController.IsUncoveredModelShown;
		if (flag && !_marked.Contains(component))
		{
			_marked.Add(component);
		}
		else if (!flag)
		{
			_marked.Remove(component);
		}
	}
}
