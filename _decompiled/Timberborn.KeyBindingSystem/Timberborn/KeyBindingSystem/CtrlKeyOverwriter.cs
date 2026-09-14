using Timberborn.PlatformUtilities;

namespace Timberborn.KeyBindingSystem;

public class CtrlKeyOverwriter
{
	private static readonly string LeftCtrlKey = "leftCtrl";

	private static readonly string RightCtrlKey = "rightCtrl";

	private static readonly string LeftCmdKey = "leftMeta";

	private static readonly string RightCmdKey = "rightMeta";

	public InputBindingSpec OverwriteIfOnMacOS(InputBindingSpec inputBindingSpec)
	{
		if (!ApplicationPlatform.IsMacOS())
		{
			return inputBindingSpec;
		}
		return OverwriteCtrl(inputBindingSpec);
	}

	private static InputBindingSpec OverwriteCtrl(InputBindingSpec inputBindingSpec)
	{
		return inputBindingSpec with
		{
			Path = OverwritePath(inputBindingSpec.Path),
			InputModifiers = OverwriteModifiers(inputBindingSpec.InputModifiers)
		};
	}

	private static string OverwritePath(string path)
	{
		return path.Replace(LeftCtrlKey, LeftCmdKey).Replace(RightCtrlKey, RightCmdKey);
	}

	private static InputModifiers OverwriteModifiers(InputModifiers modifiers)
	{
		if (modifiers.HasFlag(InputModifiers.Ctrl))
		{
			modifiers &= ~InputModifiers.Ctrl;
			modifiers |= InputModifiers.Cmd;
		}
		return modifiers;
	}
}
