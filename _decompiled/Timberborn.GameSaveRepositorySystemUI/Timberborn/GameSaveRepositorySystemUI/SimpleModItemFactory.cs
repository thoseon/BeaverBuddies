using Timberborn.CoreUI;
using Timberborn.Modding;
using Timberborn.SaveMetadataSystem;
using Timberborn.TooltipSystem;
using Timberborn.Versioning;
using UnityEngine.UIElements;

namespace Timberborn.GameSaveRepositorySystemUI;

public class SimpleModItemFactory
{
	private static readonly string WarningIconClass = "warning-icon";

	private static readonly string ErrorIconClass = "error-icon";

	private static readonly string MissingModLocKey = "Modding.MissingMod";

	private static readonly string VersionMismatchLocKey = "Modding.VersionMismatch";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ModRepository _modRepository;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	public SimpleModItemFactory(VisualElementLoader visualElementLoader, ModRepository modRepository, ITooltipRegistrar tooltipRegistrar)
	{
		_visualElementLoader = visualElementLoader;
		_modRepository = modRepository;
		_tooltipRegistrar = tooltipRegistrar;
	}

	public void FillActiveMods(VisualElement container)
	{
		foreach (Mod enabledMod in _modRepository.EnabledMods)
		{
			VisualElement child = CreateModItem(enabledMod.Manifest.Name, enabledMod.Manifest.Version.Formatted);
			container.Add(child);
		}
	}

	public void FillSavedMods(VisualElement container, SaveMetadata metadata)
	{
		ModReference[] mods = metadata.Mods;
		for (int i = 0; i < mods.Length; i++)
		{
			ModReference modReference = mods[i];
			VisualElement visualElement = CreateModItem(modReference.Name, Version.Create(modReference.Version).Formatted);
			if (_modRepository.ModIsNotEnabled(modReference.Id))
			{
				SetErrorIcon(visualElement);
			}
			else if (_modRepository.ModIsOnDifferentVersion(modReference.Id, modReference.Version))
			{
				SetWarningIcon(visualElement);
			}
			container.Add(visualElement);
		}
	}

	private VisualElement CreateModItem(string modName, string modVersion)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Modding/SimpleModItem");
		visualElement.Q<Label>("ModName").text = modName;
		visualElement.Q<Label>("ModVersion").text = modVersion;
		return visualElement;
	}

	private void SetErrorIcon(VisualElement modItem)
	{
		VisualElement incompatibilityIcon = GetIncompatibilityIcon(modItem);
		incompatibilityIcon.AddToClassList(ErrorIconClass);
		_tooltipRegistrar.RegisterLocalizable(incompatibilityIcon, MissingModLocKey);
	}

	private void SetWarningIcon(VisualElement modItem)
	{
		VisualElement incompatibilityIcon = GetIncompatibilityIcon(modItem);
		incompatibilityIcon.AddToClassList(WarningIconClass);
		_tooltipRegistrar.RegisterLocalizable(incompatibilityIcon, VersionMismatchLocKey);
	}

	private static VisualElement GetIncompatibilityIcon(VisualElement modItem)
	{
		return modItem.Q<VisualElement>("IncompatibilityIcon");
	}
}
