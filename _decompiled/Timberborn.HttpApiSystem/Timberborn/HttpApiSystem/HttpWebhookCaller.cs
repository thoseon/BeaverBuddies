using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.HttpApiSystem;

internal class HttpWebhookCaller : IUnloadableSingleton
{
	private struct Call
	{
		public HttpAdapter HttpAdapter { get; }

		public bool State { get; }

		public string Url { get; }

		public HttpWebhookMethod Method { get; }

		public Call(HttpAdapter httpAdapter, bool state, string url, HttpWebhookMethod method)
		{
			HttpAdapter = httpAdapter;
			State = state;
			Url = url;
			Method = method;
		}
	}

	private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(5.0);

	private static readonly int MaxPendingCalls = 100;

	private static readonly int[] BackoffMs = new int[6] { 1000, 2000, 4000, 8000, 16000, 32000 };

	private BlockingCollection<Call> _pendingCalls = new BlockingCollection<Call>();

	private Thread _thread;

	private HttpClient _http;

	private readonly TimeWindowLimiter _nonLoopbackLimiter = new TimeWindowLimiter(30, TimeSpan.FromMinutes(1.0));

	private readonly Dictionary<string, int> _consecutiveFailuresByHost = new Dictionary<string, int>();

	public void Unload()
	{
		Stop();
	}

	public void Start()
	{
		if (_thread == null)
		{
			_http = new HttpClient();
			_http.Timeout = Timeout;
			_thread = new Thread(ThreadLoop)
			{
				IsBackground = true,
				Name = "HttpWebhookCaller"
			};
			_thread.Start();
		}
	}

	public void Stop()
	{
		if (_thread != null)
		{
			_pendingCalls.CompleteAdding();
			_thread?.Join();
			_http = null;
			_thread = null;
			_pendingCalls = new BlockingCollection<Call>();
		}
	}

	public void Enqueue(HttpAdapter httpAdapter, bool state, string url, HttpWebhookMethod method)
	{
		if (_thread != null && !string.IsNullOrWhiteSpace(url) && _pendingCalls.Count < MaxPendingCalls)
		{
			_pendingCalls.Add(new Call(httpAdapter, state, url.Trim(), method));
		}
	}

	private void ThreadLoop()
	{
		StringContent content = new StringContent("");
		Call item;
		while (_pendingCalls.TryTake(out item, -1))
		{
			if (!IsAllowedUrl(item.Url, out var uri))
			{
				continue;
			}
			if (uri.IsLoopback || _nonLoopbackLimiter.TryAcquirePermit())
			{
				string host = uri.Host;
				try
				{
					using HttpResponseMessage httpResponseMessage = item.Method switch
					{
						HttpWebhookMethod.Get => _http.GetAsync(uri).Result, 
						HttpWebhookMethod.Post => _http.PostAsync(uri, content).Result, 
						_ => throw new ArgumentOutOfRangeException(item.Method.ToString()), 
					};
					httpResponseMessage.EnsureSuccessStatusCode();
					item.HttpAdapter.RegisterSuccessfulCall(item.State);
					ResetBackoff(host);
				}
				catch (Exception arg)
				{
					item.HttpAdapter.RegisterFailedCall(item.State);
					int num = (uri.IsLoopback ? 1000 : IncrementAndGetBackoffMs(host));
					Debug.Log($"Failed webhook call to {item.Url}, backing off for {num}ms.\n" + $"{arg}");
					Thread.Sleep(num);
					ClearPendingCalls();
				}
			}
			else
			{
				item.HttpAdapter.RegisterFailedCall(item.State);
				Debug.Log($"Throttled webhook call to {uri}");
			}
		}
	}

	private void ClearPendingCalls()
	{
		Call item;
		while (_pendingCalls.TryTake(out item))
		{
		}
	}

	private void ResetBackoff(string host)
	{
		_consecutiveFailuresByHost[host] = 0;
	}

	private int IncrementAndGetBackoffMs(string host)
	{
		int num = _consecutiveFailuresByHost.GetValueOrDefault(host) + 1;
		_consecutiveFailuresByHost[host] = num;
		return BackoffMs[Math.Min(num - 1, BackoffMs.Length - 1)];
	}

	private static bool IsAllowedUrl(string input, out Uri uri)
	{
		if (!Uri.TryCreate(input, UriKind.Absolute, out uri))
		{
			return false;
		}
		if (!(uri.Scheme == Uri.UriSchemeHttp))
		{
			return uri.Scheme == Uri.UriSchemeHttps;
		}
		return true;
	}
}
