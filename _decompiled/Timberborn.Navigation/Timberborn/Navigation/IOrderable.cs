namespace Timberborn.Navigation;

public interface IOrderable<in T>
{
	bool IsLessThan(T other);
}
