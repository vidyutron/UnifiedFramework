using OpenQA.Selenium.Interactions.Internal;
using System;

namespace OpenQA.Selenium.Interactions
{
	internal class ClickAction : MouseAction, IAction
	{
		public ClickAction(IMouse mouse, ILocatable actionTarget) : base(mouse, actionTarget)
		{
		}

		public void Perform()
		{
			base.MoveToLocation();
			base.Mouse.Click(base.ActionLocation);
		}
	}
}
