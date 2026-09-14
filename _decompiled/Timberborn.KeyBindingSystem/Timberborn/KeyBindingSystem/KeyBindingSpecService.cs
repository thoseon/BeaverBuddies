using System.Collections.Generic;
using System.Linq;
using Timberborn.BlueprintSystem;
using Timberborn.Common;
using Timberborn.SettingsSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.KeyBindingSystem;

public class KeyBindingSpecService : ILoadableSingleton
{
	private static readonly string PrimaryBindingFormat = "KeyBinding.{0}.Primary";

	private static readonly string SecondaryBindingFormat = "KeyBinding.{0}.Secondary";

	private readonly CustomInputBindingSerializer _customInputBindingSerializer;

	private readonly ISettings _settings;

	private readonly ISpecService _specService;

	private readonly CtrlKeyOverwriter _ctrlKeyOverwriter;

	private readonly EventBus _eventBus;

	private readonly List<KeyBindingDefinition> _keyBindingDefinitions = new List<KeyBindingDefinition>();

	public ReadOnlyList<KeyBindingDefinition> KeyBindingDefinitions => _keyBindingDefinitions.AsReadOnlyList();

	public KeyBindingSpecService(CustomInputBindingSerializer customInputBindingSerializer, ISettings settings, ISpecService specService, CtrlKeyOverwriter ctrlKeyOverwriter, EventBus eventBus)
	{
		_customInputBindingSerializer = customInputBindingSerializer;
		_settings = settings;
		_specService = specService;
		_ctrlKeyOverwriter = ctrlKeyOverwriter;
		_eventBus = eventBus;
	}

	public void Load()
	{
		foreach (KeyBindingSpec item in from spec in (from spec in _specService.GetSpecs<KeyBindingSpec>()
				where spec.Blueprint.IsAllowedByFeatureToggles()
				select spec).Select(Overwrite)
			orderby spec.Order
			select spec)
		{
			_keyBindingDefinitions.Add(KeyBindingDefinition.Create(item, GetCustomPrimaryBinding(item), GetCustomSecondaryBinding(item)));
		}
	}

	public void ResetToDefault()
	{
		foreach (KeyBindingDefinition keyBindingDefinition in _keyBindingDefinitions)
		{
			string id = keyBindingDefinition.KeyBindingSpec.Id;
			_settings.Clear(GetPrimarySettingsKey(id));
			_settings.Clear(GetSecondarySettingsKey(id));
			keyBindingDefinition.ResetToDefault();
			_eventBus.Post(new KeyReboundEvent(id));
		}
	}

	public void RebindInputBinding(DefinableInputBinding definableInputBinding, CustomInputBinding customInputBinding)
	{
		KeyBindingDefinition keyBindingDefinition = _keyBindingDefinitions.Single((KeyBindingDefinition key) => key.KeyBindingSpec.Id == definableInputBinding.KeyBinding.Id);
		string settingsKey;
		if (definableInputBinding.IsPrimary())
		{
			keyBindingDefinition.RebindPrimaryInputBinding(customInputBinding);
			settingsKey = GetPrimarySettingsKey(definableInputBinding.KeyBinding.Id);
		}
		else
		{
			keyBindingDefinition.RebindSecondaryInputBinding(customInputBinding);
			settingsKey = GetSecondarySettingsKey(definableInputBinding.KeyBinding.Id);
		}
		SaveInputBinding(customInputBinding, settingsKey);
		definableInputBinding.KeyBinding.Lock();
		_eventBus.Post(new KeyReboundEvent(definableInputBinding.KeyBinding.Id));
	}

	private void SaveInputBinding(CustomInputBinding customInputBinding, string settingsKey)
	{
		string value = _customInputBindingSerializer.Serialize(customInputBinding);
		_settings.SetString(settingsKey, value);
	}

	private CustomInputBinding GetCustomPrimaryBinding(KeyBindingSpec keyBindingSpec)
	{
		string primarySettingsKey = GetPrimarySettingsKey(keyBindingSpec.Id);
		return GetBindingFromSettings(primarySettingsKey);
	}

	private CustomInputBinding GetCustomSecondaryBinding(KeyBindingSpec keyBindingSpec)
	{
		string secondarySettingsKey = GetSecondarySettingsKey(keyBindingSpec.Id);
		return GetBindingFromSettings(secondarySettingsKey);
	}

	private CustomInputBinding GetBindingFromSettings(string settingsKey)
	{
		string text = _settings.GetString(settingsKey, string.Empty);
		if (!string.IsNullOrEmpty(text))
		{
			return _customInputBindingSerializer.Deserialize(text);
		}
		return null;
	}

	private static string GetPrimarySettingsKey(string keyBindingId)
	{
		return string.Format(PrimaryBindingFormat, keyBindingId);
	}

	private static string GetSecondarySettingsKey(string keyBindingId)
	{
		return string.Format(SecondaryBindingFormat, keyBindingId);
	}

	private KeyBindingSpec Overwrite(KeyBindingSpec keyBindingSpec)
	{
		ComponentSpec componentSpec = keyBindingSpec;
		PrimaryInputBindingSpec spec = componentSpec.GetSpec<PrimaryInputBindingSpec>();
		if ((object)spec != null)
		{
			componentSpec = _ctrlKeyOverwriter.OverwriteIfOnMacOS(spec);
		}
		SecondaryInputBindingSpec spec2 = componentSpec.GetSpec<SecondaryInputBindingSpec>();
		if ((object)spec2 != null)
		{
			componentSpec = _ctrlKeyOverwriter.OverwriteIfOnMacOS(spec2);
		}
		return componentSpec.GetSpec<KeyBindingSpec>();
	}
}
