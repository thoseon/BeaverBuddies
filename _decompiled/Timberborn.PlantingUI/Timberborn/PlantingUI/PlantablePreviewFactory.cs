using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.BaseComponentSystem;
using Timberborn.BlueprintSystem;
using Timberborn.Coordinates;
using Timberborn.Planting;
using Timberborn.Rendering;
using Timberborn.RootProviders;
using Timberborn.SingletonSystem;
using Timberborn.TemplateInstantiation;
using Timberborn.TemplateSystem;
using Timberborn.Timbermesh;
using UnityEngine;

namespace Timberborn.PlantingUI;

public class PlantablePreviewFactory : ILoadableSingleton
{
	private static readonly Vector3 PreviewOffset = new Vector3(0f, 0.01f, 0f);

	private readonly TemplateService _templateService;

	private readonly TemplateInstantiator _templateInstantiator;

	private readonly MaterialColorer _materialColorer;

	private readonly RootObjectProvider _rootObjectProvider;

	private Transform _parent;

	private readonly Dictionary<string, Blueprint> _previewBlueprints = new Dictionary<string, Blueprint>();

	public PlantablePreviewFactory(TemplateService templateService, TemplateInstantiator templateInstantiator, MaterialColorer materialColorer, RootObjectProvider rootObjectProvider)
	{
		_templateService = templateService;
		_templateInstantiator = templateInstantiator;
		_materialColorer = materialColorer;
		_rootObjectProvider = rootObjectProvider;
	}

	public void Load()
	{
		_parent = _rootObjectProvider.CreateRootObject("PlantablePreviewFactory").transform;
		foreach (PlantableSpec item in _templateService.GetAll<PlantableSpec>())
		{
			PlantablePreviewModelSpec spec = item.GetSpec<PlantablePreviewModelSpec>();
			if ((object)spec != null)
			{
				string templateName = item.GetSpec<TemplateSpec>().TemplateName;
				if (!(spec.Model != null))
				{
					throw new Exception("Empty model path in PlantablePreviewModelSpec for plantable " + templateName);
				}
				CreateBlueprint(templateName, spec);
			}
		}
	}

	public PlantablePreview CreatePreview(string resource, Vector3Int coords)
	{
		GameObject gameObject = _templateInstantiator.Instantiate(_previewBlueprints[resource], _parent);
		gameObject.transform.position = CoordinateSystem.GridToWorld(coords) + PreviewOffset;
		PlantablePreview componentSlow = gameObject.GetComponentSlow<PlantablePreview>();
		_materialColorer.EnableGrayscale(componentSlow);
		return componentSlow;
	}

	private void CreateBlueprint(string templateName, PlantablePreviewModelSpec modelSpec)
	{
		PlantablePreviewSpec plantablePreviewSpec = new PlantablePreviewSpec
		{
			Model = modelSpec.Model
		};
		TimbermeshSpec timbermeshSpec = new TimbermeshSpec
		{
			Model = modelSpec.Model
		};
		Blueprint value = new Blueprint(templateName + "-PreviewTemplate", new ComponentSpec[2] { plantablePreviewSpec, timbermeshSpec }, ImmutableArray<Blueprint>.Empty);
		_previewBlueprints.Add(templateName, value);
	}
}
