using Timberborn.WorkerTypes;

namespace Timberborn.WorkerTypesUI;

public class WorkerTypeHelper
{
	public static readonly string BeaverWorkerType = "Beaver";

	public static readonly string BotWorkerType = "Bot";

	private readonly WorkerTypeService _workerTypeService;

	public WorkerTypeHelper(WorkerTypeService workerTypeService)
	{
		_workerTypeService = workerTypeService;
	}

	public string GetDisallowedWorkerText(string workerType)
	{
		return _workerTypeService.GetWorkerTypeSpec(workerType).WorkerOnlyText.Value;
	}

	public bool IsBeaverWorkerType(string workerType)
	{
		return workerType == BeaverWorkerType;
	}

	public bool IsBotWorkerType(string workerType)
	{
		return workerType == BotWorkerType;
	}

	public string GetBeaverWorkerTypeDisplayText()
	{
		return GetDisplayText(BeaverWorkerType);
	}

	public string GetBotWorkerTypeDisplayText()
	{
		return GetDisplayText(BotWorkerType);
	}

	private string GetDisplayText(string workerType)
	{
		return _workerTypeService.GetWorkerTypeSpec(workerType).DisplayName.Value;
	}
}
