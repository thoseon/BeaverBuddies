using System.Collections.Generic;
using UnityEngine;

namespace Timberborn.ErrorReporting;

internal class LoadingIssueService : ILoadingIssueService
{
	private readonly Dictionary<LoadingIssueMessage, int> _issues = new Dictionary<LoadingIssueMessage, int>();

	public bool HasAnyIssues => _issues.Count > 0;

	public IEnumerable<(LoadingIssueMessage, int)> GetIssues()
	{
		foreach (KeyValuePair<LoadingIssueMessage, int> issue in _issues)
		{
			yield return (issue.Key, issue.Value);
		}
	}

	public void AddIssue(string warningText, string messageLocKey, string messageParam = null, bool paramIsLocKey = false)
	{
		Debug.LogWarning(warningText);
		LoadingIssueMessage key = new LoadingIssueMessage(messageLocKey, messageParam, paramIsLocKey);
		if (_issues.TryGetValue(key, out var value))
		{
			_issues[key] = value + 1;
		}
		else
		{
			_issues[key] = 1;
		}
	}
}
