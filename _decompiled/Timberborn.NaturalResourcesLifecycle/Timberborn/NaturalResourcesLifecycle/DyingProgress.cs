using Timberborn.TimeSystem;

namespace Timberborn.NaturalResourcesLifecycle;

public readonly struct DyingProgress
{
	public bool IsDying { get; }

	public bool Died { get; }

	public float Progress { get; }

	public float DaysLeft { get; }

	public static DyingProgress Healthy => new DyingProgress(isDying: false, died: false, 0f, float.MaxValue);

	public static DyingProgress Dead => new DyingProgress(isDying: false, died: true, 1f, 0f);

	private DyingProgress(bool isDying, bool died, float progress, float daysLeft)
	{
		IsDying = isDying;
		Died = died;
		Progress = progress;
		DaysLeft = daysLeft;
	}

	public static DyingProgress Create(ITimeTrigger timeTrigger)
	{
		return new DyingProgress(timeTrigger.InProgress, timeTrigger.Finished, timeTrigger.Progress, timeTrigger.DaysLeft);
	}
}
