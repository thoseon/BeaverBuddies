using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockObjectModelSystem;
using Timberborn.BlockSystem;
using Timberborn.Buildings;
using Timberborn.Common;
using Timberborn.ConstructionMode;
using Timberborn.ConstructionSites;
using Timberborn.Coordinates;
using Timberborn.EntitySystem;
using Timberborn.StatusSystem;
using UnityEngine;

namespace Timberborn.BuildingStatuses;

public class BuildingStatusIconOffsetter : BaseComponent, IAwakableComponent, IPreInitializableEntity, IPostLoadableEntity, IDeletableEntity, IStatusIconOffsetter
{
	private static readonly float FinishedOffset = 0.7f;

	private static readonly float UnfinishedOffset = 1f;

	private readonly IStatusIconOffsetService _statusIconOffsetService;

	private readonly BoundsCalculator _boundsCalculator;

	private readonly ConstructionModeService _constructionModeService;

	private BuildingModel _buildingModel;

	private StatusIconCycler _statusIconCycler;

	private BlockObjectModelController _blockObjectModelController;

	private StatusVisibilityToggle _statusVisibilityToggle;

	private ConstructionSiteProgressVisualizer _constructionSiteProgressVisualizer;

	private bool _isInitialized;

	private bool _shouldAlwaysUseFinishedBound;

	public Vector3 Position { get; private set; }

	public Vector2Int Key { get; private set; }

	public BlockObject BlockObject { get; private set; }

	public float FinishedTopBound { get; private set; }

	public float UnfinishedTopBound { get; private set; }

	public bool StatusActive => _statusIconCycler.VisibleAndActive;

	public float TopBound
	{
		get
		{
			if (!ShouldUseFinishedBound)
			{
				return GetUnfinishedTopBound();
			}
			return FinishedTopBound;
		}
	}

	private bool IsPreviewModelBlocked => _buildingModel.IsUnfinishedModelBlocked;

	private bool IsShown => _blockObjectModelController.IsAnyModelShown;

	private bool ShouldUseFinishedBound
	{
		get
		{
			if (!_shouldAlwaysUseFinishedBound && (!_constructionModeService.InConstructionMode || GetComponent<BuildingModel>().UnfinishedConstructionModeModel))
			{
				return BlockObject.IsFinished;
			}
			return true;
		}
	}

	public BuildingStatusIconOffsetter(IStatusIconOffsetService statusIconOffsetService, BoundsCalculator boundsCalculator, ConstructionModeService constructionModeService)
	{
		_statusIconOffsetService = statusIconOffsetService;
		_boundsCalculator = boundsCalculator;
		_constructionModeService = constructionModeService;
	}

	public void Awake()
	{
		BlockObject = GetComponent<BlockObject>();
		_buildingModel = GetComponent<BuildingModel>();
		_blockObjectModelController = GetComponent<BlockObjectModelController>();
		_blockObjectModelController.ModelsUpdated += OnModelChanged;
		_statusIconCycler = GetComponent<StatusIconCycler>();
		_statusVisibilityToggle = _statusIconCycler.GetStatusVisibilityToggle();
		_constructionSiteProgressVisualizer = GetComponent<ConstructionSiteProgressVisualizer>();
		if ((bool)_constructionSiteProgressVisualizer)
		{
			_constructionSiteProgressVisualizer.StageChanged += OnModelChanged;
		}
	}

	public void PreInitializeEntity()
	{
		_statusIconCycler.InitializeIcon(base.Transform, 0.5f);
	}

	public void PostLoadEntity()
	{
		_statusIconCycler.ActiveStateChanged += OnActiveStateChanged;
		Position = GetComponent<BlockObjectCenter>().GridCenter;
		Key = new Vector2Int(Mathf.RoundToInt(Position.x * 2f), Mathf.RoundToInt(Position.y * 2f));
		_statusIconOffsetService.AddOffsetter(this);
		_statusIconCycler.Root.transform.position = CoordinateSystem.GridToWorld(Position);
		RefreshTopBounds();
		_statusIconOffsetService.UpdateIcons(this);
		_isInitialized = true;
	}

	public void DeleteEntity()
	{
		_statusIconOffsetService.RemoveOffsetter(this);
		_statusIconOffsetService.UpdateIcons(this);
	}

	public void UpdateIcon()
	{
		if (_isInitialized)
		{
			SetIconVisibility();
			SetIconPosition();
		}
	}

	private void RefreshTopBounds()
	{
		float finishedTopBound = FinishedTopBound;
		float unfinishedTopBound = UnfinishedTopBound;
		FinishedTopBound = GetFinishedTopBound();
		UnfinishedTopBound = GetUnfinishedTopBound();
		_shouldAlwaysUseFinishedBound = UnfinishedTopBound > FinishedTopBound;
		if (_isInitialized && (!Mathf.Approximately(finishedTopBound, FinishedTopBound) || !Mathf.Approximately(unfinishedTopBound, UnfinishedTopBound)))
		{
			_statusIconOffsetService.UpdatePositions(this);
		}
	}

	private float GetFinishedTopBound()
	{
		return _boundsCalculator.GetRendererYMaxBound(_buildingModel.FinishedModel.transform) + FinishedOffset;
	}

	private float GetUnfinishedTopBound()
	{
		if (IsPreviewModelBlocked)
		{
			return BlockObject.CoordinatesAtBaseZ.z;
		}
		if ((bool)_constructionSiteProgressVisualizer)
		{
			float num = (_constructionSiteProgressVisualizer.ShouldShowProgress ? FinishedOffset : UnfinishedOffset);
			return _boundsCalculator.GetEnabledRendererYMaxBound(_buildingModel.UnfinishedModel.transform) + num;
		}
		return (_buildingModel.HasUnfinishedModel ? _boundsCalculator.GetRendererYMaxBound(_buildingModel.UnfinishedModel.transform) : 0f) + UnfinishedOffset;
	}

	private void OnModelChanged(object sender, EventArgs e)
	{
		if (_isInitialized)
		{
			RefreshTopBounds();
			UpdateIcon();
		}
	}

	private void OnActiveStateChanged(object sender, EventArgs eventArgs)
	{
		_statusIconOffsetService.UpdateIcons(this);
	}

	private void SetIconVisibility()
	{
		if (IsShown)
		{
			_statusVisibilityToggle.Show();
		}
		else
		{
			_statusVisibilityToggle.Hide();
		}
	}

	private void SetIconPosition()
	{
		if (StatusActive)
		{
			float y = _statusIconOffsetService.CalculateVerticalPosition(this) - Position.z;
			_statusIconCycler.SetIconLocalPosition(new Vector3(0f, y, 0f));
		}
	}
}
