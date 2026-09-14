using Timberborn.BaseComponentSystem;

namespace Timberborn.PathSystem;

internal class PathModelTypeEnforcer : BaseComponent, IAwakableComponent
{
	private PathModelTypeEnforcerSpec _pathModelTypeEnforcerSpec;

	public PathModelType PathModelType => _pathModelTypeEnforcerSpec.PathModelType;

	public void Awake()
	{
		_pathModelTypeEnforcerSpec = GetComponent<PathModelTypeEnforcerSpec>();
	}
}
