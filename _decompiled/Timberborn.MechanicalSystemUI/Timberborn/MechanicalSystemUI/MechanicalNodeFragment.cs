using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.MechanicalSystem;
using UnityEngine.UIElements;

namespace Timberborn.MechanicalSystemUI;

internal class MechanicalNodeFragment : IEntityPanelFragment
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly GeneratorFragmentService _generatorFragmentService;

	private readonly ConsumerFragmentService _consumerFragmentService;

	private readonly NetworkFragmentService _networkFragmentService;

	private VisualElement _root;

	private MechanicalNode _mechanicalNode;

	public MechanicalNodeFragment(VisualElementLoader visualElementLoader, GeneratorFragmentService generatorFragmentService, ConsumerFragmentService consumerFragmentService, NetworkFragmentService networkFragmentService)
	{
		_visualElementLoader = visualElementLoader;
		_generatorFragmentService = generatorFragmentService;
		_consumerFragmentService = consumerFragmentService;
		_networkFragmentService = networkFragmentService;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/MechanicalNodeFragment");
		_generatorFragmentService.Initialize(_root.Q<Label>("Generator"));
		_consumerFragmentService.Initialize(_root.Q<Label>("Consumer"));
		_networkFragmentService.Initialize(_root.Q<Label>("Network"));
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_mechanicalNode = entity.GetComponent<MechanicalNode>();
	}

	public void ClearFragment()
	{
		_mechanicalNode = null;
		_generatorFragmentService.Hide();
		_consumerFragmentService.Hide();
		_networkFragmentService.Hide();
		_root.ToggleDisplayStyle(visible: false);
	}

	public void UpdateFragment()
	{
		bool visible = false;
		if ((bool)_mechanicalNode && _mechanicalNode.Enabled)
		{
			visible = _generatorFragmentService.Update(_mechanicalNode) | _consumerFragmentService.Update(_mechanicalNode) | _networkFragmentService.Update(_mechanicalNode);
		}
		_root.ToggleDisplayStyle(visible);
	}
}
