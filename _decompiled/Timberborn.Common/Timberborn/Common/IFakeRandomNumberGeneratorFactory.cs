using System;

namespace Timberborn.Common;

public interface IFakeRandomNumberGeneratorFactory
{
	IFakeRandomNumberGenerator Create(Guid guid, int salt);
}
