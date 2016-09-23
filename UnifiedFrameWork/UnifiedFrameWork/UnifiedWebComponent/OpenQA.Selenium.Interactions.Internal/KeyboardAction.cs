using System;

namespace OpenQA.Selenium.Interactions.Internal
{
	internal class KeyboardAction : WebDriverAction
	{
		private IKeyboard keyboard;

		private IMouse mouse;

		protected IKeyboard Keyboard
		{
			get
			{
				return this.keyboard;
			}
		}

		protected KeyboardAction(IKeyboard keyboard, IMouse mouse, ILocatable actionTarget) : base(actionTarget)
		{
			this.keyboard = keyboard;
			this.mouse = mouse;
		}

		protected void FocusOnElement()
		{
			if (base.ActionTarget != null)
			{
				this.mouse.Click(base.ActionTarget.Coordinates);
			}
		}
	}
}
