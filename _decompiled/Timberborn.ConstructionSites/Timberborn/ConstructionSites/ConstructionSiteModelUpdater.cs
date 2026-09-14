using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.Buildings;
using Timberborn.Common;
using Timberborn.EntitySystem;

namespace Timberborn.ConstructionSites;

internal class ConstructionSiteModelUpdater : BaseComponent, IAwakableComponent, IPostInitializableEntity
{
	private readonly List<IConstructionSiteValidator> _constructionSiteValidators = new List<IConstructionSiteValidator>();

	private BuildingModel _buildingModel;

	public void Awake()
	{
		_buildingModel = GetComponent<BuildingModel>();
		GetComponents(_constructionSiteValidators);
	}

	public void PostInitializeEntity()
	{
		foreach (IConstructionSiteValidator constructionSiteValidator in _constructionSiteValidators)
		{
			constructionSiteValidator.ValidationStateChanged += OnValidationStateChanged;
		}
		UpdateModel();
	}

	private void OnValidationStateChanged(object sender, EventArgs e)
	{
		UpdateModel();
	}

	private void UpdateModel()
	{
		if (_constructionSiteValidators.FastAll((IConstructionSiteValidator validator) => validator.IsModelValid))
		{
			_buildingModel.UnblockUnfinishedModel();
		}
		else
		{
			_buildingModel.BlockUnfinishedModel();
		}
	}
}
