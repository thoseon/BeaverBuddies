using Timberborn.BaseComponentSystem;
using Timberborn.Localization;
using UnityEngine;

namespace Timberborn.EntitySystem;

public class LabeledEntity : BaseComponent, IAwakableComponent
{
	private readonly ILoc _loc;

	private LabeledEntitySpec _labeledEntitySpec;

	private string _displayName;

	private Sprite _image;

	public string DisplayName => _displayName ?? (_displayName = _loc.T(_labeledEntitySpec.DisplayNameLocKey));

	public Sprite Image => _labeledEntitySpec.Icon.Asset;

	public LabeledEntity(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		_labeledEntitySpec = GetComponent<LabeledEntitySpec>();
	}
}
