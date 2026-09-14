using System;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;

namespace Timberborn.CoreUI;

[UxmlElement]
internal class NineSliceTextField : TextField
{
	[Serializable]
	[CompilerGenerated]
	public new class UxmlSerializedData : TextField.UxmlSerializedData
	{
		public override object CreateInstance()
		{
			return new NineSliceTextField();
		}
	}

	private readonly NineSliceBackground _nineSliceBackground = new NineSliceBackground();

	public NineSliceTextField()
	{
		base.generateVisualContent = (Action<MeshGenerationContext>)Delegate.Combine(base.generateVisualContent, new Action<MeshGenerationContext>(OnGenerateVisualContent));
		RegisterCallback<CustomStyleResolvedEvent>(OnCustomStyleResolved);
	}

	private void OnCustomStyleResolved(CustomStyleResolvedEvent e)
	{
		_nineSliceBackground.GetDataFromStyle(base.customStyle);
		MarkDirtyRepaint();
	}

	private void OnGenerateVisualContent(MeshGenerationContext mgc)
	{
		_nineSliceBackground.GenerateVisualContent(mgc, base.paddingRect);
	}
}
