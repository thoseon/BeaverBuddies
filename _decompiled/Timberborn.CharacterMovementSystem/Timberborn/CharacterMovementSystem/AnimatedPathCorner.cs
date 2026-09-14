using UnityEngine;

namespace Timberborn.CharacterMovementSystem;

public readonly struct AnimatedPathCorner
{
	public Vector3 Position { get; }

	public float Time { get; }

	public float Speed { get; }

	public float DistanceToPathCorner { get; }

	public int GroupId { get; }

	public AnimatedPathCorner(Vector3 position, float time, float speed, float distanceToPathCorner, int groupId)
	{
		Position = position;
		Time = time;
		Speed = speed;
		DistanceToPathCorner = distanceToPathCorner;
		GroupId = groupId;
	}
}
