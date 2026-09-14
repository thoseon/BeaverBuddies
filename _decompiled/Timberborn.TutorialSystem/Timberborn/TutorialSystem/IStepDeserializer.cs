using Timberborn.BlueprintSystem;

namespace Timberborn.TutorialSystem;

public interface IStepDeserializer
{
	bool TryDeserialize(Blueprint step, out TutorialStep tutorialStep);
}
