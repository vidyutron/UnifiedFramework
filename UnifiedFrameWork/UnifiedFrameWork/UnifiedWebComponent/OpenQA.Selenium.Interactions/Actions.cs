using OpenQA.Selenium.Internal;
using System;

namespace OpenQA.Selenium.Interactions
{
	public class Actions
	{
		private IKeyboard keyboard;

		private IMouse mouse;

		private CompositeAction action = new CompositeAction();

		public Actions(IWebDriver driver)
		{
			IHasInputDevices hasInputDevices = driver as IHasInputDevices;
			if (hasInputDevices == null)
			{
				for (IWrapsDriver wrapsDriver = driver as IWrapsDriver; wrapsDriver != null; wrapsDriver = (wrapsDriver.WrappedDriver as IWrapsDriver))
				{
					hasInputDevices = (wrapsDriver.WrappedDriver as IHasInputDevices);
					if (hasInputDevices != null)
					{
						break;
					}
				}
			}
			if (hasInputDevices == null)
			{
				throw new ArgumentException("The IWebDriver object must implement or wrap a driver that implements IHasInputDevices.", "driver");
			}
			this.keyboard = hasInputDevices.Keyboard;
			this.mouse = hasInputDevices.Mouse;
		}

		public Actions KeyDown(string theKey)
		{
			return this.KeyDown(null, theKey);
		}

		public Actions KeyDown(IWebElement element, string theKey)
		{
			ILocatable locatableFromElement = Actions.GetLocatableFromElement(element);
			this.action.AddAction(new KeyDownAction(this.keyboard, this.mouse, locatableFromElement, theKey));
			return this;
		}

		public Actions KeyUp(string theKey)
		{
			return this.KeyUp(null, theKey);
		}

		public Actions KeyUp(IWebElement element, string theKey)
		{
			ILocatable locatableFromElement = Actions.GetLocatableFromElement(element);
			this.action.AddAction(new KeyUpAction(this.keyboard, this.mouse, locatableFromElement, theKey));
			return this;
		}

		public Actions SendKeys(string keysToSend)
		{
			return this.SendKeys(null, keysToSend);
		}

		public Actions SendKeys(IWebElement element, string keysToSend)
		{
			ILocatable locatableFromElement = Actions.GetLocatableFromElement(element);
			this.action.AddAction(new SendKeysAction(this.keyboard, this.mouse, locatableFromElement, keysToSend));
			return this;
		}

		public Actions ClickAndHold(IWebElement onElement)
		{
			ILocatable locatableFromElement = Actions.GetLocatableFromElement(onElement);
			this.action.AddAction(new ClickAndHoldAction(this.mouse, locatableFromElement));
			return this;
		}

		public Actions ClickAndHold()
		{
			return this.ClickAndHold(null);
		}

		public Actions Release(IWebElement onElement)
		{
			ILocatable locatableFromElement = Actions.GetLocatableFromElement(onElement);
			this.action.AddAction(new ButtonReleaseAction(this.mouse, locatableFromElement));
			return this;
		}

		public Actions Release()
		{
			return this.Release(null);
		}

		public Actions Click(IWebElement onElement)
		{
			ILocatable locatableFromElement = Actions.GetLocatableFromElement(onElement);
			this.action.AddAction(new ClickAction(this.mouse, locatableFromElement));
			return this;
		}

		public Actions Click()
		{
			return this.Click(null);
		}

		public Actions DoubleClick(IWebElement onElement)
		{
			ILocatable locatableFromElement = Actions.GetLocatableFromElement(onElement);
			this.action.AddAction(new DoubleClickAction(this.mouse, locatableFromElement));
			return this;
		}

		public Actions DoubleClick()
		{
			return this.DoubleClick(null);
		}

		public Actions MoveToElement(IWebElement toElement)
		{
			ILocatable locatableFromElement = Actions.GetLocatableFromElement(toElement);
			this.action.AddAction(new MoveMouseAction(this.mouse, locatableFromElement));
			return this;
		}

		public Actions MoveToElement(IWebElement toElement, int offsetX, int offsetY)
		{
			ILocatable locatableFromElement = Actions.GetLocatableFromElement(toElement);
			this.action.AddAction(new MoveToOffsetAction(this.mouse, locatableFromElement, offsetX, offsetY));
			return this;
		}

		public Actions MoveByOffset(int offsetX, int offsetY)
		{
			return this.MoveToElement(null, offsetX, offsetY);
		}

		public Actions ContextClick(IWebElement onElement)
		{
			ILocatable locatableFromElement = Actions.GetLocatableFromElement(onElement);
			this.action.AddAction(new ContextClickAction(this.mouse, locatableFromElement));
			return this;
		}

		public Actions ContextClick()
		{
			return this.ContextClick(null);
		}

		public Actions DragAndDrop(IWebElement source, IWebElement target)
		{
			ILocatable locatableFromElement = Actions.GetLocatableFromElement(source);
			ILocatable locatableFromElement2 = Actions.GetLocatableFromElement(target);
			this.action.AddAction(new ClickAndHoldAction(this.mouse, locatableFromElement));
			this.action.AddAction(new MoveMouseAction(this.mouse, locatableFromElement2));
			this.action.AddAction(new ButtonReleaseAction(this.mouse, locatableFromElement2));
			return this;
		}

		public Actions DragAndDropToOffset(IWebElement source, int offsetX, int offsetY)
		{
			ILocatable locatableFromElement = Actions.GetLocatableFromElement(source);
			this.action.AddAction(new ClickAndHoldAction(this.mouse, locatableFromElement));
			this.action.AddAction(new MoveToOffsetAction(this.mouse, null, offsetX, offsetY));
			this.action.AddAction(new ButtonReleaseAction(this.mouse, null));
			return this;
		}

		public IAction Build()
		{
			CompositeAction result = this.action;
			this.action = new CompositeAction();
			return result;
		}

		public void Perform()
		{
			this.Build().Perform();
		}

		protected static ILocatable GetLocatableFromElement(IWebElement element)
		{
			if (element == null)
			{
				return null;
			}
			ILocatable locatable = element as ILocatable;
			if (locatable == null)
			{
				for (IWrapsElement wrapsElement = element as IWrapsElement; wrapsElement != null; wrapsElement = (wrapsElement.WrappedElement as IWrapsElement))
				{
					locatable = (wrapsElement.WrappedElement as ILocatable);
					if (locatable != null)
					{
						break;
					}
				}
			}
			if (locatable == null)
			{
				throw new ArgumentException("The IWebElement object must implement or wrap an element that implements ILocatable.", "element");
			}
			return locatable;
		}

		protected void AddAction(IAction actionToAdd)
		{
			this.action.AddAction(actionToAdd);
		}
	}
}
