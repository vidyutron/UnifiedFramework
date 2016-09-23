using OpenQA.Selenium.Remote;
using System;

namespace OpenQA.Selenium.Firefox
{
	public class FirefoxDriverCommandExecutor : ICommandExecutor, IDisposable
	{
		private FirefoxDriverServer server;

		private HttpCommandExecutor internalExecutor;

		private TimeSpan commandTimeout;

		private bool isDisposed;

		public CommandInfoRepository CommandInfoRepository
		{
			get
			{
				return this.internalExecutor.CommandInfoRepository;
			}
		}

		public FirefoxDriverCommandExecutor(FirefoxBinary binary, FirefoxProfile profile, string host, TimeSpan commandTimeout)
		{
			this.server = new FirefoxDriverServer(binary, profile, host);
			this.commandTimeout = commandTimeout;
		}

		public Response Execute(Command commandToExecute)
		{
			if (commandToExecute == null)
			{
				throw new ArgumentNullException("commandToExecute", "Command may not be null");
			}
			Response result = null;
			if (commandToExecute.Name == DriverCommand.NewSession)
			{
				this.server.Start();
				this.internalExecutor = new HttpCommandExecutor(this.server.ExtensionUri, this.commandTimeout);
			}
			try
			{
				result = this.internalExecutor.Execute(commandToExecute);
			}
			finally
			{
				if (commandToExecute.Name == DriverCommand.Quit)
				{
					this.Dispose();
				}
			}
			return result;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.isDisposed)
			{
				if (disposing)
				{
					this.server.Dispose();
				}
				this.isDisposed = true;
			}
		}
	}
}
