using OpenQA.Selenium.Interactions.Internal;
using System;

namespace OpenQA.Selenium.Interactions
{
	internal class MoveMouseAction : MouseAction, IAction
	{
		public MoveMouseAction(IMouse mouse, ILocatable actionTarget) : base(mouse, actionTarget)
		{
			if (actionTarget == null)
			{
				throw new ArgumentException("Must provide a location for a move action.", "actionTarget");
			}
		}

		public void Perform()
		{
			base.Mouse.MouseMove(base.ActionLocation);
		}
	}
}
