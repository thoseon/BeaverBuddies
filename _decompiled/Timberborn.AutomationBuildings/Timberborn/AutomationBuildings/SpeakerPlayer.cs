using System.Collections.Generic;
using Timberborn.Automation;

namespace Timberborn.AutomationBuildings;

internal class SpeakerPlayer : ISamplingSingleton
{
	private readonly HashSet<Speaker> _speakers = new HashSet<Speaker>();

	public void AddSpeaker(Speaker speaker)
	{
		_speakers.Add(speaker);
	}

	public void RemoveSpeaker(Speaker speaker)
	{
		_speakers.Remove(speaker);
	}

	public void Sample()
	{
		foreach (Speaker speaker in _speakers)
		{
			speaker.PlayIfRequested();
		}
	}
}
