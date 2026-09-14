using System;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;

namespace Timberborn.MechanicalSystem;

public class MechanicalNodeActuals : BaseComponent
{
	public int PowerOutput { get; private set; }

	public int PowerInput { get; private set; }

	public int BatteryCharge { get; private set; }

	public int BatteryCapacity { get; private set; }

	public event EventHandler<ValueChangedEventArgs<int>> PowerOutputChanged;

	public event EventHandler<ValueChangedEventArgs<int>> PowerInputChanged;

	public event EventHandler<ValueChangedEventArgs<int>> BatteryChargeChanged;

	public event EventHandler<ValueChangedEventArgs<int>> BatteryCapacityChanged;

	internal void SetPowerOutput(int value)
	{
		if (PowerOutput != value)
		{
			int powerOutput = PowerOutput;
			PowerOutput = value;
			PowerOutputChanged?.Invoke(this, new ValueChangedEventArgs<int>(powerOutput, value));
		}
	}

	internal void SetPowerInput(int value)
	{
		if (PowerInput != value)
		{
			int powerInput = PowerInput;
			PowerInput = value;
			PowerInputChanged?.Invoke(this, new ValueChangedEventArgs<int>(powerInput, value));
		}
	}

	internal void SetBatteryCharge(int value)
	{
		if (BatteryCharge != value)
		{
			int batteryCharge = BatteryCharge;
			BatteryCharge = value;
			BatteryChargeChanged?.Invoke(this, new ValueChangedEventArgs<int>(batteryCharge, value));
		}
	}

	internal void SetBatteryCapacity(int value)
	{
		if (BatteryCapacity != value)
		{
			int batteryCapacity = BatteryCapacity;
			BatteryCapacity = value;
			BatteryCapacityChanged?.Invoke(this, new ValueChangedEventArgs<int>(batteryCapacity, value));
		}
	}
}
