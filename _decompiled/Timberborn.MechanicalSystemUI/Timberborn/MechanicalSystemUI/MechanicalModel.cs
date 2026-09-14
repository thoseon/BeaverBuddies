using System;
using Timberborn.BaseComponentSystem;
using Timberborn.MechanicalSystem;

namespace Timberborn.MechanicalSystemUI;

internal class MechanicalModel : BaseComponent, IAwakableComponent
{
	private MechanicalNode _mechanicalNode;

	private IMechanicalModelUpdater _mechanicalModelUpdater;

	public void Awake()
	{
		_mechanicalNode = GetComponent<MechanicalNode>();
		_mechanicalModelUpdater = GetComponent<IMechanicalModelUpdater>();
		_mechanicalNode.AddedToGraph += OnAddedToGraph;
	}

	public void UpdateModel()
	{
		_mechanicalModelUpdater?.UpdateModel();
	}

	private void OnAddedToGraph(object sender, EventArgs eventArgs)
	{
		if (_mechanicalNode.Powered)
		{
			UpdateModel();
		}
	}
}
