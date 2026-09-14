using Timberborn.Automation;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BlueprintSystem;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.AutomationUI;

internal class TransmitterPickerToolHighlighter : ILoadableSingleton
{
	private readonly Highlighter _highlighter;

	private readonly AutomatorRegistry _automatorRegistry;

	private readonly ISpecService _specService;

	private Color _selectionColor;

	private Color _transmitterColor;

	private Color _unfinishedTransmitterColor;

	private Color _hoveredTransmitterColor;

	private BaseComponent _owner;

	private Automator _hoveredTransmitter;

	public TransmitterPickerToolHighlighter(Highlighter highlighter, AutomatorRegistry automatorRegistry, ISpecService specService)
	{
		_highlighter = highlighter;
		_automatorRegistry = automatorRegistry;
		_specService = specService;
	}

	public void Load()
	{
		_selectionColor = _specService.GetSingleSpec<SelectionColorsSpec>().EntitySelection;
		TransmitterPickerColorsSpec singleSpec = _specService.GetSingleSpec<TransmitterPickerColorsSpec>();
		_transmitterColor = singleSpec.TransmitterColor;
		_unfinishedTransmitterColor = singleSpec.UnfinishedTransmitterColor;
		_hoveredTransmitterColor = singleSpec.HoveredTransmitterColor;
	}

	public void Highlight(BaseComponent owner)
	{
		_owner = owner;
		_highlighter.UnhighlightAllPrimary();
		foreach (Automator transmitter in _automatorRegistry.Transmitters)
		{
			HighlightTransmitter(transmitter);
		}
	}

	public void UpdateHover(Automator hoveredTransmitter)
	{
		if (hoveredTransmitter != _hoveredTransmitter)
		{
			if (_hoveredTransmitter != null)
			{
				HighlightTransmitter(_hoveredTransmitter);
			}
			_hoveredTransmitter = hoveredTransmitter;
			if (_hoveredTransmitter != null)
			{
				_highlighter.HighlightPrimary(_hoveredTransmitter, _hoveredTransmitterColor);
			}
		}
	}

	public void Clear()
	{
		_highlighter.UnhighlightAllPrimary();
		_owner = null;
		_hoveredTransmitter = null;
	}

	private void HighlightTransmitter(Automator transmitter)
	{
		Color color = ((transmitter.GameObject == _owner.GameObject) ? _selectionColor : (transmitter.GetComponent<BlockObject>().IsFinished ? _transmitterColor : _unfinishedTransmitterColor));
		_highlighter.HighlightPrimary(transmitter, color);
	}
}
