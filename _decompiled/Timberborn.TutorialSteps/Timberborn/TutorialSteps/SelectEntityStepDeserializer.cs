using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using Timberborn.LocalizationSerialization;
using Timberborn.SelectionSystem;
using Timberborn.TutorialSystem;

namespace Timberborn.TutorialSteps;

internal class SelectEntityStepDeserializer : IStepDeserializer
{
	private readonly EntitySelectionService _entitySelectionService;

	public SelectEntityStepDeserializer(EntitySelectionService entitySelectionService)
	{
		_entitySelectionService = entitySelectionService;
	}

	public bool TryDeserialize(Blueprint step, out TutorialStep tutorialStep)
	{
		if (step.Specs[0] is SelectEntityStepSpec selectEntityStepSpec)
		{
			tutorialStep = Create(selectEntityStepSpec.TemplateNames, selectEntityStepSpec.Description);
			return true;
		}
		tutorialStep = null;
		return false;
	}

	private TutorialStep Create(ImmutableArray<string> templateNames, LocalizedText description)
	{
		return TutorialStep.Create(new SelectEntityStep(_entitySelectionService, templateNames, description.Value));
	}
}
