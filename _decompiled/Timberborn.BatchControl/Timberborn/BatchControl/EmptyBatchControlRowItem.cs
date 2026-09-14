using UnityEngine.UIElements;

namespace Timberborn.BatchControl;

public class EmptyBatchControlRowItem : IBatchControlRowItem
{
	public VisualElement Root { get; }

	public EmptyBatchControlRowItem(VisualElement root)
	{
		Root = root;
	}
}
