using OpenQA.Selenium.Interactions.Internal;
using System;

namespace OpenQA.Selenium.Interactions
{
	internal class ScreenReleaseAction : TouchAction, IAction
	{
		private int x;

		private int y;

		public ScreenReleaseAction(ITouchScreen touchScreen, int x, int y) : base(touchScreen, null)
		{
			this.x = x;
			this.y = y;
		}

		public void Perform()
		{
			base.TouchScreen.Up(this.x, this.y);
		}
	}
}
