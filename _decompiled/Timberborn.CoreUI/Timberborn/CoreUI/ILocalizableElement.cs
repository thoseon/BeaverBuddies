using Timberborn.Localization;

namespace Timberborn.CoreUI;

public interface ILocalizableElement
{
	bool IsSet { get; }

	void Localize(ILoc loc);
}
