using System;
using System.Collections.Immutable;
using System.Linq;

namespace Timberborn.Versioning;

public readonly struct Version
{
	private static readonly char ExperimentalSymbol = 'x';

	private readonly bool _isExperimental;

	private readonly ImmutableArray<int> _subNumbers;

	public string Numeric { get; }

	public string Full { get; }

	public string Formatted => "v" + Full;

	public string NumericWithBranch
	{
		get
		{
			if (!_isExperimental)
			{
				return Numeric;
			}
			return $"{Numeric}-{ExperimentalSymbol}";
		}
	}

	public bool IsDevelopmentVersion => Numeric == "0";

	private Version(string numeric, string full, bool isExperimental, ImmutableArray<int> subNumbers)
	{
		Numeric = numeric;
		Full = full;
		_isExperimental = isExperimental;
		_subNumbers = subNumbers;
	}

	public static Version Create(string version)
	{
		string text = ExtractVersionNumber(version);
		return new Version(subNumbers: text.Split('.').Select(int.Parse).ToImmutableArray(), numeric: text, full: version, isExperimental: GetExperimental(version));
	}

	public override string ToString()
	{
		return Numeric;
	}

	public bool IsEqualOrHigherThan(Version other, int? depth = null)
	{
		int num = (depth.HasValue ? Math.Min(depth.Value, _subNumbers.Length) : _subNumbers.Length);
		for (int i = 0; i < num; i++)
		{
			if (i >= other._subNumbers.Length)
			{
				return true;
			}
			if (_subNumbers[i] > other._subNumbers[i])
			{
				return true;
			}
			if (_subNumbers[i] < other._subNumbers[i])
			{
				return false;
			}
		}
		int num2 = (depth.HasValue ? Math.Min(depth.Value, other._subNumbers.Length) : other._subNumbers.Length);
		for (int j = num; j < num2; j++)
		{
			if (other._subNumbers[j] > 0)
			{
				return false;
			}
		}
		return true;
	}

	public bool IsFromSameBranch(Version other)
	{
		return _isExperimental == other._isExperimental;
	}

	private static string ExtractVersionNumber(string version)
	{
		if (!VersionIsInOldFormat(version))
		{
			return version.Split('-')[0];
		}
		return "0.0.0.0";
	}

	private static bool VersionIsInOldFormat(string version)
	{
		return version[0] == 'v';
	}

	private static bool GetExperimental(string version)
	{
		string[] array = version.Split('-');
		if (!VersionIsInOldFormat(version) && array.Length > 2)
		{
			return array[2].StartsWith(ExperimentalSymbol);
		}
		return false;
	}
}
