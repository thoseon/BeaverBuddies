using UnityEngine;

namespace Timberborn.Characters;

public record CharacterInit(int DayOfBirth, float LifeProgress, Character Child, Vector3 Position, Quaternion Rotation);
