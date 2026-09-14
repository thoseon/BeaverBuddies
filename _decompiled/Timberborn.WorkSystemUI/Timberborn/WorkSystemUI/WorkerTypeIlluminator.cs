using Timberborn.BaseComponentSystem;
using Timberborn.Bots;
using Timberborn.EntitySystem;
using Timberborn.Illumination;
using Timberborn.WorkSystem;
using Timberborn.WorkerTypesUI;

namespace Timberborn.WorkSystemUI;

public class WorkerTypeIlluminator : BaseComponent, IAwakableComponent, IInitializableEntity
{
	private readonly WorkerTypeHelper _workerTypeHelper;

	private readonly BotColors _botColors;

	private Illuminator _illuminator;

	private WorkplaceWorkerType _workplaceWorkerType;

	private bool _lightingEnabled;

	private IlluminatorColorizer _illuminatorColorizer;

	public WorkerTypeIlluminator(WorkerTypeHelper workerTypeHelper, BotColors botColors)
	{
		_workerTypeHelper = workerTypeHelper;
		_botColors = botColors;
	}

	public void Awake()
	{
		_illuminator = GetComponent<Illuminator>();
		_workplaceWorkerType = GetComponent<WorkplaceWorkerType>();
		_workplaceWorkerType.WorkerTypeChanged += OnWorkerTypeChanged;
		_illuminatorColorizer = _illuminator.CreateColorizer(20);
	}

	public void InitializeEntity()
	{
		UpdateIlluminator();
	}

	private void UpdateIlluminator()
	{
		if (_workerTypeHelper.IsBotWorkerType(_workplaceWorkerType.WorkerType))
		{
			_illuminatorColorizer.SetColor(_botColors.BotIlluminationColor);
		}
		else
		{
			_illuminatorColorizer.ClearColor();
		}
	}

	private void OnWorkerTypeChanged(object sender, WorkerTypeChangedEventArgs e)
	{
		UpdateIlluminator();
	}
}
