using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.Rendering;
using Timberborn.StatusSystem;
using UnityEngine;

namespace Timberborn.Demolishing;

public class DemolishableStatusIconOffsetter : BaseComponent, IAwakableComponent, IStatusIconOffsetter, IPreInitializableEntity, IDeletableEntity
{
	private static readonly float Offset = 1f;

	private readonly IStatusIconOffsetService _statusIconOffsetService;

	private readonly BoundsCalculator _boundsCalculator;

	private Demolishable _demolishable;

	private MarkerPosition _markerPosition;

	public float TopBound { get; private set; }

	public Vector3 Position { get; private set; }

	public Vector2Int Key { get; private set; }

	public BlockObject BlockObject { get; private set; }

	public float UnfinishedTopBound => TopBound;

	public float FinishedTopBound => TopBound;

	public bool StatusActive => _demolishable.IsMarked;

	public DemolishableStatusIconOffsetter(IStatusIconOffsetService statusIconOffsetService, BoundsCalculator boundsCalculator)
	{
		_statusIconOffsetService = statusIconOffsetService;
		_boundsCalculator = boundsCalculator;
	}

	public void Awake()
	{
		BlockObject = GetComponent<BlockObject>();
		_demolishable = GetComponent<Demolishable>();
		_markerPosition = GetComponent<MarkerPosition>();
		_demolishable.Marked += delegate
		{
			UpdateIcon();
		};
	}

	public void PreInitializeEntity()
	{
		Position = GetComponent<BlockObjectCenter>().GridCenter;
		Key = new Vector2Int(Mathf.RoundToInt(Position.x * 2f), Mathf.RoundToInt(Position.y * 2f));
		_statusIconOffsetService.AddOffsetter(this);
		TopBound = _boundsCalculator.GetRendererYMaxBound(base.Transform) + Offset;
		_statusIconOffsetService.UpdateIcons(this);
	}

	public void DeleteEntity()
	{
		_statusIconOffsetService.RemoveOffsetter(this);
		_statusIconOffsetService.UpdateIcons(this);
	}

	public void UpdateIcon()
	{
		float z = _statusIconOffsetService.CalculateVerticalPosition(this) - Position.z - Offset;
		_markerPosition.UpdatePosition(new Vector3(0f, 0f, z));
	}
}
