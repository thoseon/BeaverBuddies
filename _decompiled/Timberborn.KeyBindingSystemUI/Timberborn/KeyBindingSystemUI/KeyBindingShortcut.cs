using Timberborn.KeyBindingSystem;

namespace Timberborn.KeyBindingSystemUI;

public class KeyBindingShortcut
{
	private readonly InputBindingDescriber _inputBindingDescriber;

	private readonly DefinableInputBinding _definableInputBinding;

	private readonly ShortcutTextElement _shortcutTextElement;

	public string KeyBindingId => _definableInputBinding.KeyBinding.Id;

	public KeyBindingShortcut(InputBindingDescriber inputBindingDescriber, DefinableInputBinding definableInputBinding, ShortcutTextElement shortcutTextElement)
	{
		_inputBindingDescriber = inputBindingDescriber;
		_definableInputBinding = definableInputBinding;
		_shortcutTextElement = shortcutTextElement;
	}

	public void Update()
	{
		if (_definableInputBinding.TryGetDefinedInputBinding(out var inputBinding))
		{
			_shortcutTextElement.SetShortcut(_inputBindingDescriber.GetInputBindingText(inputBinding));
		}
		else
		{
			_shortcutTextElement.SetUndefinedShortcut();
		}
	}
}
