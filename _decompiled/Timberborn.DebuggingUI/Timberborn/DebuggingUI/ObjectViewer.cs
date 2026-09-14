using Timberborn.SingletonSystem;
using UnityEngine.UIElements;

namespace Timberborn.DebuggingUI;

internal class ObjectViewer : IUpdatableSingleton
{
	private readonly ObjectViewerNodeFactory _objectViewerNodeFactory;

	private ScrollView _root;

	private object _viewedObject;

	private ObjectViewerNode _rootNode;

	public ObjectViewer(ObjectViewerNodeFactory objectViewerNodeFactory)
	{
		_objectViewerNodeFactory = objectViewerNodeFactory;
	}

	public void Initialize(ScrollView root)
	{
		_root = root;
	}

	public void SetObject(object viewedObject)
	{
		_root.Clear();
		_viewedObject = viewedObject;
		_rootNode = _objectViewerNodeFactory.CreateRoot(viewedObject.GetType().Name);
		_rootNode.Update(viewedObject);
		_root.Add(_rootNode.Root);
	}

	public void UpdateSingleton()
	{
		_rootNode?.Update(_viewedObject);
	}

	public void Clear()
	{
		_root.Clear();
		_viewedObject = null;
		_rootNode = null;
	}
}
