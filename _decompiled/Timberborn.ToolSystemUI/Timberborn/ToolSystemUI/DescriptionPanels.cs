using System.Collections.Generic;
using Timberborn.CoreUI;
using UnityEngine.UIElements;

namespace Timberborn.ToolSystemUI;

internal class DescriptionPanels
{
	private static readonly string BackgroundClass = "bg-sub-box--blue";

	private static readonly string PrioritizedClass = "description-panel-section--prioritized";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly Dictionary<IToolDescriptor, DescriptionPanel> _descriptionPanels = new Dictionary<IToolDescriptor, DescriptionPanel>();

	public DescriptionPanels(VisualElementLoader visualElementLoader)
	{
		_visualElementLoader = visualElementLoader;
	}

	public DescriptionPanel GetDescriptionPanel(IToolDescriptor toolDescriptor)
	{
		if (!_descriptionPanels.TryGetValue(toolDescriptor, out var value))
		{
			value = CreateDescriptionPanel(toolDescriptor);
			_descriptionPanels.Add(toolDescriptor, value);
		}
		return value;
	}

	private DescriptionPanel CreateDescriptionPanel(IToolDescriptor toolDescriptor)
	{
		ToolDescription toolDescription = toolDescriptor.DescribeTool();
		VisualElement root = _visualElementLoader.LoadVisualElement("Common/ToolPanel/DescriptionPanel");
		DescriptionPanel descriptionPanel = new DescriptionPanel(root);
		SetBasicInfo(toolDescription, root);
		AddSections(toolDescription, descriptionPanel);
		return descriptionPanel;
	}

	private static void SetBasicInfo(ToolDescription toolDescription, VisualElement root)
	{
		Label label = root.Q<Label>("Title");
		if (toolDescription.HasTitle)
		{
			label.text = toolDescription.Title;
		}
		label.parent.ToggleDisplayStyle(toolDescription.HasTitle);
	}

	private void AddSections(ToolDescription toolDescription, DescriptionPanel panel)
	{
		VisualElement root = panel.Root;
		VisualElement internalSections = root.Q<VisualElement>("InternalSections");
		VisualElement externalSections = root.Q<VisualElement>("ExternalSections");
		foreach (ToolDescriptionSection section in toolDescription.Sections)
		{
			AddSection(section, internalSections, externalSections);
			if (section.UpdateCallback != null)
			{
				panel.AddUpdateCallback(section.UpdateCallback);
			}
		}
		panel.AddUpdateCallback(delegate
		{
			internalSections.ToggleDisplayStyle(internalSections.childCount > 0);
		});
	}

	private void AddSection(ToolDescriptionSection toolDescriptionSection, VisualElement internalSections, VisualElement externalSections)
	{
		VisualElement sectionRoot = GetSectionRoot(toolDescriptionSection);
		if (toolDescriptionSection.Prioritized)
		{
			sectionRoot.AddToClassList(PrioritizedClass);
			sectionRoot.AddToClassList(BackgroundClass);
		}
		if (toolDescriptionSection.Section != null)
		{
			if (toolDescriptionSection.External)
			{
				externalSections.Add(sectionRoot);
			}
			else
			{
				internalSections.Add(toolDescriptionSection.Section);
			}
		}
		else if (!string.IsNullOrEmpty(toolDescriptionSection.Content))
		{
			sectionRoot.Q<Label>("SectionText").text = toolDescriptionSection.Content;
			sectionRoot.AddToClassList(BackgroundClass);
			internalSections.Add(sectionRoot);
		}
	}

	private VisualElement GetSectionRoot(ToolDescriptionSection toolDescriptionSection)
	{
		if (toolDescriptionSection.External)
		{
			return toolDescriptionSection.Section;
		}
		string elementName = "Common/ToolPanel/DescriptionPanelSection";
		return _visualElementLoader.LoadVisualElement(elementName);
	}
}
