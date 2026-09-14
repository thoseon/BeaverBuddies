using System;

namespace Timberborn.TemplateInstantiation;

public class CachedTemplateInitializer
{
	public Action<object, object> Method { get; }

	public int SubjectIndex { get; }

	public int DecoratorIndex { get; }

	public CachedTemplateInitializer(Action<object, object> method, int subjectIndex, int decoratorIndex)
	{
		Method = method;
		SubjectIndex = subjectIndex;
		DecoratorIndex = decoratorIndex;
	}
}
