namespace Timberborn.ZiplineSystem;

public class ZiplineConnectionActivatedEvent
{
	public ZiplineTower ZiplineTower { get; }

	public ZiplineConnectionActivatedEvent(ZiplineTower ziplineTower)
	{
		ZiplineTower = ziplineTower;
	}
}
