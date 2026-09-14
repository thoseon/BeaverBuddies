using System.Collections.Generic;
using Timberborn.CoreUI;
using Timberborn.Localization;
using UnityEngine.UIElements;

namespace Timberborn.BatchControl;

public class BatchControlRowGroupFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly ILoc _loc;

	public BatchControlRowGroupFactory(VisualElementLoader visualElementLoader, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_loc = loc;
	}

	public BatchControlRowGroup CreateUnsorted(BatchControlRow header)
	{
		return Create(header);
	}

	public BatchControlRowGroup CreateSortedWithTextHeader(string headerTextLocKey, string sortingKey)
	{
		return Create(_loc.T(headerTextLocKey), sortingKey);
	}

	public BatchControlRowGroup CreateSortedWithTextHeader(string headerTextLocKey)
	{
		string text = _loc.T(headerTextLocKey);
		return Create(text, text);
	}

	private BatchControlRowGroup Create(string headerText, string sortingKey)
	{
		string elementName = "Game/BatchControl/BatchControlHeaderRow";
		VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
		visualElement.Q<Label>("Text").text = headerText;
		BatchControlRowGroupChildrenCounter batchControlRowGroupChildrenCounter = CreateCounter();
		BatchControlRowGroup batchControlRowGroup = Create(new BatchControlRow(visualElement, batchControlRowGroupChildrenCounter), new SortableNameRowComparer(), sortingKey);
		batchControlRowGroupChildrenCounter.SetRowGroup(batchControlRowGroup);
		return batchControlRowGroup;
	}

	private BatchControlRowGroup Create(BatchControlRow header, IComparer<BatchControlRow> comparer = null, string sortingKey = "")
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/BatchControl/BatchControlRowGroup");
		visualElement.Add(header.Root);
		return new BatchControlRowGroup(visualElement, sortingKey, header, comparer);
	}

	private BatchControlRowGroupChildrenCounter CreateCounter()
	{
		string elementName = "Game/BatchControl/BatchControlRowGroupChildrenCounter";
		VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
		Label counter = visualElement.Q<Label>("Counter");
		return new BatchControlRowGroupChildrenCounter(visualElement, counter);
	}
}
