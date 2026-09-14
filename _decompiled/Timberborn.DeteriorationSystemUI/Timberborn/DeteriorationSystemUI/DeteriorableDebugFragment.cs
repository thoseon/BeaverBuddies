using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.DeteriorationSystem;
using Timberborn.EntityPanelSystem;
using UnityEngine.UIElements;

namespace Timberborn.DeteriorationSystemUI;

internal class DeteriorableDebugFragment : IEntityPanelFragment
{
	private readonly DebugFragmentFactory _debugFragmentFactory;

	private Deteriorable _deteriorable;

	private VisualElement _root;

	public DeteriorableDebugFragment(DebugFragmentFactory debugFragmentFactory)
	{
		_debugFragmentFactory = debugFragmentFactory;
	}

	public VisualElement InitializeFragment()
	{
		DebugFragmentButton debugFragmentButton = new DebugFragmentButton(Expire, "Set durability to zero");
		_root = _debugFragmentFactory.Create(debugFragmentButton);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_deteriorable = entity.GetComponent<Deteriorable>();
	}

	public void ClearFragment()
	{
		_deteriorable = null;
	}

	public void UpdateFragment()
	{
		_root.ToggleDisplayStyle((bool)(BaseComponent)(object)_deteriorable && ((BaseComponent)(object)_deteriorable).Enabled);
	}

	private void Expire()
	{
		if ((bool)(BaseComponent)(object)_deteriorable && ((BaseComponent)(object)_deteriorable).Enabled)
		{
			_deteriorable.SetDeteriorationToZero();
		}
	}
}
