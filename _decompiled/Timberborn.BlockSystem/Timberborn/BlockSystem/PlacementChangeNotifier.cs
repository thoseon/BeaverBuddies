using System.Collections.Generic;
using Timberborn.BaseComponentSystem;

namespace Timberborn.BlockSystem;

public class PlacementChangeNotifier : BaseComponent, IAwakableComponent
{
	private readonly List<IPrePlacementChangeListener> _prePlacementChangeListeners = new List<IPrePlacementChangeListener>();

	private readonly List<IPostPlacementChangeListener> _postPlacementChangeListeners = new List<IPostPlacementChangeListener>();

	public void Awake()
	{
		GetComponents(_prePlacementChangeListeners);
		GetComponents(_postPlacementChangeListeners);
	}

	public void NotifyPreChangeListeners()
	{
		for (int i = 0; i < _prePlacementChangeListeners.Count; i++)
		{
			_prePlacementChangeListeners[i].OnPrePlacementChanged();
		}
	}

	public void NotifyPostChangeListeners()
	{
		for (int i = 0; i < _postPlacementChangeListeners.Count; i++)
		{
			_postPlacementChangeListeners[i].OnPostPlacementChanged();
		}
	}
}
