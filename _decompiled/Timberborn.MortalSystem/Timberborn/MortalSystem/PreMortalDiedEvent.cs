namespace Timberborn.MortalSystem;

public class PreMortalDiedEvent
{
	public Mortal Mortal { get; }

	public PreMortalDiedEvent(Mortal mortal)
	{
		Mortal = mortal;
	}
}
