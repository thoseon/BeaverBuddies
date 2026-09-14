using UnityEngine;

namespace Timberborn.Localization;

public interface ILocalizationCsvValidator
{
	void Validate(TextAsset textAsset);
}
