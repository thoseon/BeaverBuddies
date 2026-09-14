using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Timberborn.Localization;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.CoreUI;

[UxmlElement]
internal class LocalizableToggle : Toggle, ILocalizableElement
{
	[Serializable]
	[CompilerGenerated]
	public new class UxmlSerializedData : Toggle.UxmlSerializedData
	{
		[UxmlAttribute("text-loc-key")]
		[SerializeField]
		private string _textLocKey;

		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UxmlAttributeFlags _textLocKey_UxmlAttributeFlags;

		[RegisterUxmlCache]
		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			UxmlDescriptionCache.RegisterType(typeof(UxmlSerializedData), new UxmlAttributeNames[1]
			{
				new UxmlAttributeNames("_textLocKey", "text-loc-key", null)
			});
		}

		public override object CreateInstance()
		{
			return new LocalizableToggle();
		}

		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			LocalizableToggle localizableToggle = (LocalizableToggle)obj;
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(_textLocKey_UxmlAttributeFlags))
			{
				localizableToggle._textLocKey = _textLocKey;
			}
		}
	}

	[UxmlAttribute("text-loc-key")]
	private string _textLocKey;

	private readonly NineSliceBackground _nineSliceBackground = new NineSliceBackground();

	public bool IsSet => !string.IsNullOrEmpty(_textLocKey);

	public LocalizableToggle()
	{
		Delegate[] obj = base.generateVisualContent?.GetInvocationList() ?? new Delegate[0];
		base.generateVisualContent = (Action<MeshGenerationContext>)Delegate.Combine(base.generateVisualContent, new Action<MeshGenerationContext>(OnGenerateVisualContent));
		Delegate[] array = obj;
		foreach (Delegate obj2 in array)
		{
			base.generateVisualContent = (Action<MeshGenerationContext>)Delegate.Remove(base.generateVisualContent, (Action<MeshGenerationContext>)obj2);
			base.generateVisualContent = (Action<MeshGenerationContext>)Delegate.Combine(base.generateVisualContent, (Action<MeshGenerationContext>)obj2);
		}
		RegisterCallback<CustomStyleResolvedEvent>(OnCustomStyleResolved);
	}

	public void Localize(ILoc loc)
	{
		base.text = loc.T(_textLocKey);
	}

	private void OnCustomStyleResolved(CustomStyleResolvedEvent e)
	{
		_nineSliceBackground.GetDataFromStyle(base.customStyle);
		MarkDirtyRepaint();
	}

	private void OnGenerateVisualContent(MeshGenerationContext mgc)
	{
		if (_nineSliceBackground.IsNineSlice)
		{
			_nineSliceBackground.GenerateVisualContent(mgc, base.paddingRect);
		}
	}
}
