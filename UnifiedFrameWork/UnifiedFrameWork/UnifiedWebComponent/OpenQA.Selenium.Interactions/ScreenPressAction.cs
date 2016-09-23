using OpenQA.Selenium.Interactions.Internal;
using System;

namespace OpenQA.Selenium.Interactions
{
	internal class ScreenPressAction : TouchAction, IAction
	{
		private int x;

		private int y;

		public ScreenPressAction(ITouchScreen touchScreen, int x, int y) : base(touchScreen, null)
		{
			this.x = x;
			this.y = y;
		}

		public void Perform()
		{
			base.TouchScreen.Down(this.x, this.y);
		}
	}
}
