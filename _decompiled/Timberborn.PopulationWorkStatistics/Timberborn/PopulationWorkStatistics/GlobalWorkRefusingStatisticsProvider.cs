using Timberborn.Characters;
using Timberborn.PopulationStatisticsSystem;
using Timberborn.SingletonSystem;
using Timberborn.WorkSystem;

namespace Timberborn.PopulationWorkStatistics;

public class GlobalWorkRefusingStatisticsProvider : ILoadableSingleton, IWorkRefusingStatisticsProvider
{
	private readonly EventBus _eventBus;

	private readonly WorkRefuserRegistry _workRefuserRegistry = new WorkRefuserRegistry();

	public GlobalWorkRefusingStatisticsProvider(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	public WorkRefusingStatistics GetWorkRefusingStatistics(string workerType)
	{
		return _workRefuserRegistry.GetWorkRefusingStatistics(workerType);
	}

	[OnEvent]
	public void OnCharacterCreated(CharacterCreatedEvent characterCreatedEvent)
	{
		WorkRefuser component = characterCreatedEvent.Character.GetComponent<WorkRefuser>();
		if (component != null)
		{
			_workRefuserRegistry.AddWorkRefuser(component);
		}
	}

	[OnEvent]
	public void OnCharacterKilled(CharacterKilledEvent characterKilledEvent)
	{
		WorkRefuser component = characterKilledEvent.Character.GetComponent<WorkRefuser>();
		if (component != null)
		{
			_workRefuserRegistry.RemoveWorkRefuser(component);
		}
	}
}
