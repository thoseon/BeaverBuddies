using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.TutorialSystem;

internal class TutorialStageService : ILoadableSingleton
{
	private readonly ISpecService _specService;

	private readonly ImmutableArray<IStepDeserializer> _stepDeserializers;

	private readonly Dictionary<string, TutorialStageSpec> _stages = new Dictionary<string, TutorialStageSpec>();

	public TutorialStageService(ISpecService specService, IEnumerable<IStepDeserializer> stepDeserializers)
	{
		_specService = specService;
		_stepDeserializers = stepDeserializers.ToImmutableArray();
	}

	public void Load()
	{
		foreach (TutorialStageSpec spec in _specService.GetSpecs<TutorialStageSpec>())
		{
			_stages.Add(spec.Id, spec);
		}
	}

	public TutorialStage GetStage(string stageId)
	{
		TutorialStageSpec tutorialStageSpec = _stages[stageId];
		return new TutorialStage(tutorialStageSpec.Id, tutorialStageSpec.Intro.Value, GetSteps(tutorialStageSpec.Blueprint.Children));
	}

	private IEnumerable<TutorialStep> GetSteps(ImmutableArray<Blueprint> steps)
	{
		foreach (Blueprint item in steps)
		{
			foreach (IStepDeserializer stepDeserializer in _stepDeserializers)
			{
				if (stepDeserializer.TryDeserialize(item, out var tutorialStep))
				{
					yield return tutorialStep;
					break;
				}
			}
		}
	}
}
