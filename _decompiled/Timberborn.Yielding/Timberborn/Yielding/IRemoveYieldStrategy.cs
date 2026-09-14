using Timberborn.ReservableSystem;

namespace Timberborn.Yielding;

public interface IRemoveYieldStrategy
{
	string Id { get; }

	ReservableReacher Reacher { get; }

	bool IsStillRemovable { get; }
}
