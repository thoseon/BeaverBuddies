using System.Collections.Generic;
using Timberborn.BlockObjectModelSystem;
using Timberborn.BlueprintSystem;
using Timberborn.ConstructionMode;
using Timberborn.LevelVisibilitySystem;
using Timberborn.Rendering;
using Timberborn.RootProviders;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;
using Timberborn.TemplateInstantiation;
using UnityEngine;

namespace Timberborn.ZiplineSystem;

public class ZiplineCableRenderer : ILoadableSingleton, IUpdatableSingleton
{
	private static readonly string CableTemplatePath = "Models/ZiplineCable/ZiplineCable.blueprint";

	private readonly RootObjectProvider _rootObjectProvider;

	private readonly ConstructionModeService _constructionModeService;

	private readonly EventBus _eventBus;

	private readonly Highlighter _highlighter;

	private readonly MaterialColorer _materialColorer;

	private readonly TemplateInstantiator _templateInstantiator;

	private readonly ISpecService _specService;

	private Blueprint _cableTemplate;

	private Transform _root;

	private readonly Dictionary<CableKey, ZiplineCableModel> _cableModels = new Dictionary<CableKey, ZiplineCableModel>();

	private bool _layerVisibilityChanged;

	public ZiplineCableRenderer(RootObjectProvider rootObjectProvider, ConstructionModeService constructionModeService, EventBus eventBus, Highlighter highlighter, MaterialColorer materialColorer, TemplateInstantiator templateInstantiator, ISpecService specService)
	{
		_rootObjectProvider = rootObjectProvider;
		_constructionModeService = constructionModeService;
		_eventBus = eventBus;
		_highlighter = highlighter;
		_materialColorer = materialColorer;
		_templateInstantiator = templateInstantiator;
		_specService = specService;
	}

	public void Load()
	{
		_root = _rootObjectProvider.CreateRootObject("ZiplineCableRenderer").transform;
		_cableTemplate = _specService.GetBlueprint(CableTemplatePath);
		_eventBus.Register(this);
	}

	public void UpdateSingleton()
	{
		if (_layerVisibilityChanged)
		{
			_layerVisibilityChanged = false;
			UpdateLayerVisibility();
		}
	}

	[OnEvent]
	public void OnConstructionModeChanged(ConstructionModeChangedEvent constructionModeChangedEvent)
	{
		foreach (var (cableKey2, ziplineCableModel2) in _cableModels)
		{
			if (!ziplineCableModel2.IsActive)
			{
				UpdateVisibility(cableKey2, _constructionModeService.InConstructionMode);
			}
		}
	}

	[OnEvent]
	public void OnMaxVisibleTerrainLevelChanged(MaxVisibleLevelChangedEvent maxVisibleLevelChangedEvent)
	{
		_layerVisibilityChanged = true;
	}

	public void HighlightConnection(ZiplineTower ziplineTower, ZiplineTower otherZiplineTower, Color highlightColor)
	{
		if (TryGetCableModel(ziplineTower, otherZiplineTower, out var model, out var _))
		{
			model.Highlight(highlightColor);
		}
	}

	public void UnhighlightConnection(ZiplineTower ziplineTower, ZiplineTower otherZiplineTower)
	{
		if (TryGetCableModel(ziplineTower, otherZiplineTower, out var model, out var _))
		{
			model.Unhighlight();
		}
	}

	public void AddActiveConnection(ZiplineTower ziplineTower, ZiplineTower otherZiplineTower)
	{
		CableKey cableKey = CableKey.Create(ziplineTower, otherZiplineTower);
		CreateCableModel(cableKey);
		UpdateVisibility(cableKey, isVisible: true);
	}

	public void AddInactiveConnection(ZiplineTower ziplineTower, ZiplineTower otherZiplineTower)
	{
		CableKey cableKey = CableKey.Create(ziplineTower, otherZiplineTower);
		ZiplineCableModel ziplineCableModel = CreateCableModel(cableKey);
		ziplineCableModel.IsActive = false;
		ziplineCableModel.SetGreyscale(isGrayscale: true);
		UpdateVisibility(cableKey, _constructionModeService.InConstructionMode);
	}

	public void ActivateConnection(ZiplineTower ziplineTower, ZiplineTower otherZiplineTower)
	{
		if (TryGetCableModel(ziplineTower, otherZiplineTower, out var model, out var cableKey))
		{
			model.IsActive = true;
			model.SetGreyscale(isGrayscale: false);
			UpdateVisibility(cableKey, isVisible: true);
		}
	}

	public ZiplineCableModel CreateCableModel()
	{
		return new ZiplineCableModel(_materialColorer, _highlighter, _templateInstantiator.Instantiate(_cableTemplate, _root), _templateInstantiator.Instantiate(_cableTemplate, _root));
	}

	public void RemoveConnection(ZiplineTower ziplineTower, ZiplineTower otherZiplineTower)
	{
		if (TryGetCableModel(ziplineTower, otherZiplineTower, out var model, out var cableKey))
		{
			model.Destroy();
			_cableModels.Remove(cableKey);
		}
	}

	public void UpdateConnection(ZiplineTower ziplineTower, ZiplineTower otherZiplineTower)
	{
		if (TryGetCableModel(ziplineTower, otherZiplineTower, out var model, out var _))
		{
			model.UpdateModel(ziplineTower, otherZiplineTower);
		}
	}

	private void UpdateVisibility(CableKey cableKey, bool isVisible)
	{
		ZiplineCableModel ziplineCableModel = _cableModels[cableKey];
		ziplineCableModel.SetVisibility(isVisible);
		UpdateLayerVisibility(cableKey, ziplineCableModel);
	}

	private void UpdateLayerVisibility()
	{
		foreach (var (cableKey2, cableModel) in _cableModels)
		{
			UpdateLayerVisibility(cableKey2, cableModel);
		}
	}

	private static void UpdateLayerVisibility(CableKey cableKey, ZiplineCableModel cableModel)
	{
		BlockObjectModelController component = cableKey.ZiplineTower.GetComponent<BlockObjectModelController>();
		BlockObjectModelController component2 = cableKey.OtherZiplineTower.GetComponent<BlockObjectModelController>();
		cableModel.SetShadowOnly(!component.IsAnyModelShown || !component2.IsAnyModelShown);
	}

	private ZiplineCableModel CreateCableModel(CableKey cableKey)
	{
		ZiplineCableModel ziplineCableModel = CreateCableModel();
		ziplineCableModel.UpdateModel(cableKey.ZiplineTower, cableKey.OtherZiplineTower);
		_cableModels.Add(cableKey, ziplineCableModel);
		return ziplineCableModel;
	}

	private bool TryGetCableModel(ZiplineTower ziplineTower, ZiplineTower otherZiplineTower, out ZiplineCableModel model, out CableKey cableKey)
	{
		cableKey = CableKey.Create(ziplineTower, otherZiplineTower);
		return _cableModels.TryGetValue(cableKey, out model);
	}
}
