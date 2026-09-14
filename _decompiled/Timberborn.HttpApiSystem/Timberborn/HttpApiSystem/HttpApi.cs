using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using HandlebarsDotNet;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.HttpApiSystem;

public class HttpApi : IUnloadableSingleton, ILoadableSingleton, ISaveableSingleton
{
	public static readonly string RootPath = Path.Combine(Application.streamingAssetsPath, "HttpApi");

	private static readonly SingletonKey HttpApiKey = new SingletonKey("HttpApi");

	private static readonly PropertyKey<int> PortKey = new PropertyKey<int>("Port");

	private static readonly int DefaultPort = 8080;

	private static readonly TimeSpan TaskStopTimeout = TimeSpan.FromMilliseconds(5000.0);

	private readonly ISingletonLoader _singletonLoader;

	private readonly HttpWebhookCaller _httpWebhookCaller;

	private readonly ImmutableArray<IHttpApiEndpoint> _httpApiEndpoints;

	private HttpListener _httpListener;

	private Task _task;

	private HandlebarsTemplate<object, object> _indexTemplate;

	private volatile bool _stopping;

	public bool IsRunning { get; private set; }

	public string Url { get; private set; }

	public string ErrorMessage { get; private set; }

	public int Port { get; private set; } = DefaultPort;

	public event EventHandler IsRunningChanged;

	public event EventHandler UrlChanged;

	internal HttpApi(ISingletonLoader singletonLoader, HttpWebhookCaller httpWebhookCaller, IEnumerable<IHttpApiEndpoint> httpApiEndpoints)
	{
		_singletonLoader = singletonLoader;
		_httpWebhookCaller = httpWebhookCaller;
		_httpApiEndpoints = httpApiEndpoints.ToImmutableArray();
	}

	public void Load()
	{
		if (_singletonLoader.TryGetSingleton(HttpApiKey, out var objectLoader) && objectLoader.Has(PortKey))
		{
			Port = objectLoader.Get(PortKey);
		}
		UpdateUrl();
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		IObjectSaver singleton = singletonSaver.GetSingleton(HttpApiKey);
		if (Port != DefaultPort)
		{
			singleton.Set(PortKey, Port);
		}
	}

	public void Start()
	{
		if (!IsRunning)
		{
			Debug.Log("Starting HttpApi at " + Url);
			try
			{
				_httpListener = new HttpListener();
				_httpListener.Prefixes.Add(Url);
				_httpListener.Start();
			}
			catch (Exception ex)
			{
				Debug.Log(ex);
				_httpListener = null;
				ErrorMessage = ex.Message;
				return;
			}
			_task = Task.Run((Func<Task>)ProcessRequests);
			_httpWebhookCaller.Start();
			IsRunning = true;
			ErrorMessage = null;
			NotifyIsRunningChanged();
		}
	}

	public void Stop()
	{
		StopInternal(notify: true);
	}

	public void Unload()
	{
		StopInternal(notify: false);
	}

	public void SetPort(ushort value)
	{
		if (Port != value)
		{
			Port = value;
			UpdateUrl();
		}
	}

	private void StopInternal(bool notify)
	{
		if (IsRunning)
		{
			_stopping = true;
			_httpListener.Stop();
			_httpListener.Close();
			if (!_task.Wait(TaskStopTimeout))
			{
				Debug.Log("Failed to stop HttpApi task!");
			}
			_httpListener = null;
			_task = null;
			_httpWebhookCaller.Stop();
			Debug.Log("Stopped HttpApi");
			IsRunning = false;
			_stopping = false;
			if (notify)
			{
				NotifyIsRunningChanged();
			}
		}
	}

	private void UpdateUrl()
	{
		string text = $"http://localhost:{Port}/";
		if (Url != text)
		{
			Url = text;
			UrlChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	private async Task ProcessRequests()
	{
		IsRunning = true;
		while (true)
		{
			try
			{
				HttpListenerContext context = await _httpListener.GetContextAsync();
				try
				{
					if (!(await TryHandleWithEndpoints(context)))
					{
						await Process404(context);
					}
				}
				finally
				{
					context.Response.OutputStream.Close();
				}
			}
			catch (ObjectDisposedException exception)
			{
				if (!_stopping)
				{
					Debug.LogException(exception);
				}
				break;
			}
			catch (Exception exception2)
			{
				Debug.LogException(exception2);
			}
		}
	}

	private async Task<bool> TryHandleWithEndpoints(HttpListenerContext context)
	{
		foreach (IHttpApiEndpoint httpApiEndpoint in _httpApiEndpoints)
		{
			if (await httpApiEndpoint.TryHandle(context))
			{
				return true;
			}
		}
		return false;
	}

	private void NotifyIsRunningChanged()
	{
		IsRunningChanged?.Invoke(this, EventArgs.Empty);
	}

	private static async Task Process404(HttpListenerContext context)
	{
		await context.WriteText("Beaver says URL no exist.", 404);
	}
}
