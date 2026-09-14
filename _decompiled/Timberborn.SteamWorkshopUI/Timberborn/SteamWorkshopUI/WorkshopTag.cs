namespace Timberborn.SteamWorkshopUI;

public readonly struct WorkshopTag
{
	public WorkshopTagCategory Category { get; }

	public string Name { get; }

	public int Order { get; }

	public WorkshopTag(WorkshopTagCategory category, string name, int order)
	{
		Category = category;
		Name = name;
		Order = order;
	}
}
