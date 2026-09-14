using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Buildings;
using Timberborn.EntitySystem;
using Timberborn.Rendering;

namespace Timberborn.ConstructionMode;

internal class ConstructionModeModel : BaseComponent, IAwakableComponent, IUnfinishedStateListener, IRegisteredComponent
{
	private readonly MaterialColorer _materialColorer;

	private BuildingModel _buildingModel;

	public ConstructionModeModel(MaterialColorer materialColorer)
	{
		_materialColorer = materialColorer;
	}

	public void Awake()
	{
		_buildingModel = GetComponent<BuildingModel>();
		DisableComponent();
	}

	public void OnEnterUnfinishedState()
	{
		_materialColorer.EnableGrayscale(_buildingModel.FinishedModel);
		if ((bool)_buildingModel.FinishedUncoveredModel)
		{
			_materialColorer.EnableGrayscale(_buildingModel.FinishedUncoveredModel);
		}
		EnableComponent();
	}

	public void OnExitUnfinishedState()
	{
		_materialColorer.DisableGrayscale(_buildingModel.FinishedModel);
		if ((bool)_buildingModel.FinishedUncoveredModel)
		{
			_materialColorer.DisableGrayscale(_buildingModel.FinishedUncoveredModel);
		}
		DisableComponent();
	}

	public void EnterConstructionMode()
	{
		_buildingModel.ShowFinishedModel();
	}

	public void ExitConstructionMode()
	{
		_buildingModel.ShowUnfinishedModel();
	}
}
