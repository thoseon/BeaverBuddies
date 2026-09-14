using Timberborn.SelectionSystem;
using Timberborn.TemplateSystem;
using Timberborn.TutorialSystem;
using Timberborn.WorkSystem;

namespace Timberborn.TutorialSteps;

internal class IncreaseDesiredWorkersStep : ITutorialStep
{
	private readonly EntitySelectionService _entitySelectionService;

	private readonly string _description;

	private readonly string _templateName;

	private bool _wasAchieved;

	public IncreaseDesiredWorkersStep(EntitySelectionService entitySelectionService, string description, string templateName)
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
			Workplace component = selectedObject.GetComponent<Workplace>();
			if (component != null && component.GetComponent<TemplateSpec>().IsNamedExactly(_templateName))
			{
				_wasAchieved = component.DesiredWorkers > component.GetComponent<WorkplaceSpec>().DefaultWorkers;
				return _wasAchieved;
			}
		}
		return false;
	}
}
