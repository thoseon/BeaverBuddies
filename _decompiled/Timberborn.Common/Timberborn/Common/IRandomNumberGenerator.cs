using System.Collections.Generic;
using UnityEngine;

namespace Timberborn.Common;

public interface IRandomNumberGenerator
{
	float Range(float inclusiveMin, float inclusiveMax);

	int Range(int inclusiveMin, int exclusiveMax);

	Vector2 InsideUnitCircle();

	T GetListElement<T>(IReadOnlyList<T> list);

	bool TryGetListElement<T>(IReadOnlyList<T> list, out T randomElement);

	T GetListElementOrDefault<T>(IReadOnlyList<T> list);

	T GetEnumerableElement<T>(IEnumerable<T> source);

	bool TryGetEnumerableElement<T>(IEnumerable<T> source, out T randomElement);

	bool CheckProbability(float normalizedProbability);
}
