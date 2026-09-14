namespace Timberborn.KeyBindingSystem;

public class KeyBindingDefinition
{
	public KeyBindingSpec KeyBindingSpec { get; }

	public InputBinding PrimaryInputBinding { get; private set; }

	public InputBinding SecondaryInputBinding { get; private set; }

	private KeyBindingDefinition(KeyBindingSpec keyBindingSpec)
	{
		KeyBindingSpec = keyBindingSpec;
	}

	public static KeyBindingDefinition Create(KeyBindingSpec keyBindingSpec, CustomInputBinding customPrimaryInputBinding, CustomInputBinding customSecondaryInputBinding)
	{
		KeyBindingDefinition keyBindingDefinition = new KeyBindingDefinition(keyBindingSpec);
		keyBindingDefinition.RebindPrimaryInputBinding(customPrimaryInputBinding);
		keyBindingDefinition.RebindSecondaryInputBinding(customSecondaryInputBinding);
		return keyBindingDefinition;
	}

	public void RebindPrimaryInputBinding(CustomInputBinding customInputBinding)
	{
		PrimaryInputBinding = CreateInputBinding(customInputBinding, KeyBindingSpec.GetSpec<PrimaryInputBindingSpec>());
	}

	public void RebindSecondaryInputBinding(CustomInputBinding customInputBinding)
	{
		SecondaryInputBinding = CreateInputBinding(customInputBinding, KeyBindingSpec.GetSpec<SecondaryInputBindingSpec>());
	}

	public void ResetToDefault()
	{
		RebindPrimaryInputBinding(null);
		RebindSecondaryInputBinding(null);
	}

	private InputBinding CreateInputBinding(CustomInputBinding customInputBinding, InputBindingSpec defaultInputBindingSpec)
	{
		return InputBinding.Create((customInputBinding != null) ? customInputBinding.ToInputBindingSpec() : defaultInputBindingSpec, customInputBinding?.DefaultName, KeyBindingSpec.AllowOtherModifiers);
	}
}
