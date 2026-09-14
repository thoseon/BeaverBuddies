using System.Collections.Generic;
using System.Collections.Immutable;

namespace Timberborn.Debugging;

public class DevModuleDefinition
{
	public class Builder
	{
		private readonly List<DevMethod> _methods = new List<DevMethod>();

		public Builder AddMethod(DevMethod devMethod)
		{
			_methods.Add(devMethod);
			return this;
		}

		public DevModuleDefinition Build()
		{
			return new DevModuleDefinition(_methods);
		}
	}

	public ImmutableArray<DevMethod> Methods { get; }

	private DevModuleDefinition(IEnumerable<DevMethod> methods)
	{
		Methods = methods.ToImmutableArray();
	}
}
