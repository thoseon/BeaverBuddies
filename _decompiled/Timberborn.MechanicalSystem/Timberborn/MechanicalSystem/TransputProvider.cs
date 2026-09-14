using System.Collections.Immutable;
using Timberborn.BaseComponentSystem;

namespace Timberborn.MechanicalSystem;

public class TransputProvider : BaseComponent, IAwakableComponent
{
	private TransputProviderSpec _transputProviderSpec;

	public ImmutableArray<TransputSpec> TransputSpecs => _transputProviderSpec.Transputs;

	public void Awake()
	{
		_transputProviderSpec = GetComponent<TransputProviderSpec>();
	}
}
