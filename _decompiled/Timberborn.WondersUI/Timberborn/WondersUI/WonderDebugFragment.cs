using Timberborn.BaseComponentSystem;
using Timberborn.ConstructionSites;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Wonders;
using UnityEngine.UIElements;

namespace Timberborn.WondersUI;

internal class WonderDebugFragment : IEntityPanelFragment
{
	private static readonly float BuildTimeAmount = 1000f;

	private readonly DebugFragmentFactory _debugFragmentFactory;

	private VisualElement _root;

	private Wonder _wonder;

	private ConstructionSite _constructionSite;

	public WonderDebugFragment(DebugFragmentFactory debugFragmentFactory)
	{
		_debugFragmentFactory = debugFragmentFactory;
	}

	public VisualElement InitializeFragment()
	{
		DebugFragmentButton debugFragmentButton = new DebugFragmentButton(OnProgressConstructionClick, "Progress construction");
		_root = _debugFragmentFactory.Create("Wonder", debugFragmentButton);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_wonder = entity.GetComponent<Wonder>();
		_constructionSite = entity.GetComponent<ConstructionSite>();
	}

	public void UpdateFragment()
	{
		_root.ToggleDisplayStyle(_wonder);
	}

	public void ClearFragment()
	{
		_wonder = null;
		_constructionSite = null;
	}

	private void OnProgressConstructionClick()
	{
		_constructionSite.IncreaseBuildTime(BuildTimeAmount);
	}
}
