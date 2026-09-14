using JetBrains.Annotations;
using LINQtoCSV;

namespace Timberborn.Localization;

internal class LocalizationRecord
{
	[CsvColumn(Name = "ID")]
	public string Id { get; set; }

	[CsvColumn(Name = "Text")]
	public string Text { get; set; }

	[UsedImplicitly]
	[CsvColumn(Name = "Comment")]
	public string Comment { get; set; }

	public bool HideWarning { get; set; }

	public bool IsBuiltIn { get; set; }
}
