using OpenQA.Selenium.Interactions.Internal;
using System;

namespace OpenQA.Selenium.Interactions
{
	internal class KeyDownAction : SingleKeyAction, IAction
	{
		public KeyDownAction(IKeyboard keyboard, IMouse mouse, ILocatable actionTarget, string key) : base(keyboard, mouse, actionTarget, key)
		{
		}

		public void Perform()
		{
			base.FocusOnElement();
			base.Keyboard.PressKey(base.Key);
		}
	}
}
