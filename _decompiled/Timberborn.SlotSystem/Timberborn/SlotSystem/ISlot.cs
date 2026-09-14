using Timberborn.EnterableSystem;

namespace Timberborn.SlotSystem;

public interface ISlot
{
	string Name { get; }

	Enterer AssignedEnterer { get; }

	bool IsAvailable { get; }

	void AssignEnterer(Enterer enterer);

	void UnassignEnterer();

	void Update(float deltaTime);
}
