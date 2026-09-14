using System;
using Timberborn.BaseComponentSystem;
using Timberborn.WorkSystem;

namespace Timberborn.WorkerOutfitSystem;

internal class WorkerOutfitChangeNotifier : BaseComponent, IAwakableComponent
{
	private readonly WorkerOutfitService _workerOutfitService;

	private Worker _worker;

	public event EventHandler<WorkerOutfitChangedEventArgs> OutfitChanged;

	public WorkerOutfitChangeNotifier(WorkerOutfitService workerOutfitService)
	{
		_workerOutfitService = workerOutfitService;
	}

	public void Awake()
	{
		_worker = GetComponent<Worker>();
		_worker.GotEmployed += OnGotEmployed;
		_worker.GotUnemployed += OnGotUnemployed;
	}

	private void OnGotEmployed(object sender, EventArgs e)
	{
		if (_workerOutfitService.TryGetOutfitSpec(_worker, out var workerOutfitSpec))
		{
			OutfitChanged?.Invoke(this, new WorkerOutfitChangedEventArgs(workerOutfitSpec));
		}
	}

	private void OnGotUnemployed(object sender, EventArgs e)
	{
		OutfitChanged?.Invoke(this, WorkerOutfitChangedEventArgs.None);
	}
}
