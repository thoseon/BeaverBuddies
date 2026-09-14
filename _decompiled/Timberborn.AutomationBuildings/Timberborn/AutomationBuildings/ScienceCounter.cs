using Timberborn.Automation;
using Timberborn.BaseComponentSystem;
using Timberborn.DuplicationSystem;
using Timberborn.Persistence;
using Timberborn.ScienceSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.AutomationBuildings;

public class ScienceCounter : BaseComponent, IAwakableComponent, IPersistentEntity, IDuplicable<ScienceCounter>, IDuplicable, ISamplingTransmitter, ITransmitter
{
	private static readonly ComponentKey ScienceCounterKey = new ComponentKey("ScienceCounter");

	private static readonly PropertyKey<int> ThresholdKey = new PropertyKey<int>("Threshold");

	private static readonly PropertyKey<NumericComparisonMode> ModeKey = new PropertyKey<NumericComparisonMode>("Mode");

	private readonly ScienceService _scienceService;

	private Automator _automator;

	public int SampledSciencePoints { get; private set; }

	public int Threshold { get; private set; }

	public NumericComparisonMode Mode { get; private set; }

	public ScienceCounter(ScienceService scienceService)
	{
		_scienceService = scienceService;
	}

	public void Awake()
	{
		_automator = GetComponent<Automator>();
	}

	public void Save(IEntitySaver entitySaver)
	{
		IObjectSaver component = entitySaver.GetComponent(ScienceCounterKey);
		component.Set(ThresholdKey, Threshold);
		component.Set(ModeKey, Mode);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(ScienceCounterKey);
		Threshold = component.Get(ThresholdKey);
		Mode = component.Get(ModeKey);
	}

	public void DuplicateFrom(ScienceCounter source)
	{
		Threshold = source.Threshold;
		Mode = source.Mode;
		UpdateOutputState();
	}

	public void SetThreshold(int threshold)
	{
		Threshold = threshold;
		UpdateOutputState();
	}

	public void SetMode(NumericComparisonMode mode)
	{
		Mode = mode;
		UpdateOutputState();
	}

	public void Sample()
	{
		SampledSciencePoints = _scienceService.SciencePoints;
		UpdateOutputState();
	}

	private void UpdateOutputState()
	{
		_automator.SetState(Mode.Evaluate(SampledSciencePoints, Threshold));
	}
}
