using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.Common;

namespace Timberborn.TutorialSystem;

public class TutorialStage
{
	public string Id { get; }

	public string Intro { get; }

	public ImmutableArray<TutorialStep> TutorialSteps { get; }

	public bool AllStepsAchieved => TutorialSteps.FastAll((TutorialStep tutorialStep) => tutorialStep.Step.Achieved());

	public bool HasSteps => TutorialSteps.Length > 0;

	public TutorialStage(string id, string intro, IEnumerable<TutorialStep> tutorialSteps)
	{
		Id = id;
		Intro = intro;
		TutorialSteps = tutorialSteps.ToImmutableArray();
	}
}
