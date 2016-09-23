using OpenQA.Selenium.Interactions.Internal;
using System;

namespace OpenQA.Selenium.Interactions
{
	internal class ScreenMoveAction : TouchAction, IAction
	{
		private int x;

		private int y;

		public ScreenMoveAction(ITouchScreen touchScreen, int x, int y) : base(touchScreen, null)
		{
			this.x = x;
			this.y = y;
		}

		public void Perform()
		{
			base.TouchScreen.Move(this.x, this.y);
		}
	}
}
