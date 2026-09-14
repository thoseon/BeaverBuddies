using UnityEngine;

namespace Timberborn.Navigation;

internal readonly struct PathRequest
{
	public Vector3 Start { get; }

	public Vector3 Destination { get; }

	public bool Reversed { get; }

	private PathRequest(Vector3 start, Vector3 destination, bool reversed)
	{
		Start = start;
		Destination = destination;
		Reversed = reversed;
	}

	public static PathRequest Create(Vector3 start, Vector3 destination)
	{
		return new PathRequest(start, destination, reversed: false);
	}

	public static PathRequest CreateReversed(Vector3 start, Vector3 destination)
	{
		return new PathRequest(start, destination, reversed: true);
	}
}
