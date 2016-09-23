using OpenQA.Selenium.Interactions.Internal;
using System;

namespace OpenQA.Selenium.Interactions
{
	internal class SendKeysAction : KeyboardAction, IAction
	{
		private string keysToSend;

		public SendKeysAction(IKeyboard keyboard, IMouse mouse, ILocatable actionTarget, string keysToSend) : base(keyboard, mouse, actionTarget)
		{
			this.keysToSend = keysToSend;
		}

		public void Perform()
		{
			base.FocusOnElement();
			base.Keyboard.SendKeys(this.keysToSend);
		}
	}
}
