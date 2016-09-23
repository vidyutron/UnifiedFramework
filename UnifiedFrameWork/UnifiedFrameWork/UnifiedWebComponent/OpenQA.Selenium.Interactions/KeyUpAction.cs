using OpenQA.Selenium.Interactions.Internal;
using System;

namespace OpenQA.Selenium.Interactions
{
	internal class KeyUpAction : SingleKeyAction, IAction
	{
		public KeyUpAction(IKeyboard keyboard, IMouse mouse, ILocatable actionTarget, string key) : base(keyboard, mouse, actionTarget, key)
		{
		}

		public void Perform()
		{
			base.FocusOnElement();
			base.Keyboard.ReleaseKey(base.Key);
		}
	}
}
