using System.Reflection;
using UnityEngine.UIElements;

namespace Timberborn.DebuggingUI;

internal class ObjectViewerFieldNode : ObjectViewerNode
{
	private readonly TextField _textField;

	public ObjectViewerFieldNode(VisualElement root, FieldInfo nodeFieldInfo, ObjectViewerNodeFactory objectViewerNodeFactory, TextField textField)
		: base(objectViewerNodeFactory, root, nodeFieldInfo)
	{
		_textField = textField;
	}

	public override void Update(object parent)
	{
		base.Update(parent);
		_textField.SetValueWithoutNotify(base.Value?.ToString() ?? "(null)");
	}
}
