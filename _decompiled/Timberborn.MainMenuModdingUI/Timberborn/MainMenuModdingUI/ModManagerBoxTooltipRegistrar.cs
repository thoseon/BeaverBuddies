using System;
using Timberborn.Localization;
using Timberborn.ModdingUI;
using Timberborn.TooltipSystem;
using UnityEngine.UIElements;

namespace Timberborn.MainMenuModdingUI;

internal class ModManagerBoxTooltipRegistrar : IModManagerTooltipRegistrar
{
	private static readonly string DecreasePriorityLocKey = "Modding.DecreasePriority";

	private static readonly string IncreasePriorityLocKey = "Modding.IncreasePriority";

	private static readonly string InvalidGameVersionLocKey = "Modding.ModWarning.InvalidGameVersion";

	private static readonly string MissingRequiredModLocKey = "Modding.ModWarning.MissingRequiredMod";

	private static readonly string RequiredModInvalidOrderLocKey = "Modding.ModWarning.RequiredModInvalidOrder";

	private static readonly string RequiredModInvalidVersionLocKey = "Modding.ModWarning.RequiredModInvalidVersion";

	private static readonly string RequiredModNotEnabledLocKey = "Modding.ModWarning.RequiredModNotEnabled";

	private static readonly string CloudModLocKey = "Modding.CloudSource";

	private static readonly string LocalModLocKey = "Modding.LocalSource";

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly ILoc _loc;

	public ModManagerBoxTooltipRegistrar(ITooltipRegistrar tooltipRegistrar, ILoc loc)
	{
		_tooltipRegistrar = tooltipRegistrar;
		_loc = loc;
	}

	public void RegisterModWarning(VisualElement element, ModItem modItem)
	{
		_tooltipRegistrar.Register(element, () => GetWarningText(modItem));
	}

	public void RegisterModIcon(VisualElement element, ModItem modItem)
	{
		_tooltipRegistrar.Register(element, GetModSourceText(modItem));
	}

	public void RegisterIncreaseButton(VisualElement element)
	{
		_tooltipRegistrar.Register(element, _loc.T(IncreasePriorityLocKey));
	}

	public void RegisterDecreaseButton(VisualElement element)
	{
		_tooltipRegistrar.Register(element, _loc.T(DecreasePriorityLocKey));
	}

	private string GetWarningText(ModItem modItem)
	{
		return modItem.WarningReason switch
		{
			ModWarningReason.MissingRequiredMod => _loc.T(MissingRequiredModLocKey, modItem.WarningInfo), 
			ModWarningReason.RequiredModNotEnabled => _loc.T(RequiredModNotEnabledLocKey, modItem.WarningInfo), 
			ModWarningReason.RequiredModInvalidVersion => _loc.T(RequiredModInvalidVersionLocKey, modItem.WarningInfo), 
			ModWarningReason.RequiredModInvalidOrder => _loc.T(RequiredModInvalidOrderLocKey, modItem.WarningInfo), 
			ModWarningReason.InvalidGameVersion => _loc.T(InvalidGameVersionLocKey, modItem.WarningInfo), 
			ModWarningReason.None => throw new ArgumentException("GetWarningText called with None warning reason"), 
			_ => throw new ArgumentOutOfRangeException(string.Format("Unknown {0}: {1}", "ModWarningReason", modItem.WarningReason)), 
		};
	}

	private string GetModSourceText(ModItem modItem)
	{
		if (!modItem.Mod.ModDirectory.IsUserMod)
		{
			return _loc.T(CloudModLocKey, modItem.Mod.ModDirectory.DisplaySource);
		}
		return _loc.T(LocalModLocKey);
	}
}
