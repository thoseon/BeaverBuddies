using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.DecalSystem;
using Timberborn.EntityPanelSystem;
using Timberborn.PlatformUtilities;
using Timberborn.SingletonSystem;
using UnityEngine.UIElements;

namespace Timberborn.DecalSystemUI;

internal class DecalSupplierFragment : IEntityPanelFragment
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly IDecalService _decalService;

	private readonly DecalButtonContainer _decalButtonContainer;

	private readonly IExplorerOpener _explorerOpener;

	private readonly EventBus _eventBus;

	private readonly UserDecalTextureRepository _userDecalTextureRepository;

	private VisualElement _root;

	private DecalSupplier _decalSupplier;

	public DecalSupplierFragment(VisualElementLoader visualElementLoader, IDecalService decalService, DecalButtonContainer decalButtonContainer, IExplorerOpener explorerOpener, EventBus eventBus, UserDecalTextureRepository userDecalTextureRepository)
	{
		_visualElementLoader = visualElementLoader;
		_decalService = decalService;
		_decalButtonContainer = decalButtonContainer;
		_explorerOpener = explorerOpener;
		_eventBus = eventBus;
		_userDecalTextureRepository = userDecalTextureRepository;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/DecalSupplierFragment");
		_root.ToggleDisplayStyle(visible: false);
		_decalButtonContainer.Initialize(_root);
		_root.Q<Button>("BrowseButton").RegisterCallback<ClickEvent>(OnBrowseButtonClicked);
		_root.Q<Button>("RefreshButton").RegisterCallback<ClickEvent>(OnRefreshButtonClicked);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_decalSupplier = entity.GetComponent<DecalSupplier>();
		if ((bool)_decalSupplier)
		{
			_decalButtonContainer.Show(_decalSupplier);
			_root.ToggleDisplayStyle(visible: true);
			_eventBus.Register(this);
		}
	}

	public void UpdateFragment()
	{
	}

	public void ClearFragment()
	{
		if ((bool)_decalSupplier)
		{
			_decalButtonContainer.Clear();
			_decalSupplier = null;
			_root.ToggleDisplayStyle(visible: false);
			_eventBus.Unregister(this);
		}
	}

	[OnEvent]
	public void OnDecalsReloaded(DecalsReloadedEvent decalsReloadedEvent)
	{
		_decalButtonContainer.Show(_decalSupplier);
	}

	private void OnBrowseButtonClicked(ClickEvent evt)
	{
		_explorerOpener.OpenDirectory(_userDecalTextureRepository.GetCustomDecalDirectory(_decalSupplier.Category));
	}

	private void OnRefreshButtonClicked(ClickEvent evt)
	{
		_decalService.ReloadCustomDecals(_decalSupplier.Category);
	}
}
