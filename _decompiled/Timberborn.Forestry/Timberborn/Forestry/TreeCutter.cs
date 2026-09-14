using System;
using Timberborn.BaseComponentSystem;
using Timberborn.Characters;
using Timberborn.EntitySystem;
using Timberborn.ReservableSystem;
using Timberborn.SingletonSystem;
using Timberborn.Yielding;

namespace Timberborn.Forestry;

public class TreeCutter : BaseComponent, IInitializableEntity
{
	private readonly EventBus _eventBus;

	private YielderRemover _yielderRemover;

	private RemoveYieldExecutor _removeYieldExecutor;

	private TreeRemoveYieldStrategy _treeRemoveYieldStrategy;

	private bool _cuttingStarted;

	public event EventHandler CuttingStarted;

	public event EventHandler CuttingStopped;

	public TreeCutter(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public void InitializeEntity()
	{
		_yielderRemover = GetComponent<YielderRemover>();
		_removeYieldExecutor = GetComponent<RemoveYieldExecutor>();
		_removeYieldExecutor.WorkStarted += OnWorkStarted;
		_removeYieldExecutor.WorkFinished += OnWorkFinished;
		GetComponent<Character>().Died += delegate
		{
			FinishIfCutting();
		};
	}

	private void OnWorkStarted(object sender, EventArgs e)
	{
		if ((bool)_yielderRemover.ReservedYielder && _yielderRemover.ReservedYielder.RemoveYieldStrategy is TreeRemoveYieldStrategy treeRemoveYieldStrategy)
		{
			if (_removeYieldExecutor.IsLoadedLegacyExecutor())
			{
				treeRemoveYieldStrategy.Reacher.NotifyReservableReached(this);
			}
			CuttingStarted?.Invoke(this, EventArgs.Empty);
			_treeRemoveYieldStrategy = treeRemoveYieldStrategy;
			treeRemoveYieldStrategy.StartCutting(this);
			_cuttingStarted = true;
		}
	}

	private void OnWorkFinished(object sender, WorkFinishedEventArgs e)
	{
		if (e.WasCompleted)
		{
			_eventBus.Post(new TreeCutEvent());
		}
		FinishIfCutting();
	}

	private void FinishIfCutting()
	{
		if (_cuttingStarted)
		{
			if ((bool)_treeRemoveYieldStrategy)
			{
				_treeRemoveYieldStrategy.StopCutting();
				_treeRemoveYieldStrategy = null;
			}
			CuttingStopped?.Invoke(this, EventArgs.Empty);
			_cuttingStarted = false;
		}
	}
}
