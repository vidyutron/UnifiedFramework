using OpenQA.Selenium.Interactions.Internal;
using System;

namespace OpenQA.Selenium.Interactions
{
	internal class MoveToOffsetAction : MouseAction, IAction
	{
		private int offsetX;

		private int offsetY;

		public MoveToOffsetAction(IMouse mouse, ILocatable actionTarget, int offsetX, int offsetY) : base(mouse, actionTarget)
		{
			this.offsetX = offsetX;
			this.offsetY = offsetY;
		}

		public void Perform()
		{
			base.Mouse.MouseMove(base.ActionLocation, this.offsetX, this.offsetY);
		}
	}
}
