using Timberborn.Navigation;
using Timberborn.TemplateInstantiation;

namespace Timberborn.AccessibleNavigation;

internal class AccessibleInitializer : IDedicatedDecoratorInitializer<IAccessibleNeeder, Accessible>
{
	public void Initialize(IAccessibleNeeder subject, Accessible decorator)
	{
		decorator.Initialize(subject.AccessibleComponentName);
		subject.SetAccessible(decorator);
	}
}
