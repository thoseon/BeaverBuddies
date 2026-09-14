using Timberborn.Buildings;
using Timberborn.SelectionSystem;
using Timberborn.TemplateSystem;
using Timberborn.TutorialSystem;

namespace Timberborn.TutorialSteps;

internal class ChangePausedStateStep : ITutorialStep
{
	private readonly EntitySelectionService _entitySelectionService;

	private readonly string _description;

	private readonly bool _shouldBePaused;

	private readonly string _templateName;

	private bool _wasOppositeState;

	private bool _wasAchieved;

	public ChangePausedStateStep(EntitySelectionService entitySelectionService, string description, bool shouldBePaused, string templateName)
	{
		_entitySelectionService = entitySelectionService;
		_description = description;
		_shouldBePaused = shouldBePaused;
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
			PausableBuilding component = selectedObject.GetComponent<PausableBuilding>();
			if (component != null && component.GetComponent<TemplateSpec>().IsNamedExactly(_templateName))
			{
				if (_wasOppositeState)
				{
					_wasAchieved = (_shouldBePaused && component.Paused) || (!_shouldBePaused && !component.Paused);
					return _wasAchieved;
				}
				_wasOppositeState = (_shouldBePaused && !component.Paused) || (!_shouldBePaused && component.Paused);
			}
		}
		return false;
	}
}
