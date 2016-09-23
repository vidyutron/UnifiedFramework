using System;

namespace OpenQA.Selenium.Remote
{
	public interface ICommandExecutor
	{
		CommandInfoRepository CommandInfoRepository
		{
			get;
		}

		Response Execute(Command commandToExecute);
	}
}
