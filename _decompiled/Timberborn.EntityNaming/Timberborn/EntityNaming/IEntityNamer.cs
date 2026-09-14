namespace Timberborn.EntityNaming;

public interface IEntityNamer
{
	int EntityNamerPriority { get; }

	string GenerateEntityName();
}
