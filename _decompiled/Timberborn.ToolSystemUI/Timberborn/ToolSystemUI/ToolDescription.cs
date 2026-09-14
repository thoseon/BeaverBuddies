using System;
using System.Collections.Generic;
using Timberborn.Common;
using UnityEngine.UIElements;

namespace Timberborn.ToolSystemUI;

public class ToolDescription
{
	public class Builder
	{
		private readonly string _title;

		private readonly List<ToolDescriptionSection> _sections = new List<ToolDescriptionSection>();

		public Builder()
		{
		}

		public Builder(string title)
		{
			_title = title;
		}

		public Builder AddSection(string content)
		{
			_sections.Add(ToolDescriptionSection.CreateInternal(content));
			return this;
		}

		public Builder AddPrioritizedSection(string content)
		{
			_sections.Add(ToolDescriptionSection.CreateInternalPrioritized(content));
			return this;
		}

		public Builder AddExternalSection(VisualElement content)
		{
			_sections.Add(ToolDescriptionSection.CreateExternal(content));
			return this;
		}

		public Builder AddSection(VisualElement content)
		{
			_sections.Add(ToolDescriptionSection.CreateInternal(content));
			return this;
		}

		public Builder AddUpdatableSection(VisualElement content, Action updateCallback)
		{
			_sections.Add(ToolDescriptionSection.CreateInternalUpdatable(content, updateCallback));
			return this;
		}

		public ToolDescription Build()
		{
			return new ToolDescription(_title, _sections);
		}
	}

	private readonly List<ToolDescriptionSection> _sections;

	public string Title { get; }

	public ReadOnlyList<ToolDescriptionSection> Sections => _sections.AsReadOnlyList();

	public bool HasTitle => Title != null;

	private ToolDescription(string title, List<ToolDescriptionSection> sections)
	{
		Title = title;
		_sections = sections;
	}
}
