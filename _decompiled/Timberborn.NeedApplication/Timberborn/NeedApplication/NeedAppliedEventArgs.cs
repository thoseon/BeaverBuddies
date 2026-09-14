using Timberborn.Characters;
using Timberborn.Effects;

namespace Timberborn.NeedApplication;

public readonly struct NeedAppliedEventArgs
{
	public Character Character { get; }

	public InstantEffect NeedEffect { get; }

	public NeedAppliedEventArgs(Character character, InstantEffect needEffect)
	{
		Character = character;
		NeedEffect = needEffect;
	}
}
