using UnityEngine.UIElements;

namespace Timberborn.EntityPanelSystem;

public class EntityDescription
{
	public string Content { get; }

	public VisualElement Section { get; }

	public int Order { get; }

	public bool FlavorSection { get; }

	public bool Input { get; }

	public bool Output { get; }

	public string Time { get; }

	public bool BottomSection { get; }

	public bool TextSection
	{
		get
		{
			if (Content != null)
			{
				return !FlavorSection;
			}
			return false;
		}
	}

	public bool MiddleSection
	{
		get
		{
			if (Section != null && !ProductionSection)
			{
				return !BottomSection;
			}
			return false;
		}
	}

	public bool ProductionSection
	{
		get
		{
			if (!Input)
			{
				return Output;
			}
			return true;
		}
	}

	private EntityDescription(string content, VisualElement section, int order, bool flavorSection = false, bool input = false, bool output = false, string time = null, bool bottomSection = false)
	{
		Content = content;
		Section = section;
		Order = order;
		FlavorSection = flavorSection;
		Input = input;
		Output = output;
		Time = time;
		BottomSection = bottomSection;
	}

	public static EntityDescription CreateTextSection(string content, int order)
	{
		return new EntityDescription(content, null, order);
	}

	public static EntityDescription CreateFlavorSection(string content, int order)
	{
		return new EntityDescription(content, null, order, flavorSection: true);
	}

	public static EntityDescription CreateMiddleSection(VisualElement content, int order)
	{
		return new EntityDescription(null, content, order);
	}

	public static EntityDescription CreateBottomSection(VisualElement content, int order)
	{
		return new EntityDescription(null, content, order, flavorSection: false, input: false, output: false, null, bottomSection: true);
	}

	public static EntityDescription CreateInputOutputSection(VisualElement content, int order)
	{
		return new EntityDescription(null, content, order, flavorSection: false, input: true, output: true);
	}

	public static EntityDescription CreateInputSectionWithTime(VisualElement content, int order, string time)
	{
		return new EntityDescription(null, content, order, flavorSection: false, input: true, output: false, time);
	}

	public static EntityDescription CreateOutputSection(VisualElement content, int order)
	{
		return new EntityDescription(null, content, order, flavorSection: false, input: false, output: true);
	}
}
