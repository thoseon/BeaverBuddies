using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.HttpApiSystem;
using UnityEngine.UIElements;

namespace Timberborn.HttpApiSystemUI;

internal class HttpLeverFragment : IEntityPanelFragment
{
	private readonly VisualElementLoader _visualElementLoader;

	private VisualElement _root;

	private TextField _switchOnUrlTextField;

	private TextField _switchOffUrlTextField;

	private HttpLever _httpLever;

	public HttpLeverFragment(VisualElementLoader visualElementLoader)
	{
		_visualElementLoader = visualElementLoader;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/HttpLeverFragment");
		_switchOnUrlTextField = _root.Q<TextField>("SwitchOnUrl");
		_switchOffUrlTextField = _root.Q<TextField>("SwitchOffUrl");
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		if (entity.TryGetComponent<HttpLever>(out _httpLever))
		{
			_root.ToggleDisplayStyle(visible: true);
		}
	}

	public void UpdateFragment()
	{
		if ((bool)_httpLever)
		{
			_switchOnUrlTextField.SetValueWithoutNotify(_httpLever.SwitchOnUrl);
			_switchOffUrlTextField.SetValueWithoutNotify(_httpLever.SwitchOffUrl);
		}
	}

	public void ClearFragment()
	{
		_httpLever = null;
		_root.ToggleDisplayStyle(visible: false);
	}
}
