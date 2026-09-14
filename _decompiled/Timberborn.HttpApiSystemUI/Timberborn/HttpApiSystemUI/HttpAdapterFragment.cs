using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.DropdownSystem;
using Timberborn.EntityPanelSystem;
using Timberborn.HttpApiSystem;
using Timberborn.Localization;
using UnityEngine.UIElements;

namespace Timberborn.HttpApiSystemUI;

internal class HttpAdapterFragment : IEntityPanelFragment
{
	private static readonly string StatusLocKey = "Building.HttpAdapter.Status";

	private static readonly string StatusOKLocKey = "Building.HttpAdapter.Status.OK";

	private static readonly string StatusErrorLocKey = "Building.HttpAdapter.Status.Error";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly DropdownItemsSetter _dropdownItemsSetter;

	private readonly EnumDropdownProviderFactory _enumDropdownProviderFactory;

	private readonly ILoc _loc;

	private EnumDropdownProvider<HttpWebhookMethod> _methodDropdownProvider;

	private VisualElement _root;

	private Dropdown _methodDropdown;

	private Toggle _switchedOnWebhookEnabledToggle;

	private Toggle _switchedOffWebhookEnabledToggle;

	private TextField _switchedOnWebhookUrlTextField;

	private TextField _switchedOffWebhookUrlTextField;

	private Label _switchedOnWebhookStatusLabel;

	private Label _switchedOffWebhookStatusLabel;

	private HttpAdapter _httpAdapter;

	public HttpAdapterFragment(VisualElementLoader visualElementLoader, DropdownItemsSetter dropdownItemsSetter, EnumDropdownProviderFactory enumDropdownProviderFactory, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_dropdownItemsSetter = dropdownItemsSetter;
		_enumDropdownProviderFactory = enumDropdownProviderFactory;
		_loc = loc;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/HttpAdapterFragment");
		_methodDropdown = _root.Q<Dropdown>("Method");
		_methodDropdownProvider = _enumDropdownProviderFactory.Create(() => _httpAdapter.Method, delegate(HttpWebhookMethod method)
		{
			_httpAdapter.Method = method;
		}, (string method) => method.ToString().ToUpper());
		_switchedOnWebhookEnabledToggle = _root.Q<Toggle>("SwitchedOnWebhookEnabled");
		_switchedOffWebhookEnabledToggle = _root.Q<Toggle>("SwitchedOffWebhookEnabled");
		_switchedOnWebhookUrlTextField = _root.Q<TextField>("SwitchedOnWebhookUrl");
		_switchedOffWebhookUrlTextField = _root.Q<TextField>("SwitchedOffWebhookUrl");
		_switchedOnWebhookStatusLabel = _root.Q<Label>("SwitchedOnWebhookStatus");
		_switchedOffWebhookStatusLabel = _root.Q<Label>("SwitchedOffWebhookStatus");
		_switchedOnWebhookEnabledToggle.RegisterValueChangedCallback(delegate
		{
			OnWebhooksChanged();
		});
		_switchedOffWebhookEnabledToggle.RegisterValueChangedCallback(delegate
		{
			OnWebhooksChanged();
		});
		_switchedOnWebhookUrlTextField.RegisterValueChangedCallback(delegate
		{
			OnWebhooksChanged();
		});
		_switchedOffWebhookUrlTextField.RegisterValueChangedCallback(delegate
		{
			OnWebhooksChanged();
		});
		_switchedOnWebhookUrlTextField.isDelayed = true;
		_switchedOffWebhookUrlTextField.isDelayed = true;
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		if (entity.TryGetComponent<HttpAdapter>(out _httpAdapter))
		{
			_switchedOnWebhookEnabledToggle.SetValueWithoutNotify(_httpAdapter.SwitchedOnWebhookEnabled);
			_switchedOffWebhookEnabledToggle.SetValueWithoutNotify(_httpAdapter.SwitchedOffWebhookEnabled);
			_switchedOnWebhookUrlTextField.SetValueWithoutNotify(_httpAdapter.SwitchedOnWebhookUrl);
			_switchedOffWebhookUrlTextField.SetValueWithoutNotify(_httpAdapter.SwitchedOffWebhookUrl);
			_root.ToggleDisplayStyle(visible: true);
			_dropdownItemsSetter.SetItems(_methodDropdown, _methodDropdownProvider);
		}
	}

	public void UpdateFragment()
	{
		if ((bool)_httpAdapter)
		{
			UpdateStatusLabel(_switchedOnWebhookStatusLabel, _httpAdapter.LastOnCallSuccessful);
			UpdateStatusLabel(_switchedOffWebhookStatusLabel, _httpAdapter.LastOffCallSuccessful);
		}
	}

	public void ClearFragment()
	{
		_httpAdapter = null;
		_root.ToggleDisplayStyle(visible: false);
	}

	private void UpdateStatusLabel(Label label, bool? successful)
	{
		if (successful.HasValue)
		{
			label.text = _loc.T(StatusLocKey, successful.Value ? _loc.T(StatusOKLocKey) : _loc.T(StatusErrorLocKey));
			label.ToggleDisplayStyle(visible: true);
		}
		else
		{
			label.ToggleDisplayStyle(visible: false);
		}
	}

	private void OnWebhooksChanged()
	{
		_httpAdapter.SwitchedOnWebhookEnabled = _switchedOnWebhookEnabledToggle.value;
		_httpAdapter.SwitchedOffWebhookEnabled = _switchedOffWebhookEnabledToggle.value;
		_httpAdapter.SwitchedOnWebhookUrl = _switchedOnWebhookUrlTextField.value;
		_httpAdapter.SwitchedOffWebhookUrl = _switchedOffWebhookUrlTextField.value;
	}
}
