using Timberborn.CoreUI;
using Timberborn.MultithreadingAnalysis;
using Timberborn.TooltipSystem;

namespace Timberborn.MultithreadingAnalysisUI;

internal class MarkerViewFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	public MarkerViewFactory(VisualElementLoader visualElementLoader, ITooltipRegistrar tooltipRegistrar)
	{
		_visualElementLoader = visualElementLoader;
		_tooltipRegistrar = tooltipRegistrar;
	}

	public MarkerView CreateMarker(Marker marker)
	{
		MarkerView markerView = new MarkerView(_visualElementLoader.LoadVisualElement("Common/MultithreadingAnalysis/MarkerView"), marker);
		_tooltipRegistrar.Register(markerView.Root, markerView.GetTooltipText());
		return markerView;
	}
}
