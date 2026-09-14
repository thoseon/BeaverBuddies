using Timberborn.SteamWorkshopUI;

namespace Timberborn.SteamWorkshopModUploadingUI;

internal static class SteamWorkshopModTags
{
	public static readonly string[] MandatoryTags = new string[1] { "Mod" };

	private static readonly WorkshopTagCategory CompatibilityCategory = new WorkshopTagCategory("Compatibility", 0);

	private static readonly WorkshopTagCategory TypeCategory = new WorkshopTagCategory("Type", 10);

	private static readonly WorkshopTagCategory ContentCategory = new WorkshopTagCategory("Content", 20);

	public static readonly WorkshopTag[] AvailableTags = new WorkshopTag[19]
	{
		new WorkshopTag(CompatibilityCategory, "Update 1.1", -10),
		new WorkshopTag(CompatibilityCategory, "Update 1.0", 0),
		new WorkshopTag(CompatibilityCategory, "Update 0.7", 10),
		new WorkshopTag(CompatibilityCategory, "Update 0.6", 20),
		new WorkshopTag(TypeCategory, "New content", 0),
		new WorkshopTag(TypeCategory, "Quality of life", 10),
		new WorkshopTag(TypeCategory, "Balance", 20),
		new WorkshopTag(TypeCategory, "Cheats", 30),
		new WorkshopTag(TypeCategory, "Visuals", 40),
		new WorkshopTag(TypeCategory, "Modding tools", 50),
		new WorkshopTag(TypeCategory, "Other", 60),
		new WorkshopTag(ContentCategory, "Buildings", 0),
		new WorkshopTag(ContentCategory, "Plants", 10),
		new WorkshopTag(ContentCategory, "Goods", 20),
		new WorkshopTag(ContentCategory, "Factions", 30),
		new WorkshopTag(ContentCategory, "Outfits", 40),
		new WorkshopTag(ContentCategory, "Decals", 50),
		new WorkshopTag(ContentCategory, "Translations", 60),
		new WorkshopTag(ContentCategory, "Miscellaneous", 70)
	};
}
