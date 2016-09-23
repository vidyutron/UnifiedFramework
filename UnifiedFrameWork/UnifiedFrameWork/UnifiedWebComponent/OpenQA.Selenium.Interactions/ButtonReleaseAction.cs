using OpenQA.Selenium.Interactions.Internal;
using System;

namespace OpenQA.Selenium.Interactions
{
	internal class ButtonReleaseAction : MouseAction, IAction
	{
		public ButtonReleaseAction(IMouse mouse, ILocatable actionTarget) : base(mouse, actionTarget)
		{
		}

		public void Perform()
		{
			base.MoveToLocation();
			base.Mouse.MouseUp(base.ActionLocation);
		}
	}
}
