using System;

namespace Timberborn.TimbermeshAnimations;

public interface IAnimator
{
	bool PlayBackwards { set; }

	bool Enabled { get; set; }

	float Speed { set; }

	float Time { get; }

	float RepeatedTime { get; }

	string AnimationName { get; }

	float AnimationLength { get; }

	bool PlayingFinished { get; }

	event EventHandler AnimationChanged;

	void Play(string animationName, bool looped = true);

	void Stop();

	void SetTime(float time);
}
