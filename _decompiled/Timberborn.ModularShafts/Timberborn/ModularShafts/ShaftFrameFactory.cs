using System.Collections.Generic;
using Bindito.Unity;
using Timberborn.Coordinates;
using Timberborn.PrefabOptimization;
using Timberborn.RootProviders;
using Timberborn.SingletonSystem;
using Timberborn.TemplateSystem;
using UnityEngine;

namespace Timberborn.ModularShafts;

internal class ShaftFrameFactory : ILoadableSingleton
{
	private readonly RootObjectProvider _rootObjectProvider;

	private readonly TemplateService _templateService;

	private readonly OptimizedPrefabInstantiator _optimizedPrefabInstantiator;

	private readonly IInstantiator _instantiator;

	private readonly MeshBuilder _meshBuilder = new MeshBuilder();

	private readonly Dictionary<FrameVariant, GameObject> _frames = new Dictionary<FrameVariant, GameObject>();

	private Transform _root;

	private GameObject _shaftBase;

	private GameObject _shaftLowerFrame;

	private GameObject _shaftSupport;

	private GameObject _shaftFrame;

	public ShaftFrameFactory(RootObjectProvider rootObjectProvider, TemplateService templateService, OptimizedPrefabInstantiator optimizedPrefabInstantiator)
	{
		_rootObjectProvider = rootObjectProvider;
		_templateService = templateService;
		_optimizedPrefabInstantiator = optimizedPrefabInstantiator;
	}

	public void Load()
	{
		ModularShaftPartsSpec single = _templateService.GetSingle<ModularShaftPartsSpec>();
		_root = _rootObjectProvider.CreateRootObject("ShaftFrameFactory").transform;
		_shaftBase = Instantiate(single.ShaftBase.Asset, _root);
		_shaftLowerFrame = Instantiate(single.ShaftLowerFrame.Asset, _root);
		_shaftSupport = Instantiate(single.ShaftSupport.Asset, _root);
		_shaftFrame = Instantiate(single.ShaftFrame.Asset, _root);
	}

	public GameObject GetFrame(FrameVariant variant)
	{
		if (!_frames.TryGetValue(variant, out var value))
		{
			value = BuildFrame(variant);
			_frames.Add(variant, value);
		}
		return Instantiate(value, _root);
	}

	private GameObject Instantiate(GameObject gameObject, Transform root)
	{
		GameObject gameObject2 = _optimizedPrefabInstantiator.Instantiate(gameObject, root);
		gameObject2.SetActive(value: false);
		return gameObject2;
	}

	private GameObject BuildFrame(FrameVariant frameVariant)
	{
		GameObject gameObject = new GameObject(frameVariant.GetName());
		gameObject.transform.SetParent(_root);
		BuiltMesh builtMesh = BuildFrameMesh(frameVariant);
		gameObject.AddComponent<MeshFilter>().sharedMesh = builtMesh.Mesh;
		gameObject.AddComponent<MeshRenderer>().sharedMaterials = builtMesh.Materials;
		gameObject.SetActive(value: false);
		return gameObject;
	}

	private BuiltMesh BuildFrameMesh(FrameVariant variant)
	{
		_meshBuilder.Reset(variant.GetName());
		AppendMesh(_shaftBase);
		if (variant.Up)
		{
			AppendMesh(_shaftFrame);
		}
		if (variant.Right)
		{
			AppendMesh(transform: new OrientationTransform(Orientation.Cw90), gameObject: _shaftFrame);
		}
		if (variant.Down)
		{
			AppendMesh(transform: new OrientationTransform(Orientation.Cw180), gameObject: _shaftFrame);
		}
		if (variant.Left)
		{
			AppendMesh(transform: new OrientationTransform(Orientation.Cw270), gameObject: _shaftFrame);
		}
		if (variant.Bottom)
		{
			AppendMesh(_shaftLowerFrame);
		}
		if (variant.Support)
		{
			AppendMesh(_shaftSupport);
		}
		return _meshBuilder.Build();
	}

	private void AppendMesh(GameObject gameObject, ITransform transform = null)
	{
		Transform child = gameObject.transform.GetChild(0);
		MeshFilter component = child.GetComponent<MeshFilter>();
		MeshRenderer component2 = child.GetComponent<MeshRenderer>();
		_meshBuilder.AppendMesh(component.sharedMesh, component2.sharedMaterials, (ITransform)(transform ?? ((object)default(TranslationTransform))));
	}
}
