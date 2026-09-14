namespace Timberborn.SingletonSystem;

[Singleton]
public interface IPostLoadableSingleton
{
	void PostLoad();
}
