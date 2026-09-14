using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.LinkedBuildingSystem;
using Timberborn.RecoverableGoodSystemUI;

namespace Timberborn.LinkedBuildingSystemUI;

internal class LinkedBuildingRecoverableObjectAdder : BaseComponent, IAwakableComponent, IRecoverableObjectAdder
{
	private BlockObject _linkedBlockObject;

	public void Awake()
	{
		GetComponent<LinkedBuilding>().BuildingLinked += OnBuildingLinked;
	}

	public BlockObject GetAdditionalObjectToRecover()
	{
		return _linkedBlockObject;
	}

	private void OnBuildingLinked(object sender, LinkedBuilding e)
	{
		_linkedBlockObject = e.GetComponent<BlockObject>();
	}
}
