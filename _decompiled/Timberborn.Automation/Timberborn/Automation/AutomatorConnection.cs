namespace Timberborn.Automation;

public class AutomatorConnection
{
	private readonly AutomationRunner _automationRunner;

	public Automator Receiver { get; }

	public Automator Transmitter { get; private set; }

	public ConnectionState State
	{
		get
		{
			if (Transmitter == null)
			{
				return ConnectionState.Disconnected;
			}
			if (Transmitter.State != AutomatorState.On)
			{
				return ConnectionState.Off;
			}
			return ConnectionState.On;
		}
	}

	public bool BooleanState => State == ConnectionState.On;

	public bool IsConnected => Transmitter != null;

	internal AutomatorConnection(Automator receiver, AutomationRunner automationRunner)
	{
		Receiver = receiver;
		_automationRunner = automationRunner;
	}

	public void Connect(Automator transmitter)
	{
		if (transmitter != null && transmitter.IsTransmitter)
		{
			if (Transmitter != transmitter)
			{
				DisconnectInternal();
				Transmitter = transmitter;
				transmitter.ConnectToOutput(this);
				if (Receiver.RegisteredForRunning && transmitter.RegisteredForRunning)
				{
					_automationRunner.MergePartitions(Receiver.Partition, transmitter.Partition);
				}
				if (Receiver.RegisteredForRunning)
				{
					Receiver.Partition?.InvalidatePlan();
				}
				Receiver.OnInputReconnected();
			}
		}
		else
		{
			Disconnect();
		}
	}

	public void Disconnect()
	{
		if (Transmitter != null)
		{
			DisconnectInternal();
			Receiver.OnInputReconnected();
		}
	}

	public void Remove()
	{
		Disconnect();
		Receiver.RemoveInput(this);
	}

	private void DisconnectInternal()
	{
		Automator transmitter = Transmitter;
		if (transmitter != null)
		{
			transmitter.DisconnectFromOutput(this);
			Transmitter = null;
			if (transmitter.RegisteredForRunning)
			{
				transmitter.Partition?.InvalidatePlan();
			}
			if (Receiver.RegisteredForRunning)
			{
				Receiver.Partition?.InvalidatePlan();
			}
			if (Receiver.RegisteredForRunning && transmitter.RegisteredForRunning && transmitter.Partition == Receiver.Partition)
			{
				_automationRunner.ReassignExistingPartition(Receiver.Partition);
			}
		}
	}
}
