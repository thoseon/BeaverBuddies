namespace Timberborn.StoreSystem;

public interface IStore
{
	bool GameIsAllowedToStart { get; }

	string Language { get; }

	string ShortUpdateUrl { get; }

	string FullUpdateUrl { get; }

	string UpdateInfoTextLocKey { get; }

	string GetCompatibilityMessage();
}
