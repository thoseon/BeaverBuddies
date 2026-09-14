using System.Collections.Generic;

namespace Timberborn.TimbermeshAnimations;

public interface IAnimatorController
{
	IEnumerable<string> AnimationNames { get; }

	bool HasParameter(string parameterName);

	void SetFloat(string parameterName, float value);

	void SetBool(string parameterName, bool state);

	void Enable();

	void Disable();
}
