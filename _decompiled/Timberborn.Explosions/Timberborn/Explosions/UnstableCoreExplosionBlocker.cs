using Timberborn.BaseComponentSystem;
using Timberborn.MapStateSystem;

namespace Timberborn.Explosions;

public class UnstableCoreExplosionBlocker : BaseComponent
{
	private readonly MapEditorMode _mapEditorMode;

	private bool _explosionBlocked;

	public bool ExplosionBlocked
	{
		get
		{
			if (base.Enabled)
			{
				if (!_explosionBlocked)
				{
					return _mapEditorMode.IsMapEditor;
				}
				return true;
			}
			return false;
		}
	}

	public UnstableCoreExplosionBlocker(MapEditorMode mapEditorMode)
	{
		_mapEditorMode = mapEditorMode;
	}

	public void BlockExplosion()
	{
		_explosionBlocked = true;
	}

	public void Disable()
	{
		DisableComponent();
	}
}
