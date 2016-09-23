using System;

namespace OpenQA.Selenium.Interactions.Internal
{
	internal class TouchAction : WebDriverAction
	{
		private ITouchScreen touchScreen;

		protected ITouchScreen TouchScreen
		{
			get
			{
				return this.touchScreen;
			}
		}

		protected ICoordinates ActionLocation
		{
			get
			{
				if (base.ActionTarget != null)
				{
					return base.ActionTarget.Coordinates;
				}
				return null;
			}
		}

		protected TouchAction(ITouchScreen touchScreen, ILocatable actionTarget) : base(actionTarget)
		{
			this.touchScreen = touchScreen;
		}
	}
}
