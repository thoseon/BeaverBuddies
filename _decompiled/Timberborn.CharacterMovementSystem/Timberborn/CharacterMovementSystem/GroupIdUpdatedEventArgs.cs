namespace Timberborn.CharacterMovementSystem;

public readonly struct GroupIdUpdatedEventArgs
{
	public int GroupId { get; }

	public GroupIdUpdatedEventArgs(int groupId)
	{
		GroupId = groupId;
	}
}
