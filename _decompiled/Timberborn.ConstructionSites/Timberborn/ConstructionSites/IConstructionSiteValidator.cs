using System;

namespace Timberborn.ConstructionSites;

public interface IConstructionSiteValidator
{
	bool IsValid { get; }

	bool IsModelValid { get; }

	event EventHandler ValidationStateChanged;

	void Validate();
}
