using Timberborn.BaseComponentSystem;
using Timberborn.ConstructionSites;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using UnityEngine.UIElements;

namespace Timberborn.ConstructionSitesUI;

internal class ConstructionSiteDebugFragment : IEntityPanelFragment
{
	private readonly DebugFragmentFactory _debugFragmentFactory;

	private ConstructionSite _constructionSite;

	private VisualElement _root;

	public ConstructionSiteDebugFragment(DebugFragmentFactory debugFragmentFactory)
	{
		_debugFragmentFactory = debugFragmentFactory;
	}

	public VisualElement InitializeFragment()
	{
		DebugFragmentButton debugFragmentButton = new DebugFragmentButton(OnFinishNowClick, "Finish now");
		_root = _debugFragmentFactory.Create(debugFragmentButton);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_constructionSite = entity.GetComponent<ConstructionSite>();
	}

	public void ClearFragment()
	{
		_constructionSite = null;
		UpdateFragment();
	}

	public void UpdateFragment()
	{
		_root.ToggleDisplayStyle((bool)(BaseComponent)(object)_constructionSite && ((BaseComponent)(object)_constructionSite).Enabled);
	}

	private void OnFinishNowClick()
	{
		if ((bool)(BaseComponent)(object)_constructionSite && ((BaseComponent)(object)_constructionSite).Enabled)
		{
			_constructionSite.FinishNow();
		}
	}
}
