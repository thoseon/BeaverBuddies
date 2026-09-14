namespace Timberborn.SingletonSystem;

[Singleton]
public interface INonSingletonPostLoader
{
	void PostLoadNonSingletons();
}
