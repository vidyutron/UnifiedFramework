using OpenQA.Selenium.Interactions.Internal;
using System;

namespace OpenQA.Selenium.Interactions
{
	internal class ClickAndHoldAction : MouseAction, IAction
	{
		public ClickAndHoldAction(IMouse mouse, ILocatable actionTarget) : base(mouse, actionTarget)
		{
		}

		public void Perform()
		{
			base.MoveToLocation();
			base.Mouse.MouseDown(base.ActionLocation);
		}
	}
}
