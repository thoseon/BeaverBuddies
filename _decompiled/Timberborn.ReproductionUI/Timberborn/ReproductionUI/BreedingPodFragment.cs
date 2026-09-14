using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.InventorySystemUI;
using Timberborn.Localization;
using Timberborn.Reproduction;
using Timberborn.UIFormatters;
using UnityEngine.UIElements;

namespace Timberborn.ReproductionUI;

public class BreedingPodFragment : IEntityPanelFragment
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly InventoryFragmentBuilderFactory _inventoryFragmentBuilderFactory;

	private readonly ILoc _loc;

	private readonly Phrase _progressPhrase = Phrase.New("Breeding.Progress").FormatPercentFloored();

	private InventoryFragment _inventoryFragment;

	private BreedingPod _breedingPod;

	private VisualElement _root;

	private Timberborn.CoreUI.ProgressBar _progressBar;

	private Label _progressLabel;

	public BreedingPodFragment(VisualElementLoader visualElementLoader, InventoryFragmentBuilderFactory inventoryFragmentBuilderFactory, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_inventoryFragmentBuilderFactory = inventoryFragmentBuilderFactory;
		_loc = loc;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/BreedingPodFragment");
		_progressBar = _root.Q<Timberborn.CoreUI.ProgressBar>("ProgressBar");
		_progressLabel = _root.Q<Label>("ProgressLabel");
		_root.ToggleDisplayStyle(visible: false);
		VisualElement root = _root.Q<VisualElement>("BreedingPodInventoryFragment");
		_inventoryFragment = _inventoryFragmentBuilderFactory.CreateBuilder(root).ShowRowLimit().ShowEmptyRows()
			.Build();
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_breedingPod = entity.GetComponent<BreedingPod>();
		if ((bool)_breedingPod)
		{
			_inventoryFragment.ShowFragment(_breedingPod.Inventory);
		}
	}

	public void ClearFragment()
	{
		_breedingPod = null;
		_inventoryFragment.ClearFragment();
		UpdateFragment();
	}

	public void UpdateFragment()
	{
		if ((bool)_breedingPod && _breedingPod.Enabled)
		{
			float num = _breedingPod.CalculateProgress();
			_progressBar.SetProgress(num);
			_progressLabel.text = _loc.T(_progressPhrase, num);
			_inventoryFragment.UpdateFragment();
			_root.ToggleDisplayStyle(visible: true);
		}
		else
		{
			_root.ToggleDisplayStyle(visible: false);
		}
	}
}
