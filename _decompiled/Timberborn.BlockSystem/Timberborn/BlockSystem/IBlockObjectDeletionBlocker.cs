namespace Timberborn.BlockSystem;

public interface IBlockObjectDeletionBlocker
{
	bool NoForcedDelete { get; }

	bool IsStackedDeletionBlocked { get; }

	bool IsDeletionBlocked { get; }

	string ReasonLocKey { get; }
}
