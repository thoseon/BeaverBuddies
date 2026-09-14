namespace Timberborn.Coordinates;

public static class Directions3DExtensions
{
	public static Direction3DEnumerator GetEnumerator(this Directions3D directions)
	{
		return new Direction3DEnumerator(directions);
	}
}
