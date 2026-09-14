namespace Timberborn.HttpApiSystem;

public interface IHttpApiPageSection
{
	int Order { get; }

	string BuildBody();

	string BuildFooter();
}
