using System;
using Timberborn.Automation;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.DuplicationSystem;
using Timberborn.EntityNaming;
using Timberborn.EntitySystem;
using Timberborn.Illumination;
using Timberborn.Persistence;
using Timberborn.WorldPersistence;

namespace Timberborn.HttpApiSystem;

public class HttpAdapter : BaseComponent, IAwakableComponent, IPersistentEntity, IDuplicable<HttpAdapter>, IDuplicable, IFinishedStateListener, IRegisteredComponent, IAutomatableNeeder, ITerminal
{
	private static readonly ComponentKey HttpAdapterKey = new ComponentKey("HttpAdapter");

	private static readonly PropertyKey<bool> SwitchedOnWebhookEnabledKey = new PropertyKey<bool>("SwitchedOnWebbookEnabledKey");

	private static readonly PropertyKey<bool> SwitchedOffWebhookEnabledKey = new PropertyKey<bool>("SwitchedOffWebbookEnabledKey");

	private static readonly PropertyKey<string> SwitchedOnWebhookUrlKey = new PropertyKey<string>("SwitchedOnWebbookUrlKey");

	private static readonly PropertyKey<string> SwitchedOffWebhookUrlKey = new PropertyKey<string>("SwitchedOffWebbookUrlKey");

	private static readonly PropertyKey<HttpWebhookMethod> MethodKey = new PropertyKey<HttpWebhookMethod>("MethodKey");

	private static readonly string DefaultSwitchedOnUrl = "http://localhost:8081/on/{name}";

	private static readonly string DefaultSwitchedOffUrl = "http://localhost:8081/off/{name}";

	private readonly HttpApiIntermediary _httpApiIntermediary;

	private readonly HttpWebhookCaller _httpWebhookCaller;

	private Automatable _automatable;

	private UniquelyNamedEntity _uniquelyNamedEntity;

	private IlluminatorToggle _illuminatorToggle;

	private Guid _entityId;

	private ConnectionState? _previousState;

	private volatile int _lastOnCallSuccessful = -1;

	private volatile int _lastOffCallSuccessful = -1;

	private bool _switchedOnWebhookEnabled;

	private bool _switchedOffWebhookEnabled;

	private string _snapshotName;

	public string SwitchedOnWebhookUrl { get; set; } = DefaultSwitchedOnUrl;

	public string SwitchedOffWebhookUrl { get; set; } = DefaultSwitchedOffUrl;

	public HttpWebhookMethod Method { get; set; }

	public bool NeedsAutomatable => true;

	public bool SwitchedOnWebhookEnabled
	{
		get
		{
			return _switchedOnWebhookEnabled;
		}
		set
		{
			_switchedOnWebhookEnabled = value;
			if (!value)
			{
				_lastOnCallSuccessful = -1;
			}
		}
	}

	public bool SwitchedOffWebhookEnabled
	{
		get
		{
			return _switchedOffWebhookEnabled;
		}
		set
		{
			_switchedOffWebhookEnabled = value;
			if (!value)
			{
				_lastOffCallSuccessful = -1;
			}
		}
	}

	public bool? LastOnCallSuccessful
	{
		get
		{
			if (_lastOnCallSuccessful != -1)
			{
				return _lastOnCallSuccessful == 1;
			}
			return null;
		}
	}

	public bool? LastOffCallSuccessful
	{
		get
		{
			if (_lastOffCallSuccessful != -1)
			{
				return _lastOffCallSuccessful == 1;
			}
			return null;
		}
	}

	public string[] AllWebhookUrls => new string[2] { SwitchedOnWebhookUrl, SwitchedOffWebhookUrl };

	internal HttpAdapter(HttpApiIntermediary httpApiIntermediary, HttpWebhookCaller httpWebhookCaller)
	{
		_httpApiIntermediary = httpApiIntermediary;
		_httpWebhookCaller = httpWebhookCaller;
	}

	public void Awake()
	{
		_automatable = GetComponent<Automatable>();
		_uniquelyNamedEntity = GetComponent<UniquelyNamedEntity>();
		_illuminatorToggle = GetComponent<Illuminator>().CreateToggle();
	}

	public void OnEnterFinishedState()
	{
		AddSnapshot();
		_uniquelyNamedEntity.EntityNameChanged += OnUniqueNameChanged;
		_uniquelyNamedEntity.IsUniqueChanged += OnUniqueNameChanged;
	}

	public void OnExitFinishedState()
	{
		RemoveSnapshot();
	}

