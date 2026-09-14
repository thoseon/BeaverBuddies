using System.Collections.Generic;
using System.Linq;
using Timberborn.CharacterControlSystem;

namespace Timberborn.CharacterControlSystemUI;

internal class ControllableCharacterAnimations
{
	private static readonly string DefaultAnimation = "CharacterControlAnimation";

	private readonly Dictionary<string, List<string>> _animations = new Dictionary<string, List<string>>();

	public IReadOnlyList<string> GatAnimations(ControllableCharacter controllableCharacter, string templateName)
	{
		if (!_animations.TryGetValue(templateName, out var value))
		{
			value = (from animationName in controllableCharacter.GetAnimationNames()
				orderby animationName
				select animationName).ToList();
			value.Remove(DefaultAnimation);
			value.Insert(0, DefaultAnimation);
			_animations[templateName] = value;
		}
		return value;
	}
}
