using System;
using Timberborn.MapRepositorySystem;

namespace Timberborn.MapRepositorySystemUI;

public interface IMapLoadValidator
{
	int Priority { get; }

	void ValidateForNewGame(MapFileReference mapFileReference, Action continueCallback);

	void ValidateForMapEditor(MapFileReference mapFileReference, Action continueCallback);
}
