namespace Timberborn.TemplateInstantiation;

public interface IDedicatedDecoratorInitializer<in TSubject, in TDecorator>
{
	void Initialize(TSubject subject, TDecorator decorator);
}
