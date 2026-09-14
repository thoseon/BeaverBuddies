namespace Timberborn.BlockSystem;

public interface IFinishedPostLoadStateListener
{
	void OnEnterFinishedPostLoadState();

	void OnExitFinishedPostLoadState();
}
