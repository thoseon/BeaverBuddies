using System.Text;
using Timberborn.Common;
using Timberborn.KeyBindingSystem;

namespace Timberborn.KeyBindingSystemUI;

public class InputBindingDescriber
{
	private static readonly string KeySeparator = " + ";

	private readonly InputBindingNameService _inputBindingNameService;

	private readonly KeyBindingRegistry _keyBindingRegistry;

	private readonly StringBuilder _inputBindingText = new StringBuilder();

	public InputBindingDescriber(InputBindingNameService inputBindingNameService, KeyBindingRegistry keyBindingRegistry)
	{
		_inputBindingNameService = inputBindingNameService;
		_keyBindingRegistry = keyBindingRegistry;
	}

	public string GetInputBindingText(string keyBindingId)
	{
		KeyBinding keyBinding = _keyBindingRegistry.Get(keyBindingId);
		InputBinding inputBinding = (keyBinding.PrimaryInputBinding.IsDefined ? keyBinding.PrimaryInputBinding : keyBinding.SecondaryInputBinding);
		return GetInputBindingText(inputBinding);
	}

	public string GetInputBindingText(InputBinding inputBinding)
	{
		AddModifierIfUsed(inputBinding, InputModifiers.Ctrl);
		AddModifierIfUsed(inputBinding, InputModifiers.Cmd);
		AddModifierIfUsed(inputBinding, InputModifiers.Shift);
		AddModifierIfUsed(inputBinding, InputModifiers.Alt);
		_inputBindingText.Append(_inputBindingNameService.GetName(inputBinding));
		return _inputBindingText.ToStringAndClear();
	}

	public string GetKeyBindingDisplayName(string keyBindingId)
	{
		return _keyBindingRegistry.Get(keyBindingId).DisplayName;
	}

	private void AddModifierIfUsed(InputBinding inputBinding, InputModifiers inputModifier)
	{
		if (inputBinding.HasModifier(inputModifier))
		{
			string inputModifierName = _inputBindingNameService.GetInputModifierName(inputModifier);
			_inputBindingText.Append(inputModifierName);
			_inputBindingText.Append(KeySeparator);
		}
	}
}
