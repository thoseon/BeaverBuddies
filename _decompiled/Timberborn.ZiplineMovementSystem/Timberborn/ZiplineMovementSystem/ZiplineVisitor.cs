using System;
using Timberborn.BaseComponentSystem;
using Timberborn.CharacterMovementSystem;
using Timberborn.EntitySystem;
using Timberborn.GameDistricts;
using Timberborn.MortalComponents;
using Timberborn.WalkingSystem;
using Timberborn.ZiplineSystem;

namespace Timberborn.ZiplineMovementSystem;

internal class ZiplineVisitor : BaseComponent, IAwakableComponent, IInitializableEntity, IPostLoadableEntity, IDeadNeededComponent
{
	private readonly ZiplineGroupService _ziplineGroupService;

	private MovementAnimator _movementAnimator;

	private ZiplinePathTracker _ziplinePathTracker;

	private NavMeshObserver _navMeshObserver;

	private Citizen _citizen;

	public bool IsOnZipline { get; private set; }

	public event EventHandler EnteredZipline;

	public event EventHandler ExitedZipline;

	public ZiplineVisitor(ZiplineGroupService ziplineGroupService)
	{
		_ziplineGroupService = ziplineGroupService;
	}

	public void Awake()
	{
		_movementAnimator = GetComponent<MovementAnimator>();
		_ziplinePathTracker = GetComponent<ZiplinePathTracker>();
		_citizen = GetComponent<Citizen>();
		_navMeshObserver = GetComponent<NavMeshObserver>();
	}

	public void InitializeEntity()
	{
		_movementAnimator.GroupIdUpdated += OnGroupIdUpdated;
		_citizen.ChangedAssignedDistrict += OnAssignedDistrictChanged;
		_navMeshObserver.PlacedOnNavMesh += OnPlacedOnNavMesh;
	}

	public void PostLoadEntity()
	{
		PostLoadValidateVisit();
	}

	private void PostLoadValidateVisit()
	{
		if (_ziplinePathTracker.IsOnNavMesh())
		{
			EnterZipline();
			_movementAnimator.SetPostLoadGroupId(_ziplineGroupService.RegularGroupId);
		}
	}

	private void OnGroupIdUpdated(object sender, GroupIdUpdatedEventArgs e)
	{
		bool flag = _ziplineGroupService.IsAnyZiplineGroup(e.GroupId);
		if (flag && !IsOnZipline)
		{
			EnterZipline();
		}
		else if (!flag && IsOnZipline)
		{
			ExitZipline();
		}
	}

	private void OnAssignedDistrictChanged(object sender, ChangeAssignedDistrictEventArgs e)
	{
		if (!_citizen.HasAssignedDistrict && !_ziplinePathTracker.IsOnNavMesh() && IsOnZipline)
		{
			ExitZipline();
		}
	}

	private void OnPlacedOnNavMesh(object sender, EventArgs e)
	{
		if (!_ziplinePathTracker.IsOnNavMesh() && IsOnZipline)
		{
			ExitZipline();
		}
	}

	private void EnterZipline()
	{
		IsOnZipline = true;
		EnteredZipline?.Invoke(this, EventArgs.Empty);
	}

	private void ExitZipline()
	{
		IsOnZipline = false;
		ExitedZipline?.Invoke(this, EventArgs.Empty);
	}
}
