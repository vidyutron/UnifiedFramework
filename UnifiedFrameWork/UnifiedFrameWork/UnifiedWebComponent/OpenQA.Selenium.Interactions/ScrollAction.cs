using OpenQA.Selenium.Interactions.Internal;
using System;

namespace OpenQA.Selenium.Interactions
{
	internal class ScrollAction : TouchAction, IAction
	{
		private int offsetX;

		private int offsetY;

		public ScrollAction(ITouchScreen touchScreen, int offsetX, int offsetY) : this(touchScreen, null, offsetX, offsetY)
		{
		}

		public ScrollAction(ITouchScreen touchScreen, ILocatable actionTarget, int offsetX, int offsetY) : base(touchScreen, actionTarget)
		{
			if (actionTarget == null)
			{
				throw new ArgumentException("Must provide a location for a single tap action.", "actionTarget");
			}
			this.offsetX = offsetX;
			this.offsetY = offsetY;
		}

		public void Perform()
		{
			base.TouchScreen.Scroll(base.ActionLocation, this.offsetX, this.offsetY);
		}
	}
}
