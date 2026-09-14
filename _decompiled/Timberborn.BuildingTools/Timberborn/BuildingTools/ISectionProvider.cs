using Timberborn.BlockSystem;
using UnityEngine.UIElements;

namespace Timberborn.BuildingTools;

public interface ISectionProvider
{
	bool TryGetSection(Preview preview, out VisualElement section);
}
