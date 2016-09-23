using OpenQA.Selenium.Interactions.Internal;
using System;

namespace OpenQA.Selenium.Interactions
{
	internal class ContextClickAction : MouseAction, IAction
	{
		public ContextClickAction(IMouse mouse, ILocatable actionTarget) : base(mouse, actionTarget)
		{
		}

		public void Perform()
		{
			base.MoveToLocation();
			base.Mouse.ContextClick(base.ActionLocation);
		}
	}
}
