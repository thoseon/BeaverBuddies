namespace Timberborn.SkySystem;

public readonly struct DayStageTransition
{
	public DayStage CurrentDayStage { get; }

	public string CurrentDayStageHazardousWeatherId { get; }

	public DayStage NextDayStage { get; }

	public string NextDayStageHazardousWeatherId { get; }

	public float TransitionProgress { get; }

	public DayStageTransition(DayStage currentDayStage, string currentDayStageHazardousWeatherId, DayStage nextDayStage, string nextDayStageHazardousWeatherId, float transitionProgress)
	{
		CurrentDayStage = currentDayStage;
		CurrentDayStageHazardousWeatherId = currentDayStageHazardousWeatherId;
		NextDayStage = nextDayStage;
		NextDayStageHazardousWeatherId = nextDayStageHazardousWeatherId;
		TransitionProgress = transitionProgress;
	}
}
