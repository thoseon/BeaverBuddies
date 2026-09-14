using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.MechanicalSystem;

namespace Timberborn.MechanicalSystemHighlighting;

internal class PreviewMechanicalNodeHighlighter : BaseComponent, IAwakableComponent, IPreviewSelectionListener
{
	private readonly MechanicalGraphHighlightService _mechanicalGraphHighlightService;

	private MechanicalNode _mechanicalNode;

	public PreviewMechanicalNodeHighlighter(MechanicalGraphHighlightService mechanicalGraphHighlightService)
	{
		_mechanicalGraphHighlightService = mechanicalGraphHighlightService;
	}

	public void Awake()
	{
		_mechanicalNode = GetComponent<MechanicalNode>();
	}

	public void OnPreviewSelect()
	{
		_mechanicalGraphHighlightService.AddNodeToHighlight(_mechanicalNode);
	}

	public void OnPreviewUnselect()
	{
		_mechanicalGraphHighlightService.RemoveAllNodesFromHighlight();
	}
}
