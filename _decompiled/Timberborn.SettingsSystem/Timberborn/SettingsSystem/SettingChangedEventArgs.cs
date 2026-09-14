namespace Timberborn.SettingsSystem;

public class SettingChangedEventArgs<T>
{
	public T Value { get; }

	public SettingChangedEventArgs(T value)
	{
		Value = value;
	}
}
