using OpenQA.Selenium.Internal;
using System;

namespace OpenQA.Selenium.Interactions
{
	public class TouchActions : Actions
	{
		private ITouchScreen touchScreen;

		public TouchActions(IWebDriver driver) : base(driver)
		{
			IHasTouchScreen hasTouchScreen = driver as IHasTouchScreen;
			if (hasTouchScreen == null)
			{
				for (IWrapsDriver wrapsDriver = driver as IWrapsDriver; wrapsDriver != null; wrapsDriver = (wrapsDriver.WrappedDriver as IWrapsDriver))
				{
					hasTouchScreen = (wrapsDriver.WrappedDriver as IHasTouchScreen);
					if (hasTouchScreen != null)
					{
						break;
					}
				}
			}
			if (hasTouchScreen == null)
			{
				throw new ArgumentException("The IWebDriver object must implement or wrap a driver that implements IHasTouchScreen.", "driver");
			}
			this.touchScreen = hasTouchScreen.TouchScreen;
		}

		public TouchActions SingleTap(IWebElement onElement)
		{
			ILocatable locatableFromElement = Actions.GetLocatableFromElement(onElement);
			base.AddAction(new SingleTapAction(this.touchScreen, locatableFromElement));
			return this;
		}

		public TouchActions Down(int locationX, int locationY)
		{
			base.AddAction(new ScreenPressAction(this.touchScreen, locationX, locationY));
			return this;
		}

		public TouchActions Up(int locationX, int locationY)
		{
			base.AddAction(new ScreenReleaseAction(this.touchScreen, locationX, locationY));
			return this;
		}

		public TouchActions Move(int locationX, int locationY)
		{
			base.AddAction(new ScreenMoveAction(this.touchScreen, locationX, locationY));
			return this;
		}

		public TouchActions Scroll(IWebElement onElement, int offsetX, int offsetY)
		{
			ILocatable locatableFromElement = Actions.GetLocatableFromElement(onElement);
			base.AddAction(new ScrollAction(this.touchScreen, locatableFromElement, offsetX, offsetY));
			return this;
		}

		public TouchActions DoubleTap(IWebElement onElement)
		{
			ILocatable locatableFromElement = Actions.GetLocatableFromElement(onElement);
			base.AddAction(new DoubleTapAction(this.touchScreen, locatableFromElement));
			return this;
		}

		public TouchActions LongPress(IWebElement onElement)
		{
			ILocatable locatableFromElement = Actions.GetLocatableFromElement(onElement);
			base.AddAction(new LongPressAction(this.touchScreen, locatableFromElement));
			return this;
		}

		public TouchActions Scroll(int offsetX, int offsetY)
		{
			base.AddAction(new ScrollAction(this.touchScreen, offsetX, offsetY));
			return this;
		}

		public TouchActions Flick(int speedX, int speedY)
		{
			base.AddAction(new FlickAction(this.touchScreen, speedX, speedY));
			return this;
		}

		public TouchActions Flick(IWebElement onElement, int offsetX, int offsetY, int speed)
		{
			ILocatable locatableFromElement = Actions.GetLocatableFromElement(onElement);
			base.AddAction(new FlickAction(this.touchScreen, locatableFromElement, offsetX, offsetY, speed));
			return this;
		}
	}
}
