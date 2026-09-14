namespace Timberborn.DistributionSystem;

internal readonly struct ImportableGood
{
	public bool IsImportable { get; }

	public bool HasCapacity { get; }

	public DistributableGood DistributableGood { get; }

	private ImportableGood(bool isImportable, bool hasCapacity, DistributableGood distributableGood)
	{
		IsImportable = isImportable;
		HasCapacity = hasCapacity;
		DistributableGood = distributableGood;
	}

	public static ImportableGood CreateImportableWithCapacity(DistributableGood distributableGood)
	{
		return new ImportableGood(isImportable: true, hasCapacity: true, distributableGood);
	}

	public static ImportableGood CreateNonImportable()
	{
		return new ImportableGood(isImportable: false, hasCapacity: false, default(DistributableGood));
	}

	public static ImportableGood CreateNonImportableWithCapacity()
	{
		return new ImportableGood(isImportable: false, hasCapacity: true, default(DistributableGood));
	}
}
