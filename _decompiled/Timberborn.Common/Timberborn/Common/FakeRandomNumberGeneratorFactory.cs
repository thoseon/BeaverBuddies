using System;
using System.Runtime.InteropServices;

namespace Timberborn.Common;

internal class FakeRandomNumberGeneratorFactory : IFakeRandomNumberGeneratorFactory
{
	public IFakeRandomNumberGenerator Create(Guid guid, int salt)
	{
		Span<byte> span = stackalloc byte[16];
		if (guid.TryWriteBytes(span))
		{
			Span<int> span2 = MemoryMarshal.Cast<byte, int>(span);
			return new FakeRandomNumberGenerator(span2[0] ^ span2[1] ^ span2[2] ^ span2[3] ^ salt);
		}
		Guid guid2 = guid;
		throw new InvalidOperationException("Failed to write bytes from Guid: " + guid2.ToString());
	}
}
