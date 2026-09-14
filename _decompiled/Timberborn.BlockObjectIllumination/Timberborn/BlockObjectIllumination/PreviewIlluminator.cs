using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Illumination;

namespace Timberborn.BlockObjectIllumination;

internal class PreviewIlluminator : BaseComponent, IInitializablePreview
{
	public void InitializePreview()
	{
		GetComponent<Illuminator>().CreateToggle().Disable();
	}
}
