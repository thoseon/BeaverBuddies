using System.Collections.Generic;
using System.Linq;
using Timberborn.Common;
using Timberborn.DropdownSystem;

namespace Timberborn.MainMenuModdingUI;

public class ModTemplateDropdownProvider : IDropdownProvider
{
	private readonly struct ModTemplate
	{
		public string Name { get; }

		public string Directory { get; }

		public ModTemplate(string name, string directory)
		{
			Name = name;
			Directory = directory;
		}
	}

	private static readonly List<ModTemplate> ModTemplates = new List<ModTemplate>
	{
		new ModTemplate("Example building", "BerryJam"),
		new ModTemplate("Translation", "Empty"),
		new ModTemplate("Tails and banners", "TailsAndBanners"),
		new ModTemplate("Empty", "Empty")
	};

	private int _selectedIndex;

	public IReadOnlyList<string> Items { get; } = ModTemplates.Select((ModTemplate template) => template.Name).ToList();

	public bool LocalizationTemplateChosen => _selectedIndex == 1;

	public string GetValue()
	{
		return Items[_selectedIndex];
	}

	public void SetValue(string value)
	{
		int num = Items.IndexOf(value);
		if (num >= 0)
		{
			_selectedIndex = num;
		}
	}

	public string GetDirectory()
	{
		return ModTemplates[_selectedIndex].Directory;
	}
}
