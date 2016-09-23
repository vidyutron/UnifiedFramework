using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

namespace OpenQA.Selenium.Remote
{
	internal class HttpCommandExecutor : ICommandExecutor
	{
		private const string JsonMimeType = "application/json";

		private const string ContentTypeHeader = "application/json;charset=utf-8";

		private const string RequestAcceptHeader = "application/json, image/png";

		private Uri remoteServerUri;

		private TimeSpan serverResponseTimeout;

		private bool enableKeepAlive;

		private CommandInfoRepository commandInfoRepository = new WebDriverWireProtocolCommandInfoRepository();

		public CommandInfoRepository CommandInfoRepository
		{
			get
			{
				return this.commandInfoRepository;
			}
		}

		public HttpCommandExecutor(Uri addressOfRemoteServer, TimeSpan timeout) : this(addressOfRemoteServer, timeout, true)
		{
		}

		public HttpCommandExecutor(Uri addressOfRemoteServer, TimeSpan timeout, bool enableKeepAlive)
		{
			if (addressOfRemoteServer == null)
			{
				throw new ArgumentNullException("addressOfRemoteServer", "You must specify a remote address to connect to");
			}
			if (!addressOfRemoteServer.AbsoluteUri.EndsWith("/", StringComparison.OrdinalIgnoreCase))
			{
				addressOfRemoteServer = new Uri(addressOfRemoteServer.ToString() + "/");
			}
			this.remoteServerUri = addressOfRemoteServer;
			this.serverResponseTimeout = timeout;
			this.enableKeepAlive = enableKeepAlive;
			ServicePointManager.Expect100Continue = false;
			if (Type.GetType("Mono.Runtime", false, true) == null)
			{
				HttpWebRequest.DefaultMaximumErrorResponseLength = -1;
			}
		}

		public virtual Response Execute(Command commandToExecute)
		{
			if (commandToExecute == null)
			{
				throw new ArgumentNullException("commandToExecute", "commandToExecute cannot be null");
			}
			CommandInfo commandInfo = this.commandInfoRepository.GetCommandInfo(commandToExecute.Name);
			HttpWebRequest httpWebRequest = commandInfo.CreateWebRequest(this.remoteServerUri, commandToExecute);
			httpWebRequest.Timeout = (int)this.serverResponseTimeout.TotalMilliseconds;
			httpWebRequest.Accept = "application/json, image/png";
			httpWebRequest.KeepAlive = this.enableKeepAlive;
			httpWebRequest.ServicePoint.ConnectionLimit = 2000;
			if (httpWebRequest.Method == "POST")
			{
				string parametersAsJsonString = commandToExecute.ParametersAsJsonString;
				byte[] bytes = Encoding.UTF8.GetBytes(parametersAsJsonString);
				httpWebRequest.ContentType = "application/json;charset=utf-8";
				Stream requestStream = httpWebRequest.GetRequestStream();
				requestStream.Write(bytes, 0, bytes.Length);
				requestStream.Close();
			}
			Response response = this.CreateResponse(httpWebRequest);
			if (commandToExecute.Name == DriverCommand.NewSession && response.IsSpecificationCompliant)
			{
				this.commandInfoRepository = new W3CWireProtocolCommandInfoRepository();
			}
			return response;
		}

		private static string GetTextOfWebResponse(HttpWebResponse webResponse)
		{
			Stream responseStream = webResponse.GetResponseStream();
			StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
			string text = streamReader.ReadToEnd();
			streamReader.Close();
			if (text.IndexOf('\0') >= 0)
			{
				text = text.Substring(0, text.IndexOf('\0'));
			}
			return text;
		}

		private Response CreateResponse(WebRequest request)
		{
			Response response = new Response();
			HttpWebResponse httpWebResponse = null;
			try
			{
				httpWebResponse = (request.GetResponse() as HttpWebResponse);
			}
			catch (WebException ex)
			{
				httpWebResponse = (ex.Response as HttpWebResponse);
				if (ex.Status == WebExceptionStatus.Timeout)
				{
					string format = "The HTTP request to the remote WebDriver server for URL {0} timed out after {1} seconds.";
					throw new WebDriverException(string.Format(CultureInfo.InvariantCulture, format, new object[]
					{
						request.RequestUri.AbsoluteUri,
						this.serverResponseTimeout.TotalSeconds
					}), ex);
				}
				if (ex.Response == null)
				{
					string format2 = "A exception with a null response was thrown sending an HTTP request to the remote WebDriver server for URL {0}. The status of the exception was {1}, and the message was: {2}";
					throw new WebDriverException(string.Format(CultureInfo.InvariantCulture, format2, new object[]
					{
						request.RequestUri.AbsoluteUri,
						ex.Status,
						ex.Message
					}), ex);
				}
			}
			if (httpWebResponse == null)
			{
				throw new WebDriverException("No response from server for url " + request.RequestUri.AbsoluteUri);
			}
			string textOfWebResponse = HttpCommandExecutor.GetTextOfWebResponse(httpWebResponse);
			if (httpWebResponse.ContentType != null && httpWebResponse.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
			{
				response = Response.FromJson(textOfWebResponse);
			}
			else
			{
				response.Value = textOfWebResponse;
			}
			if (this.commandInfoRepository.SpecificationLevel < 1 && (httpWebResponse.StatusCode < HttpStatusCode.OK || httpWebResponse.StatusCode >= HttpStatusCode.BadRequest))
			{
				if (httpWebResponse.StatusCode >= HttpStatusCode.BadRequest && httpWebResponse.StatusCode < HttpStatusCode.InternalServerError)
				{
					response.Status = WebDriverResult.UnhandledError;
				}
				else if (httpWebResponse.StatusCode >= HttpStatusCode.InternalServerError)
				{
					if (httpWebResponse.StatusCode == HttpStatusCode.NotImplemented)
					{
						response.Status = WebDriverResult.UnknownCommand;
					}
					else if (response.Status == WebDriverResult.Success)
					{
						response.Status = WebDriverResult.UnhandledError;
					}
				}
				else
				{
					response.Status = WebDriverResult.UnhandledError;
				}
			}
			if (response.Value is string)
			{
				response.Value = ((string)response.Value).Replace("\r\n", "\n").Replace("\n", Environment.NewLine);
			}
			httpWebResponse.Close();
			return response;
		}
	}
}
