using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Characters;
using Timberborn.EntitySystem;
using Timberborn.Persistence;
using Timberborn.TimeSystem;
using Timberborn.Wonders;
using Timberborn.WorkSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.WonderPlanes;

internal class PlaneLauncher : BaseComponent, IAwakableComponent, IPersistentEntity, IDeletableEntity, IWonderBlocker
{
	private static readonly float DestructionDelayInHours = 0.5f;

	private static readonly ComponentKey PlaneLauncherKey = new ComponentKey("PlaneLauncher");

	private static readonly ListKey<Pilot> PilotsKey = new ListKey<Pilot>("Pilots");

	private static readonly PropertyKey<int> PilotsSentKey = new PropertyKey<int>("PilotsSent");

	private static readonly PropertyKey<float> PilotsDestructionProgressKey = new PropertyKey<float>("PilotsDestructionProgress");

	private readonly ITimeTriggerFactory _timeTriggerFactory;

	private readonly ReferenceSerializer _referenceSerializer;

	private PlaneLauncherRotator _planeLauncherRotator;

	private Wonder _wonder;

	private PlaneCatapult _planeCatapult;

	private Workplace _workplace;

	private BlockObjectCenter _blockObjectCenter;

	private readonly List<Pilot> _pilots = new List<Pilot>();

	private int _pilotsSent;

	private ITimeTrigger _pilotsDestructionTimeTrigger;

	public PlaneLauncher(ITimeTriggerFactory timeTriggerFactory, ReferenceSerializer referenceSerializer)
	{
		_timeTriggerFactory = timeTriggerFactory;
		_referenceSerializer = referenceSerializer;
	}

	public void Awake()
	{
		_planeLauncherRotator = GetComponent<PlaneLauncherRotator>();
		_wonder = GetComponent<Wonder>();
		_planeCatapult = GetComponent<PlaneCatapult>();
		_workplace = GetComponent<Workplace>();
		_blockObjectCenter = GetComponent<BlockObjectCenter>();
		_wonder.WonderActivated += OnWonderActivated;
		_planeLauncherRotator.RotationFinished += OnRotationFinished;
		_planeCatapult.PlaneCatapulted += OnPlaneCatapulted;
		GetComponent<WonderAnimationController>().StartAnimationFinished += OnStartAnimationFinished;
		_pilotsDestructionTimeTrigger = _timeTriggerFactory.Create(DestroyPilots, DestructionDelayInHours / 24f);
	}

	public void Save(IEntitySaver entitySaver)
	{
		IObjectSaver component = entitySaver.GetComponent(PlaneLauncherKey);
		component.Set(PilotsKey, _pilots, _referenceSerializer.Of<Pilot>());
		component.Set(PilotsSentKey, _pilotsSent);
		if (_pilotsDestructionTimeTrigger.InProgress)
		{
			component.Set(PilotsDestructionProgressKey, _pilotsDestructionTimeTrigger.Progress);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(PlaneLauncherKey);
		_pilots.AddRange(component.Get(PilotsKey, _referenceSerializer.Of<Pilot>()));
		_pilotsSent = component.Get(PilotsSentKey);
		if (component.Has(PilotsDestructionProgressKey))
		{
			float progress = component.Get(PilotsDestructionProgressKey);
			_pilotsDestructionTimeTrigger.FastForwardProgress(progress);
			_pilotsDestructionTimeTrigger.Resume();
		}
	}

	public void DeleteEntity()
	{
		_pilotsDestructionTimeTrigger.Pause();
		DestroyPilots();
	}

	public bool IsWonderBlocked()
	{
		return _pilots.Count > 0;
	}

	private void DestroyPilots()
	{
		foreach (Pilot pilot in _pilots)
		{
			pilot.GetComponent<Character>().DestroyCharacter();
		}
		_pilots.Clear();
		_pilotsSent = 0;
	}

	private void OnRotationFinished(object sender, EventArgs e)
	{
		if (_pilotsSent == _pilots.Count)
		{
			_wonder.Deactivate();
			_pilotsDestructionTimeTrigger.Reset();
			_pilotsDestructionTimeTrigger.Resume();
		}
		else
		{
			StartEjectingPlane();
		}
	}

	private void StartEjectingPlane()
	{
		Pilot pilot = _pilots[_pilotsSent++];
		_planeCatapult.CatapultPlane(pilot);
	}

	private void OnPlaneCatapulted(object sender, EventArgs e)
	{
		if (_pilotsSent < _pilots.Count)
		{
			float rotationAngle = 360f / (float)_pilots.Count;
			_planeLauncherRotator.StartRotation(rotationAngle);
		}
		else
		{
			_planeLauncherRotator.RotateToOriginalPosition();
		}
	}

	private void OnWonderActivated(object sender, EventArgs e)
	{
		TeleportAndInitializePilots();
	}

	private void TeleportAndInitializePilots()
	{
		int count = _workplace.AssignedWorkers.Count;
		for (int i = 0; i < count; i++)
		{
			Pilot component = ((BaseComponent)(object)_workplace.AssignedWorkers[i]).GetComponent<Pilot>();
			component.PrepareForFlying(_blockObjectCenter.WorldCenterGrounded);
			_pilots.Add(component);
		}
	}

	private void OnStartAnimationFinished(object sender, EventArgs e)
	{
		StartEjectingPlane();
	}
}
