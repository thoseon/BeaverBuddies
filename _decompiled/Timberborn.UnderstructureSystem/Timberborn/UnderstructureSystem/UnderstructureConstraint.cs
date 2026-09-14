using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using Timberborn.Localization;
using Timberborn.TemplateSystem;
using UnityEngine;

namespace Timberborn.UnderstructureSystem;

public class UnderstructureConstraint : BaseComponent, IAwakableComponent, IInitializableEntity, IPostInitializableEntity, IDeletableEntity
{
	private static readonly string UnsuitableBuildingBelowLocKey = "Buildings.UnsuitableBuildingBelow";

	private readonly EntityService _entityService;

	private readonly UnderstructureFinder _understructureFinder;

	private readonly TemplateNameMapper _templateNameMapper;

	private readonly ILoc _loc;

	private BlockObject _blockObject;

	private UnderstructureConstraintSpec _understructureConstraintSpec;

	private UnderstructureConstructionSiteValidator _understructureConstructionSiteValidator;

	private bool _initialized;

	private EntityComponent _understructureEntity;

	public string ErrorMessage { get; private set; }

	public EntityComponent UnderstructureEntity
	{
		get
		{
			if (!_initialized)
			{
				return FindUnderstructureEntity();
			}
			return _understructureEntity;
		}
	}

	public ImmutableArray<string> UnderstructureTemplateNames => _understructureConstraintSpec.UnderstructureTemplateNames;

	internal UnderstructureConstraint(EntityService entityService, UnderstructureFinder understructureFinder, TemplateNameMapper templateNameMapper, ILoc loc)
	{
		_entityService = entityService;
		_understructureFinder = understructureFinder;
		_templateNameMapper = templateNameMapper;
		_loc = loc;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_understructureConstraintSpec = GetComponent<UnderstructureConstraintSpec>();
		_understructureConstructionSiteValidator = GetComponent<UnderstructureConstructionSiteValidator>();
		ErrorMessage = BuildErrorMessage();
	}

	public void InitializeEntity()
	{
		_understructureEntity = FindUnderstructureEntity();
		_initialized = true;
	}

	public void PostInitializeEntity()
	{
		if ((bool)UnderstructureEntity)
		{
			Understructure component = UnderstructureEntity.GetComponent<Understructure>();
			component.Deleted += OnUnderstructureDeleted;
			component.EnteredFinishedState += OnUnderstructureEnteredFinishedState;
		}
		else
		{
			Debug.LogWarning("Understructure not found for " + base.Name + " - deleting it.");
			Delete();
		}
	}

	public void DeleteEntity()
	{
		if ((bool)UnderstructureEntity)
		{
			Understructure component = UnderstructureEntity.GetComponent<Understructure>();
			component.Deleted -= OnUnderstructureDeleted;
			component.EnteredFinishedState -= OnUnderstructureEnteredFinishedState;
		}
	}

	private void OnUnderstructureDeleted(object sender, EventArgs e)
	{
		Delete();
	}

	private void OnUnderstructureEnteredFinishedState(object sender, EventArgs e)
	{
		_understructureConstructionSiteValidator.Validate();
	}

	private EntityComponent FindUnderstructureEntity()
	{
		return _understructureFinder.FindNonStrict(_blockObject, this)?.GetComponent<EntityComponent>();
	}

	private void Delete()
	{
		_entityService.Delete(this);
	}

	private string BuildErrorMessage()
	{
		IEnumerable<string> values = UnderstructureTemplateNames.Select(GetTemplateDisplayName);
		return _loc.T(UnsuitableBuildingBelowLocKey, string.Join(", ", values));
	}

	private string GetTemplateDisplayName(string templateName)
	{
		if (!_templateNameMapper.TryGetTemplate(templateName, out var templateSpec))
		{
			return templateName;
		}
		return _loc.T(templateSpec.GetSpec<LabeledEntitySpec>().DisplayNameLocKey);
	}
}
