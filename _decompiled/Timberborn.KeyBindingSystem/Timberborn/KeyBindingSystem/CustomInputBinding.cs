namespace Timberborn.KeyBindingSystem;

public class CustomInputBinding
{
	public static readonly CustomInputBinding UndefinedBinding = new CustomInputBinding(string.Empty, InputModifiers.None, null);

	public string Path { get; }

	public InputModifiers InputModifiers { get; }

	public string DefaultName { get; }

	public CustomInputBinding(string path, InputModifiers inputModifiers, string defaultName)
	{
		Path = path;
		InputModifiers = inputModifiers;
		DefaultName = defaultName;
	}

	public bool IsSame(InputBindingSpec inputBindingSpec)
	{
		if (Path == inputBindingSpec.Path)
		{
			return InputModifiers == inputBindingSpec.InputModifiers;
		}
		return false;
	}

	public InputBindingSpec ToInputBindingSpec()
	{
		return new InputBindingSpec
		{
			Path = Path,
			InputModifiers = InputModifiers
		};
	}
}
