using System.Linq;
using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.StatusSystem;
using Timberborn.TooltipSystem;
using UnityEngine.UIElements;

namespace Timberborn.StatusSystemUI;

internal class StatusBatchControlRowItem : IBatchControlRowItem, IInitializableBatchControlItem, IClearableBatchControlRowItem
{
	private readonly StatusSubject _statusSubject;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	public VisualElement Root { get; }

	public StatusBatchControlRowItem(VisualElement root, StatusSubject statusSubject, VisualElementLoader visualElementLoader, ITooltipRegistrar tooltipRegistrar)
	{
		Root = root;
		_statusSubject = statusSubject;
		_visualElementLoader = visualElementLoader;
		_tooltipRegistrar = tooltipRegistrar;
	}

	public void InitializeRowItem()
	{
		_statusSubject.StatusToggled += OnStatusToggled;
		UpdateStatuses();
	}

	public void ClearRowItem()
	{
		_statusSubject.StatusToggled -= OnStatusToggled;
	}

	private void OnStatusToggled(object sender, StatusInstance statusInstance)
	{
		UpdateStatuses();
	}

	private void UpdateStatuses()
	{
		Root.Clear();
		foreach (StatusInstance item in _statusSubject.ActiveStatuses.Where((StatusInstance status) => status.IsVisible()))
		{
			VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/BatchControl/StatusImage");
			Image image = visualElement.Q<Image>("StatusImage");
			image.sprite = item.IconSmall;
			_tooltipRegistrar.Register(image, item.StatusDescription);
			Root.Add(visualElement);
		}
	}
}
