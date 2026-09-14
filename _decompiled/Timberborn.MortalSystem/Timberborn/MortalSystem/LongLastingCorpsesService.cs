namespace Timberborn.MortalSystem;

public class LongLastingCorpsesService
{
	public bool Enabled { get; private set; }

	public void Toggle()
	{
		Enabled = !Enabled;
	}
}
