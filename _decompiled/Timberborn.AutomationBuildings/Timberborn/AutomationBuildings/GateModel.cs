using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Illumination;
using UnityEngine;

namespace Timberborn.AutomationBuildings;

internal class GateModel : BaseComponent, IAwakableComponent, IInitializablePreview, IUnfinishedStateListener, IFinishedStateListener
{
	private BlockObject _blockObject;

	private GateNavMeshBlocker _gateNavMeshBlocker;

	private GameObject _openModel;

	private GameObject _closedModel;

	private IlluminatorToggle _illuminatorToggle;

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_gateNavMeshBlocker = GetComponent<GateNavMeshBlocker>();
		GetComponent<Gate>().StateChanged += delegate
		{
			UpdateModels();
		};
		_openModel = base.GameObject.FindChild(GetComponent<GateModelSpec>().OpenModelName);
		_closedModel = base.GameObject.FindChild(GetComponent<GateModelSpec>().ClosedModelName);
		_illuminatorToggle = GetComponent<Illuminator>().CreateToggle();
	}

	public void InitializePreview()
	{
		UpdateModels();
	}

	public void OnEnterUnfinishedState()
	{
		UpdateModels();
	}

	public void OnExitUnfinishedState()
	{
	}

	public void OnEnterFinishedState()
	{
		UpdateModels();
	}

	public void OnExitFinishedState()
	{
	}

	private void UpdateModels()
	{
		bool navMeshBlocked = _gateNavMeshBlocker.NavMeshBlocked;
		_openModel.SetActive(!navMeshBlocked);
		_closedModel.SetActive(navMeshBlocked);
		_illuminatorToggle.Toggle(!navMeshBlocked && _blockObject.IsFinished);
	}
}
