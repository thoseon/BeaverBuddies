namespace Timberborn.BlockSystem;

public readonly struct PreviewState
{
	public bool IsBuildable { get; }

	public bool IsSingle { get; }

	public bool IsLast { get; }

	public static PreviewState BuildableSingle => new PreviewState(isBuildable: true, isSingle: true, isLast: true);

	public static PreviewState UnbuildableSingle => new PreviewState(isBuildable: false, isSingle: true, isLast: true);

	public static PreviewState BuildableLast => new PreviewState(isBuildable: true, isSingle: false, isLast: true);

	public static PreviewState UnbuildableLast => new PreviewState(isBuildable: false, isSingle: false, isLast: true);

	public static PreviewState BuildableNotLast => new PreviewState(isBuildable: true, isSingle: false, isLast: false);

	public static PreviewState UnbuildableNotLast => new PreviewState(isBuildable: false, isSingle: false, isLast: false);

	private PreviewState(bool isBuildable, bool isSingle, bool isLast)
	{
		IsBuildable = isBuildable;
		IsSingle = isSingle;
		IsLast = isLast;
	}
}
