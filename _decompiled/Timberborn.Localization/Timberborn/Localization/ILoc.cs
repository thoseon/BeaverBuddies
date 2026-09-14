using System.Collections.Generic;

namespace Timberborn.Localization;

public interface ILoc
{
	void Initialize(Dictionary<string, string> localization);

	IEnumerable<string> GetRawTexts();

	string T(string key);

	string T<T1>(string key, T1 param1);

	string T<T1, T2>(string key, T1 param1, T2 param2);

	string T<T1, T2, T3>(string key, T1 param1, T2 param2, T3 param3);

	string T(Phrase phrase);

	string T<T1>(Phrase phrase, T1 param1);

	string T<T1, T2>(Phrase phrase, T1 param1, T2 param2);

	string T<T1, T2, T3>(Phrase phrase, T1 param1, T2 param2, T3 param3);
}
