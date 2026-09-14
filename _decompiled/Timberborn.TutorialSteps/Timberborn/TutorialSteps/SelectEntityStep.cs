using System.Collections.Immutable;
using Timberborn.SelectionSystem;
using Timberborn.TemplateSystem;
using Timberborn.TutorialSystem;

namespace Timberborn.TutorialSteps;

internal class SelectEntityStep : ITutorialStep
{
	private readonly EntitySelectionService _entitySelectionService;

	private readonly ImmutableArray<string> _templateNames;

	private readonly string _description;

	private bool _wasAchieved;

	public SelectEntityStep(EntitySelectionService entitySelectionService, ImmutableArray<string> templateNames, string description)
	{
		_entitySelectionService = entitySelectionService;
		_templateNames = templateNames;
		_description = description;
	}

	public string Description()
	{
		return _description;
	}

	public bool Achieved()
	{
		_wasAchieved |= IsTemplateSelected();
		return _wasAchieved;
	}

	private bool IsTemplateSelected()
	{
		if (_entitySelectionService.IsAnythingSelected)
		{
			string templateName = _entitySelectionService.SelectedObject.GetComponent<TemplateSpec>().TemplateName;
			return _templateNames.Contains(templateName);
		}
		return false;
	}
}
