using Timberborn.WorldSerialization;

namespace Timberborn.WorldPersistence;

public interface ISerializedWorldSupplier
{
	SerializedWorld Get();
}
