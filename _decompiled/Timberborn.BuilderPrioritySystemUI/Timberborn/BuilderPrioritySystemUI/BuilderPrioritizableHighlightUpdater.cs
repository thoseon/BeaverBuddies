using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BuilderPrioritySystem;
using Timberborn.PrioritySystem;

namespace Timberborn.BuilderPrioritySystemUI;

internal class BuilderPrioritizableHighlightUpdater : BaseComponent, IAwakableComponent
{
	private readonly BuilderPrioritizableHighlighter _builderPrioritizableHighlighter;

	private BuilderPrioritizable _builderPrioritizable;

	public BuilderPrioritizableHighlightUpdater(BuilderPrioritizableHighlighter builderPrioritizableHighlighter)
	{
		_builderPrioritizableHighlighter = builderPrioritizableHighlighter;
	}

	public void Awake()
	{
		_builderPrioritizable = GetComponent<BuilderPrioritizable>();
		_builderPrioritizable.PriorityChanged += OnPriorityChanged;
		_builderPrioritizable.PrioritizableEnabled += OnPrioritizableEnabled;
		_builderPrioritizable.PrioritizableDisabled += OnPrioritizableDisabled;
	}

	private void OnPriorityChanged(object sender, PriorityChangedEventArgs e)
	{
		_builderPrioritizableHighlighter.HighlightIfEnabled(_builderPrioritizable);
	}

	private void OnPrioritizableEnabled(object sender, EventArgs e)
	{
		_builderPrioritizableHighlighter.AddBuilderPrioritizable(_builderPrioritizable);
	}

	private void OnPrioritizableDisabled(object sender, EventArgs e)
	{
		_builderPrioritizableHighlighter.RemoveBuilderPrioritizable(_builderPrioritizable);
	}
}
