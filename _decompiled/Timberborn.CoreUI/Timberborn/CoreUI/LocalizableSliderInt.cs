using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Timberborn.Localization;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.CoreUI;

[UxmlElement]
internal class LocalizableSliderInt : SliderInt, ILocalizableElement
{
	[Serializable]
	[CompilerGenerated]
	public new class UxmlSerializedData : SliderInt.UxmlSerializedData
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
			return new LocalizableSliderInt();
		}

		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			LocalizableSliderInt localizableSliderInt = (LocalizableSliderInt)obj;
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(_textLocKey_UxmlAttributeFlags))
			{
				localizableSliderInt._textLocKey = _textLocKey;
			}
		}
	}

	[UxmlAttribute("text-loc-key")]
	private string _textLocKey;

	public bool IsSet => !string.IsNullOrEmpty(_textLocKey);

	public void Localize(ILoc loc)
	{
		base.label = loc.T(_textLocKey);
	}
}
