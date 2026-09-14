using System;
using Timberborn.Debugging;

namespace Timberborn.DiagnosticsUI;

internal class GCTrigger : IDevModule
{
	public DevModuleDefinition GetDefinition()
	{
		return new DevModuleDefinition.Builder().AddMethod(DevMethod.Create("Trigger GC", GC.Collect)).Build();
	}
}
