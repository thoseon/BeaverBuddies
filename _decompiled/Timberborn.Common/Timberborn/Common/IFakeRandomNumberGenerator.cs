namespace Timberborn.Common;

public interface IFakeRandomNumberGenerator
{
	float Range(float inclusiveMin, float inclusiveMax, int byteIndex);

	byte Byte(int byteIndex);
}
