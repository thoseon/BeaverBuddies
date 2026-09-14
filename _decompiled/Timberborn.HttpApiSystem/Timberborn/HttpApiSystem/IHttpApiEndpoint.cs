using System.Net;
using System.Threading.Tasks;

namespace Timberborn.HttpApiSystem;

public interface IHttpApiEndpoint
{
	Task<bool> TryHandle(HttpListenerContext context);
}
