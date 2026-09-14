using System;

namespace Timberborn.ErrorReporting;

public readonly struct LoadingIssueMessage : IEquatable<LoadingIssueMessage>
{
	public string MessageLocKey { get; }

	public string MessageParam { get; }

	public bool ParamIsLocKey { get; }

	public LoadingIssueMessage(string messageLocKey, string messageParam, bool paramIsLocKey)
	{
		MessageLocKey = messageLocKey;
		MessageParam = messageParam;
		ParamIsLocKey = paramIsLocKey;
	}

	public bool Equals(LoadingIssueMessage other)
	{
		if (MessageLocKey == other.MessageLocKey)
		{
			return MessageParam == other.MessageParam;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is LoadingIssueMessage other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(MessageLocKey, MessageParam);
	}
}