	public void Save(IEntitySaver entitySaver)
	{
		IObjectSaver component = entitySaver.GetComponent(HttpAdapterKey);
		if (SwitchedOnWebhookEnabled)
		{
			component.Set(SwitchedOnWebhookEnabledKey, SwitchedOnWebhookEnabled);
		}
		if (SwitchedOffWebhookEnabled)
		{
			component.Set(SwitchedOffWebhookEnabledKey, SwitchedOffWebhookEnabled);
		}
		if (SwitchedOnWebhookUrl != DefaultSwitchedOnUrl)
		{
			component.Set(SwitchedOnWebhookUrlKey, SwitchedOnWebhookUrl);
		}
		if (SwitchedOffWebhookUrl != DefaultSwitchedOffUrl)
		{
			component.Set(SwitchedOffWebhookUrlKey, SwitchedOffWebhookUrl);
		}
		component.Set(MethodKey, Method);
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(HttpAdapterKey, out var objectLoader))
		{
			SwitchedOnWebhookEnabled = objectLoader.Has(SwitchedOnWebhookEnabledKey) && objectLoader.Get(SwitchedOnWebhookEnabledKey);
			SwitchedOffWebhookEnabled = objectLoader.Has(SwitchedOffWebhookEnabledKey) && objectLoader.Get(SwitchedOffWebhookEnabledKey);
			if (objectLoader.Has(SwitchedOnWebhookUrlKey))
			{
				SwitchedOnWebhookUrl = objectLoader.Get(SwitchedOnWebhookUrlKey);
			}
			if (objectLoader.Has(SwitchedOffWebhookUrlKey))
			{
				SwitchedOffWebhookUrl = objectLoader.Get(SwitchedOffWebhookUrlKey);
			}
			if (objectLoader.Has(MethodKey))
			{
				Method = objectLoader.Get(MethodKey);
			}
		}
	}

	public void DuplicateFrom(HttpAdapter source)
	{
		SwitchedOnWebhookEnabled = source.SwitchedOnWebhookEnabled;
		SwitchedOffWebhookEnabled = source.SwitchedOffWebhookEnabled;
		SwitchedOnWebhookUrl = source.SwitchedOnWebhookUrl;
		SwitchedOffWebhookUrl = source.SwitchedOffWebhookUrl;
		Method = source.Method;
	}

	public void Evaluate()
	{
		AddSnapshot();
		EnqueueCalls();
		UpdateLight();
	}

	internal void RegisterSuccessfulCall(bool state)
	{
		if (state)
		{
			_lastOnCallSuccessful = 1;
		}
		else
		{
			_lastOffCallSuccessful = 1;
		}
	}

	internal void RegisterFailedCall(bool state)
	{
		if (state)
		{
			_lastOnCallSuccessful = 0;
		}
		else
		{
			_lastOffCallSuccessful = 0;
		}
	}

	private void OnUniqueNameChanged(object sender, EventArgs e)
	{
		RemoveSnapshot();
		AddSnapshot();
	}

	private void AddSnapshot()
	{
		if (_uniquelyNamedEntity.IsUnique)
		{
			_snapshotName = _uniquelyNamedEntity.EntityName;
			_httpApiIntermediary.AddAdapterSnapshot(new HttpAdapterSnapshot(_snapshotName, _automatable.State == ConnectionState.On));
		}
	}

	private void RemoveSnapshot()
	{
		if (_snapshotName != null)
		{
			_httpApiIntermediary.RemoveAdapterSnapshot(_snapshotName);
			_snapshotName = null;
		}
	}

	private void EnqueueCalls()
	{
		ConnectionState state = _automatable.State;
		if (_previousState.HasValue)
		{
			if (SwitchedOnWebhookEnabled && _previousState == ConnectionState.Off && state == ConnectionState.On)
			{
				_httpWebhookCaller.Enqueue(this, state: true, ReplaceTokens(SwitchedOnWebhookUrl), Method);
			}
			else if (SwitchedOffWebhookEnabled && _previousState == ConnectionState.On && state == ConnectionState.Off)
			{
				_httpWebhookCaller.Enqueue(this, state: false, ReplaceTokens(SwitchedOffWebhookUrl), Method);
			}
		}
		_previousState = state;
	}

	private string ReplaceTokens(string url)
	{
		return url.Replace("{name}", Uri.EscapeDataString(_uniquelyNamedEntity.EntityName));
	}

	private void UpdateLight()
	{
		if (_automatable.State == ConnectionState.On)
		{
			_illuminatorToggle.TurnOn();
		}
		else
		{
			_illuminatorToggle.TurnOff();
		}
	}
}
