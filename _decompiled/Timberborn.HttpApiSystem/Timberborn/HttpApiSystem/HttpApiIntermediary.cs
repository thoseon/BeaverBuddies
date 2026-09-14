using System.Collections.Concurrent;
using System.Collections.Immutable;
using Timberborn.EntityNaming;
using Timberborn.SingletonSystem;

namespace Timberborn.HttpApiSystem;

internal class HttpApiIntermediary : IUpdatableSingleton
{
	private readonly UniquelyNamedEntityService _uniquelyNamedEntityService;

	private readonly ConcurrentDictionary<string, HttpAdapterSnapshot> _adapters = new ConcurrentDictionary<string, HttpAdapterSnapshot>();

	private readonly ConcurrentDictionary<string, HttpLeverSnapshot> _levers = new ConcurrentDictionary<string, HttpLeverSnapshot>();

	private readonly ConcurrentQueue<HttpLeverCommand> _leverCommands = new ConcurrentQueue<HttpLeverCommand>();

	public HttpApiIntermediary(UniquelyNamedEntityService uniquelyNamedEntityService)
	{
		_uniquelyNamedEntityService = uniquelyNamedEntityService;
	}

	public void AddAdapterSnapshot(HttpAdapterSnapshot httpAdapterSnapshot)
	{
		_adapters[httpAdapterSnapshot.Name] = httpAdapterSnapshot;
	}

	public void RemoveAdapterSnapshot(string name)
	{
		_adapters.TryRemove(name, out var _);
	}

	public void AddLeverSnapshot(HttpLeverSnapshot httpLeverSnapshot)
	{
		_levers[httpLeverSnapshot.Name] = httpLeverSnapshot;
	}

	public void RemoveLeverSnapshot(string name)
	{
		_levers.TryRemove(name, out var _);
	}

	public void AddLeverCommand(HttpLeverCommand httpLeverCommand)
	{
		_leverCommands.Enqueue(httpLeverCommand);
	}

	public ImmutableArray<HttpAdapterSnapshot> GetAdapters()
	{
		return _adapters.Values.ToImmutableArray();
	}

	public ImmutableArray<HttpLeverSnapshot> GetLevers()
	{
		return _levers.Values.ToImmutableArray();
	}

	public bool TryGetAdapter(string name, out HttpAdapterSnapshot httpAdapterSnapshot)
	{
		return _adapters.TryGetValue(name, out httpAdapterSnapshot);
	}

	public bool TryGetLever(string name, out HttpLeverSnapshot httpLeverSnapshot)
	{
		return _levers.TryGetValue(name, out httpLeverSnapshot);
	}

	public void UpdateSingleton()
	{
		HttpLeverCommand result;
		while (_leverCommands.TryDequeue(out result))
		{
			if (!_uniquelyNamedEntityService.TryGet(result.Name, out var uniquelyNamedEntity))
			{
				continue;
			}
			HttpLever component = uniquelyNamedEntity.GetComponent<HttpLever>();
			if (component != null)
			{
				if (result.State.HasValue)
				{
					component.SetState(result.State.Value);
				}
				if (result.Color.HasValue)
				{
					component.SetColor(result.Color.Value);
				}
			}
		}
	}
}
