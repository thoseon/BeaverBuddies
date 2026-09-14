using Timberborn.BlockSystem;

namespace Timberborn.RecoverableGoodSystemUI;

public interface IRecoverableObjectAdder
{
	BlockObject GetAdditionalObjectToRecover();
}
