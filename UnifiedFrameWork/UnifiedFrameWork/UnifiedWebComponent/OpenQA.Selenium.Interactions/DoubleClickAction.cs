using OpenQA.Selenium.Interactions.Internal;
using System;

namespace OpenQA.Selenium.Interactions
{
	internal class DoubleClickAction : MouseAction, IAction
	{
		public DoubleClickAction(IMouse mouse, ILocatable actionTarget) : base(mouse, actionTarget)
		{
		}

		public void Perform()
		{
			base.MoveToLocation();
			base.Mouse.DoubleClick(base.ActionLocation);
		}
	}
}
