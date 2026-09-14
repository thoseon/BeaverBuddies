using Timberborn.BaseComponentSystem;
using Timberborn.CharacterModelSystem;
using Timberborn.EnterableSystem;
using Timberborn.EntitySystem;
using Timberborn.MortalComponents;
using Timberborn.Persistence;
using Timberborn.StatusSystem;
using Timberborn.WalkingSystem;
using Timberborn.Wandering;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.WonderPlanes;

internal class Pilot : BaseComponent, IAwakableComponent, IPersistentEntity, IPostLoadableEntity, IDeletableEntity
{
	private static readonly string AnimationName = "Piloting";

	private static readonly ComponentKey PilotKey = new ComponentKey("Pilot");

	private static readonly PropertyKey<Plane> PlaneKey = new PropertyKey<Plane>("Plane");

	private static readonly PropertyKey<Vector3> PlaneLauncherPositionKey = new PropertyKey<Vector3>("PlaneLauncherPosition");

	private readonly DeadComponentDisabler _deadComponentDisabler;

	private readonly EntityService _entityService;

	private readonly ReferenceSerializer _referenceSerializer;

	private CharacterModel _characterModel;

	private CharacterAnimator _characterAnimator;

	private StrandedStatus _strandedStatus;

	private NavMeshObserver _navMeshObserver;

	private StatusVisibilityToggle _statusVisibilityToggle;

	private Enterer _enterer;

	private Vector3? _planeLauncherPosition;

	private Plane _plane;

	private CharacterModelBlockadeIgnoringToggle _characterModelBlockadeIgnoringToggle;

	private bool IsFlying => _plane;

	public Pilot(DeadComponentDisabler deadComponentDisabler, EntityService entityService, ReferenceSerializer referenceSerializer)
	{
		_deadComponentDisabler = deadComponentDisabler;
		_entityService = entityService;
		_referenceSerializer = referenceSerializer;
	}

	public void Awake()
	{
		_characterModel = GetComponent<CharacterModel>();
		_characterAnimator = GetComponent<CharacterAnimator>();
		_strandedStatus = GetComponent<StrandedStatus>();
		_enterer = GetComponent<Enterer>();
		_navMeshObserver = GetComponent<NavMeshObserver>();
		StatusIconCycler componentInChildren = GetComponentInChildren<StatusIconCycler>(includeInactive: true);
		_statusVisibilityToggle = componentInChildren.GetStatusVisibilityToggle();
		_characterModelBlockadeIgnoringToggle = _characterModel.CreateBlockadeIgnoringToggle();
		DisableComponent();
	}

	public void PostLoadEntity()
	{
		if (_planeLauncherPosition.HasValue)
		{
			PrepareForFlying(_planeLauncherPosition.Value);
		}
		ShowPilot();
	}

	public void Save(IEntitySaver entitySaver)
	{
		if ((bool)_plane || _planeLauncherPosition.HasValue)
		{
			IObjectSaver component = entitySaver.GetComponent(PilotKey);
			if ((bool)_plane)
			{
				component.Set(PlaneKey, _plane, _referenceSerializer.Of<Plane>());
			}
			if (_planeLauncherPosition.HasValue)
			{
				component.Set(PlaneLauncherPositionKey, _planeLauncherPosition.Value);
			}
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(PilotKey, out var objectLoader))
		{
			_planeLauncherPosition = (objectLoader.Has(PlaneLauncherPositionKey) ? new Vector3?(objectLoader.Get(PlaneLauncherPositionKey)) : ((Vector3?)null));
			if (objectLoader.Has(PlaneKey))
			{
				AssignPlane(objectLoader.Get(PlaneKey, _referenceSerializer.Of<Plane>()));
			}
		}
	}

	public void DeleteEntity()
	{
		if ((bool)_plane)
		{
			_entityService.Delete(_plane);
		}
	}

	public void PrepareForFlying(Vector3 planeLauncherPosition)
	{
		_planeLauncherPosition = planeLauncherPosition;
		_enterer.UnreserveSlotAndExit();
		_deadComponentDisabler.DisableComponentsDeadDoNotNeed(this);
		_characterModel.Hide();
		_characterAnimator.SetBool(AnimationName, value: true);
		_navMeshObserver.Disable();
		_characterModel.Position = _planeLauncherPosition.Value;
		EnableComponent();
	}

	public void AssignPlane(Plane plane)
	{
		_plane = plane;
		base.Transform.SetParent(plane.PilotSeatTransform);
		ShowPilot();
	}

	private void ShowPilot()
	{
		if (IsFlying)
		{
			_characterModelBlockadeIgnoringToggle.Block();
			_characterModel.Show();
			_statusVisibilityToggle.Hide();
			_characterModel.PositionAtTarget(_plane.PilotSeatTransform);
			_strandedStatus.Disable();
		}
	}
}
