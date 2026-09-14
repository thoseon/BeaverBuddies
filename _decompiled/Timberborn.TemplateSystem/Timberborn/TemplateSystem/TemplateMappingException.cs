using System;

namespace Timberborn.TemplateSystem;

public class TemplateMappingException : Exception
{
	public TemplateMappingException()
	{
	}

	public TemplateMappingException(string message)
		: base(message)
	{
	}

	public TemplateMappingException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
