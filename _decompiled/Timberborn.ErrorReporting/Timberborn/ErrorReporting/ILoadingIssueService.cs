using System.Collections.Generic;

namespace Timberborn.ErrorReporting;

public interface ILoadingIssueService
{
	bool HasAnyIssues { get; }

	IEnumerable<(LoadingIssueMessage, int)> GetIssues();

	void AddIssue(string warningText, string messageLocKey, string messageParam = null, bool paramIsLocKey = false);
}
