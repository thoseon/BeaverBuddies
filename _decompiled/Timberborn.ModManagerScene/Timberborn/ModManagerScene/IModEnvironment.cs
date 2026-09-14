using JetBrains.Annotations;

namespace Timberborn.ModManagerScene;

public interface IModEnvironment
{
	[UsedImplicitly]
	string ModPath { get; }

	[UsedImplicitly]
	string OriginPath { get; }
}
