using System;

namespace OpenQA.Selenium.Remote
{
	internal class DriverServiceCommandExecutor : ICommandExecutor
	{
		private DriverService service;

		private HttpCommandExecutor internalExecutor;

		public CommandInfoRepository CommandInfoRepository
		{
			get
			{
				return this.internalExecutor.CommandInfoRepository;
			}
		}

		public DriverServiceCommandExecutor(DriverService driverService, TimeSpan commandTimeout) : this(driverService, commandTimeout, true)
		{
		}

		public DriverServiceCommandExecutor(DriverService driverService, TimeSpan commandTimeout, bool enableKeepAlive)
		{
			this.service = driverService;
			this.internalExecutor = new HttpCommandExecutor(driverService.ServiceUrl, commandTimeout, enableKeepAlive);
		}

		public Response Execute(Command commandToExecute)
		{
			if (commandToExecute == null)
			{
				throw new ArgumentNullException("commandToExecute", "Command to execute cannot be null");
			}
			Response result = null;
			if (commandToExecute.Name == DriverCommand.NewSession)
			{
				this.service.Start();
			}
			try
			{
				result = this.internalExecutor.Execute(commandToExecute);
			}
			finally
			{
				if (commandToExecute.Name == DriverCommand.Quit)
				{
					this.service.Dispose();
				}
			}
			return result;
		}
	}
}
