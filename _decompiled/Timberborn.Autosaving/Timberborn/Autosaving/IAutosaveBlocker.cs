namespace Timberborn.Autosaving;

public interface IAutosaveBlocker
{
	bool IsBlocking { get; }
}
