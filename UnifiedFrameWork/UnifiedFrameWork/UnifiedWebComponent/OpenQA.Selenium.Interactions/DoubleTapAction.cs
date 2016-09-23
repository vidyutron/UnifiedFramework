using OpenQA.Selenium.Interactions.Internal;
using System;

namespace OpenQA.Selenium.Interactions
{
	internal class DoubleTapAction : TouchAction, IAction
	{
		public DoubleTapAction(ITouchScreen touchScreen, ILocatable actionTarget) : base(touchScreen, actionTarget)
		{
			if (actionTarget == null)
			{
				throw new ArgumentException("Must provide a location for a single tap action.", "actionTarget");
			}
		}

		public void Perform()
		{
			base.TouchScreen.DoubleTap(base.ActionLocation);
		}
	}
}
