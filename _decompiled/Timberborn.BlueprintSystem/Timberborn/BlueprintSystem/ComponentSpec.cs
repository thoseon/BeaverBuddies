namespace Timberborn.BlueprintSystem;

public abstract record ComponentSpec
{
	public Blueprint Blueprint { get; init; }

	internal bool DisableBlueprintCopying { get; set; }

	protected ComponentSpec()
	{
		Blueprint = new Blueprint(null, null, this);
	}

	protected ComponentSpec(ComponentSpec original)
	{
		if (original.DisableBlueprintCopying)
		{
			Blueprint = null;
		}
		else
		{
			Blueprint = new Blueprint(original.Blueprint, original, this);
		}
	}

	public bool HasSpec<T>()
	{
		return Blueprint.HasSpec<T>();
	}

	public T GetSpec<T>()
	{
		return Blueprint.GetSpec<T>();
	}
}
