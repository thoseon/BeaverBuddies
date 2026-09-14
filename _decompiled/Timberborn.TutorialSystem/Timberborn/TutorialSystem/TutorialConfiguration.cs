using System.Collections.Generic;
using System.Collections.Immutable;

namespace Timberborn.TutorialSystem;

public class TutorialConfiguration
{
	public string TutorialId { get; }

	public ImmutableArray<string> RequiredTutorialIds { get; }

	public string SkipIfTutorialFinished { get; }

	public string DisplayName { get; }

	public int SortOrder { get; }

	public ImmutableArray<TutorialStage> TutorialStages { get; }

	public bool KeepBlinking { get; }

	internal TutorialConfiguration(TutorialSpec tutorialSpec, IEnumerable<TutorialStage> tutorialStages)
	{
		TutorialId = tutorialSpec.Id;
		RequiredTutorialIds = tutorialSpec.RequiredTutorialIds;
		SkipIfTutorialFinished = tutorialSpec.SkipIfTutorialFinished;
		DisplayName = tutorialSpec.DisplayName.Value;
		SortOrder = tutorialSpec.SortOrder;
		TutorialStages = tutorialStages.ToImmutableArray();
		KeepBlinking = tutorialSpec.HasSpec<BlinkingTutorialSpec>();
	}
}
