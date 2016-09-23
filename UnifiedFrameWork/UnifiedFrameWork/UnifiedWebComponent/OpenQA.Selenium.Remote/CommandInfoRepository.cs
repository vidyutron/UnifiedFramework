using System;
using System.Collections.Generic;

namespace OpenQA.Selenium.Remote
{
	public abstract class CommandInfoRepository
	{
		private readonly Dictionary<string, CommandInfo> commandDictionary;

		public abstract int SpecificationLevel
		{
			get;
		}

		protected CommandInfoRepository()
		{
			this.commandDictionary = new Dictionary<string, CommandInfo>();
		}

		public CommandInfo GetCommandInfo(string commandName)
		{
			CommandInfo result = null;
			if (this.commandDictionary.ContainsKey(commandName))
			{
				result = this.commandDictionary[commandName];
			}
			return result;
		}

		public bool TryAddCommand(string commandName, CommandInfo commandInfo)
		{
			if (string.IsNullOrEmpty(commandName))
			{
				throw new ArgumentNullException("commandName", "The name of the command cannot be null or the empty string.");
			}
			if (commandInfo == null)
			{
				throw new ArgumentNullException("commandInfo", "The command information object cannot be null.");
			}
			if (this.commandDictionary.ContainsKey(commandName))
			{
				return false;
			}
			this.commandDictionary.Add(commandName, commandInfo);
			return true;
		}

		protected abstract void InitializeCommandDictionary();
	}
}
