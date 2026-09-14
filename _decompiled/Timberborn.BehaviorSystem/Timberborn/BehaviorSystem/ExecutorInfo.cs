namespace Timberborn.BehaviorSystem;

public struct ExecutorInfo
{
	public string Name { get; }

	public float ElapsedTime { get; }

	public ExecutorInfo(string name, float elapsedTime)
	{
		Name = name;
		ElapsedTime = elapsedTime;
	}
}
