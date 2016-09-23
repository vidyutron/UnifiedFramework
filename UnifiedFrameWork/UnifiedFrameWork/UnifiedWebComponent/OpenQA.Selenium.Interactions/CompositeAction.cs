using System;
using System.Collections.Generic;

namespace OpenQA.Selenium.Interactions
{
	internal class CompositeAction : IAction
	{
		private List<IAction> actionsList = new List<IAction>();

		public CompositeAction AddAction(IAction action)
		{
			this.actionsList.Add(action);
			return this;
		}

		public void Perform()
		{
			foreach (IAction current in this.actionsList)
			{
				current.Perform();
			}
		}
	}
}
