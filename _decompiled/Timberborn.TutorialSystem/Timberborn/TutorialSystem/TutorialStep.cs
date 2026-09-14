using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.Common;
using Timberborn.ToolButtonSystem;

namespace Timberborn.TutorialSystem;

public class TutorialStep
{
	public ITutorialStep Step { get; }

	public ToolGroupButton ToolGroupButton { get; }

	public ImmutableArray<ToolButton> ToolButtons { get; }

	public Action<bool> Highlight { get; }

	public string KeyBinding { get; }

	public string FixedKeyBinding { get; }

	private TutorialStep(ITutorialStep step, ToolGroupButton toolGroupButton, IEnumerable<ToolButton> toolButtons, Action<bool> highlight, string keyBinding, string fixedKeyBinding)
	{
		Step = step;
		ToolGroupButton = toolGroupButton;
		ToolButtons = toolButtons.ToImmutableArray();
		Highlight = highlight;
		KeyBinding = keyBinding;
		FixedKeyBinding = fixedKeyBinding;
	}

	public static TutorialStep Create(ITutorialStep step, Action<bool> highlight = null, string keyBinding = null, string fixedKeyBinding = null)
	{
		return new TutorialStep(step, null, Enumerable.Empty<ToolButton>(), highlight, keyBinding, fixedKeyBinding);
	}

	public static TutorialStep Create(ITutorialStep step, ToolGroupButton toolGroupButton, ToolButton toolButton, Action<bool> highlight = null)
	{
		return Create(step, toolGroupButton, Enumerables.One(toolButton), highlight);
	}

	public static TutorialStep Create(ITutorialStep step, ToolGroupButton toolGroupButton, IEnumerable<ToolButton> toolButtons, Action<bool> highlight = null)
	{
		return new TutorialStep(step, toolGroupButton, toolButtons, highlight, null, null);
	}

	public static TutorialStep Create(ITutorialStep step, string keyBinding, string fixedKeyBinding = null)
	{
		return new TutorialStep(step, null, Enumerable.Empty<ToolButton>(), null, keyBinding, fixedKeyBinding);
	}
}
