namespace Timberborn.Navigation;

public interface IAccessibleNeeder
{
	string AccessibleComponentName { get; }

	void SetAccessible(Accessible accessible);
}
