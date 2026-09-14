namespace Timberborn.TimbermeshAnimations;

internal interface IAnimationUpdater
{
	void Initialize();

	void SetAnimation(string animationName, bool looped);

	void UpdateAnimation(float normalizedTime);
}
