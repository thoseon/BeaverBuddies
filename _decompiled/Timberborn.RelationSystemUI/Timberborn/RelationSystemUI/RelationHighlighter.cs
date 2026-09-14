using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlueprintSystem;
using Timberborn.RelationSystem;
using Timberborn.SelectionSystem;
using UnityEngine;

namespace Timberborn.RelationSystemUI;

internal class RelationHighlighter : BaseComponent, IAwakableComponent, ISelectionListener
{
	private readonly Highlighter _highlighter;

	private readonly ISpecService _specService;

	private readonly List<IRelationOwner> _relationOwners = new List<IRelationOwner>();

	private Color _relationSelectionColor;

	public RelationHighlighter(Highlighter highlighter, ISpecService specService)
	{
		_highlighter = highlighter;
		_specService = specService;
	}

	public void Awake()
	{
		GetComponents(_relationOwners);
		foreach (IRelationOwner relationOwner in _relationOwners)
		{
			relationOwner.RelationsChanged += OnRelationsChanged;
		}
		_relationSelectionColor = _specService.GetSingleSpec<RelationHighlighterSpec>().RelationSelection;
		DisableComponent();
	}

	public void OnSelect()
	{
		HighlightRelations();
		EnableComponent();
	}

	public void OnUnselect()
	{
		_highlighter.UnhighlightAllPrimary();
		DisableComponent();
	}

	private void OnRelationsChanged(object sender, EventArgs e)
	{
		if (base.Enabled)
		{
			_highlighter.UnhighlightAllPrimary();
			HighlightRelations();
		}
	}

	private void HighlightRelations()
	{
		foreach (IRelationOwner relationOwner in _relationOwners)
		{
			foreach (BaseComponent relation in relationOwner.GetRelations())
			{
				_highlighter.HighlightPrimary(relation, _relationSelectionColor);
			}
		}
	}
}
