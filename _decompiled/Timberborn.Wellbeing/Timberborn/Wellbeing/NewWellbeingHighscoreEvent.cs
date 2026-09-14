namespace Timberborn.Wellbeing;

public class NewWellbeingHighscoreEvent
{
	public readonly int WellbeingHighscore;

	public NewWellbeingHighscoreEvent(int wellbeingHighscore)
	{
		WellbeingHighscore = wellbeingHighscore;
	}
}
