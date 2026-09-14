using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.MapStateSystem;
using Timberborn.SoilContaminationSystem;
using Timberborn.SoilMoistureSystem;
using Timberborn.TimeSystem;

namespace Timberborn.Ruins;

internal class RuinModelUpdater : BaseComponent, IAwakableComponent, IInitializablePreview, IPostInitializableEntity, IDeletableEntity
{
	private readonly ITimeTriggerFactory _timeTriggerFactory;

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private readonly MapEditorMode _mapEditorMode;

	private readonly RuinModelFactory _ruinModelFactory;

	private Ruin _ruin;

	private RuinModels _ruinModels;

	private DryObject _dryObject;

	private ContaminatedObject _contaminatedObject;

	private ITimeTrigger _timeTrigger;

	public RuinModelUpdater(ITimeTriggerFactory timeTriggerFactory, IRandomNumberGenerator randomNumberGenerator, MapEditorMode mapEditorMode, RuinModelFactory ruinModelFactory)
	{
		_timeTriggerFactory = timeTriggerFactory;
		_randomNumberGenerator = randomNumberGenerator;
		_mapEditorMode = mapEditorMode;
		_ruinModelFactory = ruinModelFactory;
	}

	public void Awake()
	{
		_ruin = GetComponent<Ruin>();
		_ruinModels = GetComponent<RuinModels>();
		_dryObject = GetComponent<DryObject>();
		_contaminatedObject = GetComponent<ContaminatedObject>();
	}

	public void InitializePreview()
	{
		CreateModels();
		_ruinModels.ShowWetModel();
	}

	public void PostInitializeEntity()
	{
		CreateModels();
		_dryObject.EnteredDryState += delegate
		{
			ChangeDryState();
		};
		_dryObject.ExitedDryState += delegate
		{
			ChangeDryState();
		};
		_contaminatedObject.EnteredContaminatedState += delegate
		{
			ChangeDryState();
		};
		_contaminatedObject.ExitedContaminatedState += delegate
		{
			ChangeDryState();
		};
		UpdateModel();
	}

	public void DeleteEntity()
	{
		_timeTrigger?.Pause();
	}

	private void ChangeDryState()
	{
		if (_mapEditorMode.IsMapEditor)
		{
			UpdateModel();
			return;
		}
		float delayInDays = _randomNumberGenerator.Range(0f, 2f) / 24f;
		_timeTrigger?.Pause();
		_timeTrigger = _timeTriggerFactory.Create(UpdateModel, delayInDays);
		_timeTrigger.Resume();
	}

	private void UpdateModel()
	{
		if (_dryObject.IsDry || _contaminatedObject.IsContaminated)
		{
			_ruinModels.ShowDryModel();
		}
		else
		{
			_ruinModels.ShowWetModel();
		}
	}

	private void CreateModels()
	{
		if (!_ruinModels.IsInitialized)
		{
			string variantId = (TryGetComponent<RuinInit>(out var component) ? component.VariantId : _ruinModels.VariantId);
			_ruinModelFactory.CreateModels(variantId, _ruin);
		}
	}
}
