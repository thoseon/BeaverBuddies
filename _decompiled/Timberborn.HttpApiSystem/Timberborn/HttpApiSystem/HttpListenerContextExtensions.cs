using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Timberborn.HttpApiSystem;

internal static class HttpListenerContextExtensions
{
	public static async Task WriteText(this HttpListenerContext context, string text, int statusCode)
	{
		if (statusCode != 200 && context.Request.Url.AbsolutePath.ToLowerInvariant() != "/favicon.ico" && !context.Request.Url.AbsolutePath.ToLowerInvariant().EndsWith(".map"))
		{
			Debug.Log($"Responding with {statusCode} to {context.Request.HttpMethod}" + $" {context.Request.Url}");
		}
		context.Response.StatusCode = statusCode;
		await context.Write("text/plain; charset=utf-8", Encoding.UTF8.GetBytes(text));
	}

	public static async Task WriteHtml(this HttpListenerContext context, string text)
	{
		await context.Write("text/html; charset=utf-8", Encoding.UTF8.GetBytes(text));
	}

	public static async Task WriteJson(this HttpListenerContext context, object json)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(json, Formatting.Indented));
		await context.Write("application/json; charset=utf-8", bytes);
	}

	public static async Task Write(this HttpListenerContext context, string contentType, byte[] bytes)
	{
		context.Response.ContentType = contentType;
		context.Response.ContentLength64 = bytes.Length;
		await context.Response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
		context.Response.Close();
	}
}
