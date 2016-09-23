using System;

namespace OpenQA.Selenium.Interactions.Internal
{
	internal abstract class WebDriverAction
	{
		private ILocatable where;

		protected ILocatable ActionTarget
		{
			get
			{
				return this.where;
			}
		}

		protected WebDriverAction(ILocatable actionLocation)
		{
			this.where = actionLocation;
		}

		protected WebDriverAction()
		{
		}
	}
}
