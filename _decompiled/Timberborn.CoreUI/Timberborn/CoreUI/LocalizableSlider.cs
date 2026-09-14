using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Timberborn.Localization;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.CoreUI;

[UxmlElement]
internal class LocalizableSlider : Slider, ILocalizableElement
{
	[Serializable]
	[CompilerGenerated]
	public new class UxmlSerializedData : Slider.UxmlSerializedData
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
			return new LocalizableSlider();
		}

		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			LocalizableSlider localizableSlider = (LocalizableSlider)obj;
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(_textLocKey_UxmlAttributeFlags))
			{
				localizableSlider._textLocKey = _textLocKey;
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
