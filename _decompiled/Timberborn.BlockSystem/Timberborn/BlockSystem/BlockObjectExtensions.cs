namespace Timberborn.BlockSystem;

public static class BlockObjectExtensions
{
	public static T GetComponentOfNullable<T>(this BlockObject component)
	{
		if (!component)
		{
			return default(T);
		}
		return component.GetComponent<T>();
	}
}
