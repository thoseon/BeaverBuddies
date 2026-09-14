using System.Collections.Generic;
using Timberborn.AchievementSystem;
using Timberborn.MechanicalSystem;
using Timberborn.SingletonSystem;
using Timberborn.TickSystem;

namespace Timberborn.Achievements;

internal abstract class GeneratePowerWithAchievement<T> : Achievement, ITickableSingleton
{
	private readonly MechanicalGraphRegistry _mechanicalGraphRegistry;

	private readonly EventBus _eventBus;

	private readonly List<MechanicalGraph> _graphCandidates = new List<MechanicalGraph>();

	private readonly int _requiredPower;

	public override string Id { get; }

	protected GeneratePowerWithAchievement(MechanicalGraphRegistry mechanicalGraphRegistry, EventBus eventBus, string id, int requiredPower)
	{
		Id = id;
		_mechanicalGraphRegistry = mechanicalGraphRegistry;
		_eventBus = eventBus;
		_requiredPower = requiredPower;
	}

	public void Tick()
	{
		if (base.IsEnabled && IsAnyCandidateProperlyPowered())
		{
			Unlock();
		}
	}

	[OnEvent]
	public void OnMechanicalGraphCreated(MechanicalGraphCreatedEvent mechanicalGraphCreatedEvent)
	{
		FindGraphCandidates();
	}

	[OnEvent]
	public void OnMechanicalGraphRemoved(MechanicalGraphRemovedEvent mechanicalGraphRemovedEvent)
	{
		FindGraphCandidates();
	}

	[OnEvent]
	public void OnMechanicalGraphGeneratorAdded(MechanicalGraphGeneratorAddedEvent mechanicalGraphGeneratorAddedEvent)
	{
		FindGraphCandidates();
	}

	protected override void EnableInternal()
	{
		_eventBus.Register(this);
	}

	protected override void DisableInternal()
	{
		_eventBus.Unregister(this);
		_graphCandidates.Clear();
	}

	private void FindGraphCandidates()
	{
		_graphCandidates.Clear();
		foreach (MechanicalGraph mechanicalGraph in _mechanicalGraphRegistry.MechanicalGraphs)
		{
			if (IsValidGraphCandidate(mechanicalGraph))
			{
				_graphCandidates.Add(mechanicalGraph);
			}
		}
	}

	private static bool IsValidGraphCandidate(MechanicalGraph candidateGraph)
	{
		foreach (MechanicalNode generator in candidateGraph.Generators)
		{
			if (generator.GetComponent<T>() == null)
			{
				return false;
			}
		}
		return candidateGraph.NumberOfGenerators > 0;
	}

	private bool IsAnyCandidateProperlyPowered()
	{
		foreach (MechanicalGraph graphCandidate in _graphCandidates)
		{
			if (graphCandidate != null && graphCandidate.PowerSupply >= _requiredPower)
			{
				return true;
			}
		}
		return false;
	}
}
