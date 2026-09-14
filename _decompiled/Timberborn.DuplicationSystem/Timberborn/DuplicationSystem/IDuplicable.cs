namespace Timberborn.DuplicationSystem;

public interface IDuplicable
{
	bool IsDuplicable => true;
}
public interface IDuplicable<T> : IDuplicable
{
	void DuplicateFrom(T source);
}
