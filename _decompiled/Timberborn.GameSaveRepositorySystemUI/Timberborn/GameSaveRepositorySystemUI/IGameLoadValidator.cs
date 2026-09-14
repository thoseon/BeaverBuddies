using System;
using Timberborn.GameSaveRepositorySystem;

namespace Timberborn.GameSaveRepositorySystemUI;

public interface IGameLoadValidator
{
	int Priority { get; }

	void ValidateSave(SaveReference saveReference, Action continueCallback);
}
