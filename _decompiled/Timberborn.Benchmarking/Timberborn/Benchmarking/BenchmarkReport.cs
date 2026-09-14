using System.Text;

namespace Timberborn.Benchmarking;

internal class BenchmarkReport
{
	internal class Builder
	{
		private static readonly string Indentation = "  ";

		private readonly StringBuilder _humanReadableContent = new StringBuilder();

		private readonly StringBuilder _csvHeader = new StringBuilder();

		private readonly StringBuilder _csvEntry = new StringBuilder();

		private string _section = "";

		public Builder AddTitle(string title)
		{
			_humanReadableContent.AppendLine(title);
			return this;
		}

		public Builder AddSection(string section)
		{
			_section = section;
			_humanReadableContent.AppendLine(section);
			return this;
		}

		public Builder AddEntry(string name, object value)
		{
			return AddEntry(name, value, value.ToString());
		}

		public Builder AddEntry(string name, object value, string humanReadableValue)
		{
			AppendToHumanReadableContent(name, humanReadableValue);
			AppendToCsvHeader(name);
			AppendToCsvRow(value);
			return this;
		}

		public BenchmarkReport Build()
		{
			return new BenchmarkReport(_humanReadableContent.ToString(), _csvHeader.ToString(), _csvEntry.ToString());
		}

		private void AppendToHumanReadableContent(string name, string humanReadableValue)
		{
			if (!string.IsNullOrWhiteSpace(_section))
			{
				_humanReadableContent.Append(Indentation);
			}
			_humanReadableContent.AppendLine(name + ": " + humanReadableValue);
		}

		private void AppendToCsvHeader(string name)
		{
			AppendToCsvLine(_csvHeader, string.IsNullOrWhiteSpace(_section) ? name : (_section + ": " + name));
		}

		private void AppendToCsvRow(object value)
		{
			AppendToCsvLine(_csvEntry, value);
		}

		private void AppendToCsvLine(StringBuilder csvLine, object value)
		{
			if (csvLine.Length > 0)
			{
				csvLine.Append(",");
			}
			csvLine.Append($"\"{value}\"");
		}
	}

	public string HumanReadableContent { get; }

	public string CsvHeader { get; }

	public string CsvRow { get; }

	private BenchmarkReport(string humanReadableContent, string csvHeader, string csvRow)
	{
		HumanReadableContent = humanReadableContent;
		CsvHeader = csvHeader;
		CsvRow = csvRow;
	}
}
