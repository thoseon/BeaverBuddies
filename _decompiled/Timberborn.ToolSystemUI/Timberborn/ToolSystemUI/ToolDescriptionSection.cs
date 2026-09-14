using System;
using UnityEngine.UIElements;

namespace Timberborn.ToolSystemUI;

public class ToolDescriptionSection
{
	public string Content { get; }

	public VisualElement Section { get; }

	public bool Prioritized { get; }

	public bool External { get; }

	public Action UpdateCallback { get; }

	private ToolDescriptionSection(string content, VisualElement section, bool external = false, bool prioritized = false, Action updateCallback = null)
	{
		Content = content;
		Section = section;
		External = external;
		Prioritized = prioritized;
		UpdateCallback = updateCallback;
	}

	public static ToolDescriptionSection CreateInternal(string content)
	{
		return new ToolDescriptionSection(content, null);
	}

	public static ToolDescriptionSection CreateInternal(VisualElement content)
	{
		return new ToolDescriptionSection("", content);
	}

	public static ToolDescriptionSection CreateInternalUpdatable(VisualElement content, Action updateCallback)
	{
		return new ToolDescriptionSection("", content, external: false, prioritized: false, updateCallback);
	}

	public static ToolDescriptionSection CreateInternalPrioritized(string content)
	{
		return new ToolDescriptionSection(content, null, external: false, prioritized: true);
	}

	public static ToolDescriptionSection CreateExternal(VisualElement content)
	{
		return new ToolDescriptionSection("", content, external: true);
	}
}
