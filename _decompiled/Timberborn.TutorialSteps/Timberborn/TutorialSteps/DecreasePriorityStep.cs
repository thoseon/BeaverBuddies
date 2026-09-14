using Timberborn.PrioritySystem;
using Timberborn.SelectionSystem;
using Timberborn.TemplateSystem;
using Timberborn.TutorialSystem;
using Timberborn.WorkSystem;

namespace Timberborn.TutorialSteps;

internal class DecreasePriorityStep : ITutorialStep
{
	private readonly EntitySelectionService _entitySelectionService;

	private readonly string _description;

	private readonly string _templateName;

	private bool _wasAchieved;

	public DecreasePriorityStep(EntitySelectionService entitySelectionService, string description, string templateName)
	{
		_entitySelectionService = entitySelectionService;
		_description = description;
		_templateName = templateName;
	}

	public string Description()
	{
		return _description;
	}

	public bool Achieved()
	{
		if (_wasAchieved)
		{
			return true;
		}
		SelectableObject selectedObject = _entitySelectionService.SelectedObject;
		if ((bool)selectedObject)
		{
			WorkplacePriority component = selectedObject.GetComponent<WorkplacePriority>();
			if (component != null && component.GetComponent<TemplateSpec>().IsNamedExactly(_templateName))
			{
				_wasAchieved = component.Priority < Priority.Normal;
				return _wasAchieved;
			}
		}
		return false;
	}
}
