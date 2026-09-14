namespace Timberborn.Common;

public struct ValueChangedEventArgs<T>
{
	public T OldValue { get; }

	public T NewValue { get; }

	public ValueChangedEventArgs(T oldValue, T newValue)
	{
		OldValue = oldValue;
		NewValue = newValue;
	}
}
