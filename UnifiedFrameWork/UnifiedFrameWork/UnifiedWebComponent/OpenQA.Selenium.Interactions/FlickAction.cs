using OpenQA.Selenium.Interactions.Internal;
using System;

namespace OpenQA.Selenium.Interactions
{
	internal class FlickAction : TouchAction, IAction
	{
		private int offsetX;

		private int offsetY;

		private int speed;

		private int speedX;

		private int speedY;

		public FlickAction(ITouchScreen touchScreen, int speedX, int speedY) : base(touchScreen, null)
		{
			this.speedX = speedX;
			this.speedY = speedY;
		}

		public FlickAction(ITouchScreen touchScreen, ILocatable actionTarget, int offsetX, int offsetY, int speed) : base(touchScreen, actionTarget)
		{
			if (actionTarget == null)
			{
				throw new ArgumentException("Must provide a location for a single tap action.", "actionTarget");
			}
			this.offsetX = offsetX;
			this.offsetY = offsetY;
			this.speed = speed;
		}

		public void Perform()
		{
			if (base.ActionLocation != null)
			{
				base.TouchScreen.Flick(base.ActionLocation, this.offsetX, this.offsetY, this.speed);
				return;
			}
			base.TouchScreen.Flick(this.speedX, this.speedY);
		}
	}
}
