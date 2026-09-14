using System;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;

namespace Timberborn.CoreUI;

[UxmlElement]
internal class NineSliceFloatField : FloatField
{
	[Serializable]
	[CompilerGenerated]
	public new class UxmlSerializedData : FloatField.UxmlSerializedData
	{
		public override object CreateInstance()
		{
			return new NineSliceFloatField();
		}
	}

	private readonly NineSliceBackground _nineSliceBackground = new NineSliceBackground();

	public NineSliceFloatField()
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
