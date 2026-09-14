using System.Text;
using Timberborn.NeedSpecs;
using Timberborn.NeedSystem;

namespace Timberborn.WellbeingUI;

public interface INeedEffectDescriber
{
	void DescribeNeedEffects(StringBuilder content, NeedManager needManager, NeedSpec needSpec);
}
