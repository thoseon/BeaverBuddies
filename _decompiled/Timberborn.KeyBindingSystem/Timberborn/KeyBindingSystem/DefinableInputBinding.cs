namespace Timberborn.KeyBindingSystem;

public class DefinableInputBinding
{
	private readonly bool? _isPrimary;

	public KeyBinding KeyBinding { get; }

	public DefinableInputBinding(KeyBinding keyBinding, bool? isPrimary)
	{
		KeyBinding = keyBinding;
		_isPrimary = isPrimary;
	}

	public bool TryGetDefinedInputBinding(out InputBinding inputBinding)
	{
		inputBinding = GetInputBinding();
		return inputBinding.IsDefined;
	}

	public InputBinding GetSingleInputBinding()
	{
		if (!IsPrimary())
		{
			return KeyBinding.SecondaryInputBinding;
		}
		return KeyBinding.PrimaryInputBinding;
	}

	public bool IsPrimary()
	{
		return _isPrimary.Value;
	}

	private InputBinding GetInputBinding()
	{
		if (_isPrimary.HasValue)
		{
			return GetSingleInputBinding();
		}
		if (!KeyBinding.PrimaryInputBinding.IsDefined)
		{
			return KeyBinding.SecondaryInputBinding;
		}
		return KeyBinding.PrimaryInputBinding;
	}
}
