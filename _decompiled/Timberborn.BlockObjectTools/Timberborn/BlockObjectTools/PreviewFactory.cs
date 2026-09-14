using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.RootProviders;
using Timberborn.SingletonSystem;
using Timberborn.TemplateInstantiation;
using UnityEngine;

namespace Timberborn.BlockObjectTools;

public class PreviewFactory : ILoadableSingleton
{
	private readonly TemplateInstantiator _templateInstantiator;

	private readonly RootObjectProvider _rootObjectProvider;

	private Transform _parent;

	public PreviewFactory(TemplateInstantiator templateInstantiator, RootObjectProvider rootObjectProvider)
	{
		_templateInstantiator = templateInstantiator;
		_rootObjectProvider = rootObjectProvider;
	}

	public void Load()
	{
		_parent = _rootObjectProvider.CreateRootObject("PreviewFactory").transform;
	}

	public Preview Create(PlaceableBlockObjectSpec template)
	{
		GameObject gameObject = _templateInstantiator.Instantiate(template.Blueprint, _parent);
		gameObject.SetActive(value: false);
		gameObject.name = gameObject.name.Replace("(Clone)", "Preview");
		Preview componentSlow = gameObject.GetComponentSlow<Preview>();
		componentSlow.GetComponent<BlockObject>().MarkAsPreviewAndInitialize();
		return componentSlow;
	}
}
