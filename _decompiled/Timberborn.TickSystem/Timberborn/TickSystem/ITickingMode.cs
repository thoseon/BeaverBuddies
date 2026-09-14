namespace Timberborn.TickSystem;

public interface ITickingMode
{
	bool SingletonIsActiveInThisMode(object singleton);
}
