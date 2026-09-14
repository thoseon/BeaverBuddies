namespace Timberborn.InputSystem;

public class InputBlocker
{
	private int _blockersCount;

	public bool IsBlocked => _blockersCount > 0;

	public void Block()
	{
		_blockersCount++;
	}

	public void Unblock()
	{
		_blockersCount--;
	}
}
