using System.Collections.Generic;
using Timberborn.BlockSystem;
using Timberborn.Buildings;
using Timberborn.SingletonSystem;
using Timberborn.TemplateSystem;

namespace Timberborn.TutorialSteps;

internal class BuiltBuildingService : ILoadableSingleton
{
	private readonly EventBus _eventBus;

	private readonly Dictionary<string, List<Building>> _unfinishedBuildings = new Dictionary<string, List<Building>>();

	private readonly Dictionary<string, List<Building>> _finishedBuildings = new Dictionary<string, List<Building>>();

	public BuiltBuildingService(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnEnteredUnfinishedState(EnteredUnfinishedStateEvent enteredUnfinishedStateEvent)
	{
		Building component = enteredUnfinishedStateEvent.BlockObject.GetComponent<Building>();
		if (component != null)
		{
			GetMutableUnfinishedBuildings(component).Add(component);
		}
	}

	[OnEvent]
	public void OnExitedUnfinishedState(ExitedUnfinishedStateEvent exitedUnfinishedState)
	{
		Building component = exitedUnfinishedState.BlockObject.GetComponent<Building>();
		if (component != null)
		{
			GetMutableUnfinishedBuildings(component).Remove(component);
		}
	}

	[OnEvent]
	public void OnEnteredFinishedState(EnteredFinishedStateEvent enteredFinishedStateEvent)
	{
		Building component = enteredFinishedStateEvent.BlockObject.GetComponent<Building>();
		if (component != null)
		{
			GetMutableFinishedBuildings(component).Add(component);
		}
	}

	[OnEvent]
	public void OnExitedFinishedState(ExitedFinishedStateEvent exitedFinishedState)
	{
		Building component = exitedFinishedState.BlockObject.GetComponent<Building>();
		if (component != null)
		{
			GetMutableFinishedBuildings(component).Remove(component);
		}
	}

	public int NumberOfAllBuildings(IReadOnlyList<string> templateNames)
	{
		int num = 0;
		for (int i = 0; i < templateNames.Count; i++)
		{
			num += NumberOfAllBuildings(templateNames[i]);
		}
		return num;
	}

	public int NumberOfFinishedBuildings(IReadOnlyList<string> templateNames)
	{
		int num = 0;
		for (int i = 0; i < templateNames.Count; i++)
		{
			num += NumberOfFinishedBuildings(templateNames[i]);
		}
		return num;
	}

	public IReadOnlyList<Building> GetFinishedBuildings(string templateName)
	{
		return GetMutableFinishedBuildings(templateName);
	}

	public IReadOnlyList<Building> GetUnfinishedBuildings(string templateName)
	{
		return GetMutableUnfinishedBuildings(templateName);
	}

	private int NumberOfAllBuildings(string templateName)
	{
		return NumberOfUnfinishedBuildings(templateName) + NumberOfFinishedBuildings(templateName);
	}

	private int NumberOfFinishedBuildings(string templateName)
	{
		if (!_finishedBuildings.TryGetValue(templateName, out var value))
		{
			return 0;
		}
		return value.Count;
	}

	private int NumberOfUnfinishedBuildings(string templateName)
	{
		if (!_unfinishedBuildings.TryGetValue(templateName, out var value))
		{
			return 0;
		}
		return value.Count;
	}

	private List<Building> GetMutableFinishedBuildings(Building building)
	{
		string templateName = building.GetComponent<TemplateSpec>().TemplateName;
		return GetMutableFinishedBuildings(templateName);
	}

	private List<Building> GetMutableFinishedBuildings(string templateName)
	{
		return GetOrCreateBuildings(_finishedBuildings, templateName);
	}

	private List<Building> GetMutableUnfinishedBuildings(Building building)
	{
		string templateName = building.GetComponent<TemplateSpec>().TemplateName;
		return GetMutableUnfinishedBuildings(templateName);
	}

	private List<Building> GetMutableUnfinishedBuildings(string templateName)
	{
		return GetOrCreateBuildings(_unfinishedBuildings, templateName);
	}

	private static List<Building> GetOrCreateBuildings(Dictionary<string, List<Building>> allBuildings, string templateName)
	{
		if (!allBuildings.TryGetValue(templateName, out var value))
		{
			value = (allBuildings[templateName] = new List<Building>());
		}
		return value;
	}
}
